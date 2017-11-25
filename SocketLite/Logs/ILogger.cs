namespace SocketLite.Logs
{
    public interface ILogger
    {
        bool ServerWrite { get; }
        bool AddTime { get; }
        void Clear();
        void WriteLog(string message);
        void WriteLog(string format, params object[] args);
        void WriteServerLog(string message);
        void WriteServerLog(string format, params object[] args);
    }
}
