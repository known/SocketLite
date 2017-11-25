using SocketLite.Logs;

namespace SocketLite
{
    public class RequestContext : AsyncContext
    {
        public RequestContext() : this(new ConsoleLogger()) { }

        public RequestContext(ILogger logger) : base(logger)
        {
        }

        public RequestInfo Request { get; set; }
    }
}
