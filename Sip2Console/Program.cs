using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Sip2Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sip = new Sip2Connection("127.0.0.1", 6001)) //stunnel server IP and port
            {
                sip.Open();
                LogMessage("Connected ...");

                var patron = GetPatronMessage();
                LogMessage(patron);
                var response = sip.SendMessage(patron);
                LogMessage(response);
            }
            Console.Write("\nPress any key to continue... ");
            Console.ReadLine();
        }

        private static string GetPatronMessage()
        {

            var dateFld = DateTime.Now.ToString(Sip2Command.SipDatetime);
            var insitutionId = "9999";
            var barcode = "00665609";
            var pin = "1558";
            var seqNum = "0";


            var patronMessage = $"{Sip2Command.PatronInfo}001{dateFld}{Sip2Command.FidInstId}{insitutionId}|{Sip2Command.FidPatronId}{barcode}|{Sip2Command.FidPatronPwd}{pin}|{Sip2Command.FidSeq}{seqNum}{Sip2Command.FidCksum}";
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
