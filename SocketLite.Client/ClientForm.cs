using SocketLite.Forms;
using SocketLite.Net;
using System;
using System.Net.Sockets;
using System.Text;

namespace SocketLite.Client
{
    public class ClientForm : FormBase
    {
        protected static string loginUrl = string.Empty;
        private static AsyncTcpClient tcpClient;
        protected AsyncContext context;

        #region Log
        protected void WriteLog(string message)
        {
            if (context != null)
                context.Logger.WriteLog(message);
        }

        protected void WriteLog(string format, params object[] args)
        {
            if (context != null)
                context.Logger.WriteLog(format, args);
        }
        #endregion

        #region Tcp
        protected void ConnectServer()
        {
            var server = ConfigHelper.AppSettings("Server");
            var index = server.LastIndexOf(':');
            var host = server.Substring(0, index);
            var port = int.Parse(server.Substring(index + 1));
            ConnectServer(host, port);
        }

        protected void ConnectServer(string remoteHostName, int remotePort)
        {
            if (tcpClient != null)
                return;

            tcpClient = new AsyncTcpClient(remoteHostName, remotePort);
            tcpClient.Encoding = Encoding.UTF8;
            tcpClient.ServerConnected += TcpClient_ServerConnected;
            tcpClient.ServerDisconnected += TcpClient_ServerDisconnected;
            tcpClient.ServerExceptionOccurred += TcpClient_ServerExceptionOccurred;
            tcpClient.DatagramReceived += TcpClient_DatagramReceived;
            tcpClient.Connect();
        }

        protected void SendRequest(string handler, string action = null, object data = null)
        {
            if (tcpClient == null)
                return;

            var info = new RequestInfo
            {
                ClientId = Utils.GetMac(),
                Handler = handler,
                Action = action,
                ParamJson = Utils.Serialize(data),
                RequestTime = DateTime.Now
            };
            var json = Utils.Serialize(info);
            tcpClient.Send(json);
        }
        #endregion

        #region TcpClientEvents
        private void TcpClient_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            WriteLog("连接成功！");
        }

        private void TcpClient_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            if (tcpClient != null && !tcpClient.Connected)
            {
                WriteLog("连接已断开，重新连接......");
                tcpClient.Connect();
            }
        }

        private void TcpClient_ServerExceptionOccurred(object sender, TcpServerExceptionOccurredEventArgs e)
        {
            if (e.Exception != null)
            {
                WriteLog("服务端异常：{0}", e.Exception.Message);
            }
        }

        private void TcpClient_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            if (e.Datagram != null)
            {
                var message = Encoding.UTF8.GetString(e.Datagram);
                var response = Utils.Deserialize<ResponseInfo>(message);
                DoResponse(e.TcpClient, response);
            }
        }
        #endregion

        #region Private
        private void DoResponse(TcpClient tcpClient, ResponseInfo response)
        {
            switch (response.Handler)
            {
                case "Connect":
                    WriteLog(response.DataJson);
                    ConnectSuccess();
                    break;
                default:
                    WriteLog(response.DataJson);
                    ResponseFinished();
                    break;
            }
        }
        #endregion

        #region Protected
        protected virtual void ConnectSuccess()
        {
        }

        protected virtual void ResponseFinished()
        {
        }
        #endregion
    }
}
