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
            LogMessage("Input SIP2 server IP and press Enter:");
            var sip2Server = Console.ReadLine();

            LogMessage("Input SIP2 server Port and press Enter:");
            var sip2Port = Convert.ToInt32(Console.ReadLine());

            using (var sip = new Sip2Connection(sip2Server, sip2Port)) //SIP2 server IP and port
            {
                sip.Open();
                LogMessage("Connected ...");
                
                LogMessage("Input barcode and press Enter:");
                var barcode = Console.ReadLine();
                
                LogMessage("Input pin and press Enter:");
                var pin = Console.ReadLine();

                LogMessage("Input SIP2 Server Password:");
                var serverLoginPwd = Console.ReadLine();

                var patron = GetPatronMessage(seqNum: 0, institutionId: "", barcode, pin, serverLoginPwd); // 63 message with Barcode and PIN
                LogMessage(patron);
                
                var response = sip.SendMessage(patron);
                LogMessage(response);
            }
            Console.Write("\nPress any key to continue... ");
            Console.ReadLine();
        }


        private static string GetPatronMessage(int seqNum, string institutionId, string barcode, string pin, string serverLoginPwd)
        {
            var dateFld = DateTime.Now.ToString(Sip2Command.SipDatetime);

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
