using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreXF
{
    public abstract class APIServiceAbstract
    {
        public abstract string Url { get; }

        public abstract string Token { get; }

        public abstract bool IsAuthRequired { get; }

        public virtual bool IsAutoAuthorization { get; set; } = false;

        public abstract Task AddAuthenticationHeaderValue(HttpRequestMessage httpRequestMessage);

    }
}
