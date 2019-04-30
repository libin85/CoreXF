using System;

namespace CoreXF
{
    public static class HttpRequestAbstractExtensions
    {
        public static HttpRequestAbstract OnSuccess<T>(this HttpRequestAbstract request, Action<T> callback)
        {
            request.OnSuccess(response =>
            {
                if (response is T result)
                {
                    callback?.Invoke(result);
                }
            });

            return request;
        }
    }
}
