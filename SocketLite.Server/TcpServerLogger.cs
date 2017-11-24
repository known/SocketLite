using SocketLite.Log;
using SocketLite.Net;
using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SocketLite.Server
{
    class TcpServerLogger : Logger
    {
        private RichTextBox rtbLog;
        private RequestInfo request;
        private AsyncTcpServer tcpServer;
        private TcpClient tcpClient;

        public TcpServerLogger(RichTextBox rtbLog, RequestInfo request, AsyncTcpServer tcpServer, TcpClient tcpClient) : base(false)
        {
            this.rtbLog = rtbLog;
            this.request = request;
            this.tcpServer = tcpServer;
            this.tcpClient = tcpClient;
        }

        public override void Clear()
        {
            rtbLog.Clear();
        }

        protected override void WriteLine(string message)
        {
            var rtbMessage = string.Format("{0:yy-MM-dd HH:mm:ss} {1}", DateTime.Now, message);
            if (!string.IsNullOrEmpty(message))
            {
                rtbLog.AppendText(rtbMessage);
                rtbLog.AppendText(Environment.NewLine);
            }

            if (tcpServer != null && !ServerWrite)
            {
                var info = ResponseInfo.Create(request, message);
                tcpServer.Send(tcpClient, Utils.Serialize(info));
            }

            System.Threading.Thread.Sleep(100);
        }
    }
}
