using SocketLite.Logs;
using System;
using System.Collections.Generic;

namespace SocketLite
{
    public class AsyncContext
    {
        public AsyncContext() : this(new ConsoleLogger()) { }

        public AsyncContext(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException("logger");
            Params = new Dictionary<string, object>();
        }

        public ILogger Logger { get; private set; }
        public Dictionary<string, object> Params { get; private set; }

        public T Param<T>(string key)
        {
            var value = Params[key];
            if (value == null)
                return default(T);

            return (T)value;
        }
    }
}
