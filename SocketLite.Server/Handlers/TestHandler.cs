using System;

namespace SocketLite.Server.Handlers
{
    public class TestHandler : RequestHandler
    {
        public TestHandler(RequestContext context) : base(context)
        {
        }

        public override Type Type => typeof(TestHandler);

        public ResponseInfo Hello(string paramJson)
        {
            return CreateResponse($"Hello! You send data: {paramJson}");
        }
    }
}
