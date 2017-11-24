using System;

namespace SocketLite
{
    public class ResponseInfo
    {
        public string ClientId { get; set; }
        public string Handler { get; set; }
        public string Action { get; set; }
        public string DataJson { get; set; }
        public DateTime ResponseTime { get; set; }

        public static ResponseInfo Create(RequestInfo request, object data)
        {
            return new ResponseInfo
            {
                ClientId = request.ClientId,
                Handler = request.Handler,
                Action = request.Action,
                DataJson = Utils.Serialize(data),
                ResponseTime = DateTime.Now
            };
        }
    }
}
