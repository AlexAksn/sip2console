using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Sip2Console
{
    internal class Sip2Connection : IDisposable
    {
        private readonly string _hostname;
        private readonly int _port;
        private NetworkStream _ns;
        private TcpClient _sipClient;


        public Sip2Connection(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
        }


        public void Dispose()
        {
            _sipClient?.Close();
            _ns?.Close();
        }


        public void Open()
        {
            _sipClient = new TcpClient { ReceiveTimeout = 60 * 1000, NoDelay = true };
            if (!_sipClient.ConnectAsync(_hostname, _port).Wait(TimeSpan.FromSeconds(60)))
                throw new TimeoutException();
            _ns = _sipClient.GetStream();
        }


        public string SendMessage(string message)
        {
            try
            {
                var buffer = Encoding.ASCII.GetBytes(message);
                _ns.Write(buffer, 0, buffer.Length);
                // read response
                var response = new byte[512];
                var responseData = new StringBuilder();
                var duration = TimeSpan.FromSeconds(60);
                var sw = Stopwatch.StartNew();
                do
                {
                    var bytes = _ns.Read(response, 0, response.Length);
                    responseData.Append(Encoding.ASCII.GetString(response, 0, bytes));
                } while (sw.Elapsed < duration && _ns.CanRead &&
                            !(responseData.ToString().EndsWith("\r") || responseData.ToString().EndsWith("\n")));

                var result = responseData.ToString();
                return result;
            }
            catch (Exception e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
                return "No response from server";
            }

        }
    }
}
