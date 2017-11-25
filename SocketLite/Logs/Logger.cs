using System;

namespace SocketLite.Logs
{
    public abstract class Logger : ILogger
    {
        protected Logger(bool addTime)
        {
            AddTime = addTime;
        }

        public bool ServerWrite { get; private set; }
        public bool AddTime { get; }

        public abstract void Clear();
        protected abstract void WriteLine(string message);

        public void WriteLog(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (AddTime)
                message = string.Format("{0:yy-MM-dd HH:mm:ss} {1}", DateTime.Now, message);

            WriteLine(message);
            System.Threading.Thread.Sleep(100);
        }

        public void WriteLog(string format, params object[] args)
        {
            var message = string.Format(format, args);
            WriteLog(message);
        }

        public void WriteServerLog(string message)
        {
            ServerWrite = true;
            WriteLog(message);
            ServerWrite = false;
        }

        public void WriteServerLog(string format, params object[] args)
        {
            ServerWrite = true;
            WriteLog(format, args);
            ServerWrite = false;
        }
    }
}
