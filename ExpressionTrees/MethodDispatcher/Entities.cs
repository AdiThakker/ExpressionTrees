using System;

namespace ExpressionTrees.MethodDispatcher
{
    #region Requests

    class HttpRequest { }
    class FtpRequest { }
    class RemoteConnectionRequest { }

    #endregion

    #region Handlers

    class HttpRequestHandler
    {
        public Type HandledType
        {
            get { return typeof(HttpRequest); }
        }

        public void Handle(HttpRequest request)
        {
            //Console.WriteLine("Http request handled!");
        }
    }

    class FtpRequestHandler
    {
        public Type HandledType
        {
            get { return typeof(FtpRequest); }
        }

        public void Handle(FtpRequest request)
        {
            //Console.WriteLine("Ftp request handled!");
        }
    }

    class RemoteConnectionRequestHandler
    {
        public Type HandledType
        {
            get { return typeof(RemoteConnectionRequest); }
        }

        public void Handle(RemoteConnectionRequest request)
        {
            //Console.WriteLine("Remote Connection request handled!");
        }
    }

    #endregion

}
