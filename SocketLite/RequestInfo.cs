using System;

namespace SocketLite
{
    public class RequestInfo
    {
        public string ClientId { get; set; }
        public string Handler { get; set; }
        public string Action { get; set; }
        public string ParamJson { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
