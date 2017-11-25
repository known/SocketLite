using System;

namespace SocketLite.Logs
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() : base(false) { }

        public ConsoleLogger(bool addTime) : base(addTime) { }

        public override void Clear()
        {
            Console.Clear();
        }

        protected override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
