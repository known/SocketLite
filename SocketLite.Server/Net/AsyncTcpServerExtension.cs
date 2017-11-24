using System.Net.Sockets;

namespace SocketLite.Net
{
    public static class AsyncTcpServerExtension
    {
        public static void Send(this AsyncTcpServer tcpServer, TcpClient tcpClient, RequestInfo request, object data)
        {
            var info = ResponseInfo.Create(request, data);
            tcpServer.Send(tcpClient, Utils.Serialize(info));
        }
    }
}
