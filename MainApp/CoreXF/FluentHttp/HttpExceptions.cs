
using System;
using System.Net;

namespace CoreXF
{

    public class NoInetException : Exception
    {

    }

    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode Code;

        public HttpStatusCodeException(HttpStatusCode code, string messege = "HttpStatusCodeException") : base(messege)
        {
            Code = code;
        }
    }

}
