using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SocketLite
{
    public abstract class RequestHandler
    {
        protected RequestHandler(RequestContext context)
        {
            Context = context;
        }

        public RequestContext Context { get; private set; }
        public abstract Type Type { get; }

        class TypeHandler
        {
            public string Type { get; set; }
            public string Handler { get; set; }
        }
        public static RequestHandler GetHandler(RequestContext context)
        {
            var path = string.Format(@"{0}\RequestHandlers.json", Application.StartupPath);
            var json = File.ReadAllText(path);
            var typeHandlers = Utils.Deserialize<List<TypeHandler>>(json);
            if (typeHandlers == null || typeHandlers.Count == 0)
                return null;

            TypeHandler typeHandler = null;
            foreach (var item in typeHandlers)
            {
                if (item.Type == context.Request.Handler)
                {
                    typeHandler = item;
                    break;
                }
            }

            if (typeHandler == null)
                return null;

            var type = Type.GetType(typeHandler.Handler);
            if (type == null)
                return null;

            return Activator.CreateInstance(type, context) as RequestHandler;
        }

        public ResponseInfo Execute()
        {
            var method = Type.GetMethod(Context.Request.Action);
            if (method == null)
                return CreateResponse($"{Context.Request.Handler}不支持{Context.Request.Action}操作！");

            return method.Invoke(this, new object[] { Context.Request.ParamJson }) as ResponseInfo;
        }

        protected ResponseInfo CreateResponse(object data)
        {
            return ResponseInfo.Create(Context.Request, data);
        }
    }
}
