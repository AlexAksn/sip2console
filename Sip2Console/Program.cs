using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sip2Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sip = new Sip2Connection("127.0.0.1", 2222)) //stunnel server IP and port
            {
                sip.Open();
                LogMessage("Connected ...");

                // In order to get InstitutionId we send ScStatus message to ASC
                var statusMessage = GetStatusMessage(0);
                LogMessage(statusMessage);

                var statusMessageResponse = sip.SendMessage(statusMessage);
                LogMessage(statusMessageResponse);

                var institutionId = GetInstitutionIdFromStatusResponse(statusMessageResponse);

                var patron = GetPatronMessage(seqNum: 1, institutionId: institutionId, barcode: "00665609", pin: "1558");
                LogMessage(patron);
                var response = sip.SendMessage(patron);
                LogMessage(response);
            }
            Console.Write("\nPress any key to continue... ");
            Console.ReadLine();
        }

        // This message will be the first message sent by SC to the ASC once a connection has been established
        // 99<status code><max print width><protocol version>
        private static string GetStatusMessage(int seqNum)
        {
            var statusMessage = $"{Sip2Command.ScStatus}0000{Sip2Command.Version}{Sip2Command.FidSeq}{seqNum}{Sip2Command.FidCksum}";
            return AddCheckSum(statusMessage);
        }

        private static string GetInstitutionIdFromStatusResponse(string response)
        {
            if (!response.StartsWith(Sip2Command.AcsStatus, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"ACS status response did not match expected result. Response - {response}");

            var re = new Regex(Sip2Command.SipStatusRegex);
            var match = re.Match(response);
            if (!match.Success) { throw new Exception($"ACS status response did not match expected result. Response - {response}"); }

            var institutionId = match.Groups["id"].Value;
            return institutionId;
        }

        private static string GetPatronMessage(int seqNum, string institutionId, string barcode, string pin)
        {
            var dateFld = DateTime.Now.ToString(Sip2Command.SipDatetime);
            var serverLoginPwd = "";

            var patronMessage = $"{Sip2Command.PatronInfo}001{dateFld}{Sip2Command.Summary}{Sip2Command.FidInstId}{institutionId}|{Sip2Command.FidPatronId}{barcode}|{Sip2Command.FidTerminalPwd}{serverLoginPwd}|{Sip2Command.FidPatronPwd}{pin}|{Sip2Command.FidSeq}{seqNum}{Sip2Command.FidCksum}";
            return AddCheckSum(patronMessage);
        }


        private static string AddCheckSum(string message)
        {
            var chkSum = ComputeChecksum(message);
            var messChk = string.Format("{0}{1}", message, chkSum);
            return string.Format("{0}\r", messChk);
        }


        private static string ComputeChecksum(string dataToCalculate)
        {
            var byteToCalculate = Encoding.ASCII.GetBytes(dataToCalculate);
            var checksum = 0;
            foreach (var chData in byteToCalculate)
            {
                checksum += chData;
            }
            checksum = -checksum & 0xFFFF;
            return checksum.ToString("X4");
        }


        private static void LogMessage(string message,
                [CallerMemberName]string callername = "")
        {
            Console.WriteLine("{0} - Thread-{1}- {2}",
                callername, Thread.CurrentThread.ManagedThreadId, message);
        }
    }
}
