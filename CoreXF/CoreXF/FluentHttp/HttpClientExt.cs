
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace CoreXF
{
    //public enum HttpReqRes { Ok, Executing, Error, NoConnection, LogoutRequired }

    public class ExceptionTokenExpired : Exception
    {
        public ExceptionTokenExpired() : base("GetAsync token is null") { }
    }


    public class HttpClientExt
    {
        const string prefix = "HTC:";

#if DEBUG
        bool _timing = true;
        DateTime _startTime;
#endif

        public CancellationToken CancellationToken;

        public HttpRequestAbstract HttpRequest { get; set; }

        static HttpClient _client;

        public bool CheckIsAppRunning { get; set; }

        public string Parameters => param?.ToString();
        StringBuilder param;

        static HttpClientExt()
        {
            _client = new HttpClient();

            //_client.DefaultRequestHeaders.Add("Accept",         "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            //_client.DefaultRequestHeaders.Add("Content-Type",   "application/json; charset=utf-8");
            //_client.DefaultRequestHeaders.Add("Accept-Language","ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");

        }

        public HttpClientExt(bool checkIsAppRunning = true)
        {
            CheckIsAppRunning = checkIsAppRunning;
        }

        public void ClearParameters()
        {
            param?.Clear();
        }

        public HttpClientExt AddParameter<T>(T value, string ignoreFields = null) where T : class
        {
            var ignoreList = ignoreFields?.Split(',');

            foreach (var prop in typeof(T).GetRuntimeProperties())
            {
                if (ignoreList != null && ignoreList.Contains(prop.Name))
                    continue;

                string val = prop.GetValue(value)?.ToString();
                if (string.IsNullOrEmpty(val)) continue;
                AddParameter(prop.Name, val);
            }
            return this;
        }

        public HttpClientExt AddParameter(string name, long value) => AddParameter(name, value.ToString());
        public HttpClientExt AddParameter(string name, DateTime datetime) => AddParameter(name, datetime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
        public HttpClientExt AddParameter(string name, DateTime datetime, bool OnlyDate) => AddParameter(name, datetime.ToString("yyyy'-'MM'-'dd"));
        public HttpClientExt AddParameter(string name, DateTime? datetime) => datetime == null ? this : AddParameter(name, ((DateTime)datetime));
        public HttpClientExt AddParameter(string name, bool value) => AddParameter(name, value.ToString());
        public HttpClientExt AddParameter(string name, int value) => AddParameter(name, value.ToString());
        public HttpClientExt AddParameter(string name, int? value) => value == null ? this : AddParameter(name, ((int)value).ToString());

        public HttpClientExt AddParameterIfNotNull(string name, int? value) => value == null ? this : AddParameter(name, (int)value);
        public HttpClientExt AddParameterIfNotNull(string name, long? value) => value == null ? this : AddParameter(name, (long)value);
        public HttpClientExt AddParameterIfNotNull(string name, string value) => string.IsNullOrEmpty(value) ? this : AddParameter(name, value);
        public HttpClientExt AddParameterIfNotNull(string name, DateTime value) => value == default(DateTime) ? this : AddParameter(name, value);
        public HttpClientExt AddParameterIfNotNull(string name, DateTime? value) => value == null ? this : AddParameter(name, value);

        public HttpClientExt AddParameterIfNotZero(string name, int value) => value <= 0 ? this : AddParameter(name, value);

        bool _thisIsAuthorizationRequest;
        public HttpClientExt ThisIsAuthorizationRequest()
        {
            _thisIsAuthorizationRequest = true;
            return this;
        }


        public HttpClientExt AddParameter(string name, string value)
        {
            if (param == null)
                param = new StringBuilder(200);

            if (param.Length > 0)
                param.Append("&");

            param.Append(WebUtility.UrlEncode(name));
            param.Append("=");
            param.Append(WebUtility.UrlEncode(value));

            return this;
        }

        public string BuildUrl(APIServiceAbstract api, string method)
        {
            string resultUrl = null;
            if (api == null)
            {
                resultUrl = method;
            }
            else if (string.IsNullOrEmpty(method))
            {
                resultUrl = api.Url;
            }
            else
            {
                resultUrl = UriHelper.Combine(api.Url, method);
            }

            if (param != null)
                resultUrl = resultUrl + "?" + param.ToString();

            return resultUrl;
        }

        public string BuildShortUrl(string method) => param == null ? method : method + "?" + param.ToString();

        #region File transfer

        public enum FileType { Jpeg }


        public async Task<HttpResponseMessage> PostFileAsyncRaw(string Url, string filename, FileType fileType, APIServiceAbstract Api = null)
        {

            string shortFileName = Path.GetFileName(filename);
            string shortFileNameWithoutExt = Path.GetFileNameWithoutExtension(filename);


            using (Stream stream = File.OpenRead(filename))
            using (StreamContent fcontent = new StreamContent(stream))
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                fcontent.Headers.Add("Content-Disposition", $"form-data; name=\"{shortFileNameWithoutExt}\"; filename=\"{shortFileName}\"");
                switch (fileType)
                {
                    case FileType.Jpeg:
                        fcontent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                        break;
                }

                content.Add(fcontent, shortFileNameWithoutExt, shortFileName);

                string fullUrl_ = BuildUrl(Api, Url);
                var responceMessage = await SendAsync(HttpMethod.Post, fullUrl_, Api, content: content);

                return responceMessage;
            }
        }


        public async Task<TResult> PostFileAsync<TResult>(string Url, string filename, FileType fileType, APIServiceAbstract Api = null)
        {
            using (var resp = await PostFileAsyncRaw(Url, filename, fileType, Api))
            {

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    TResult result = await HttpDeserialization.Deserialize<TResult>(resp, Url);
                    return result;
                }
                else
                {
                    throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Post {BuildShortUrl(Url)} Code {resp.StatusCode}");
                }
            }
        }


        public async Task PostFileAsync(string Url, string filename, FileType fileType, APIServiceAbstract Api = null)
        {
            using (var resp = await PostFileAsyncRaw(Url, filename, fileType, Api))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                    return;
                else
                    throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Post {BuildShortUrl(Url)} Code {resp.StatusCode}");
            }
        }

        #endregion

        #region Multipart

        public async Task<HttpResponseMessage> PostMultipartRawAsync(string Url, string filePath, string headerName, string contentType, APIServiceAbstract Api = null)
        {
            using (Stream stream = File.OpenRead(filePath))
            using (StreamContent fcontent = new StreamContent(stream))
            {
                // TODO: Fix disposal
                var formData = new MultipartFormDataContent("----WebKitFormBoundary7MA4YWxkTrZu0gW");

                // fcontent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data");

                fcontent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                fcontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = headerName,
                    FileName = Path.GetFileName(filePath)
                };

                formData.Add(fcontent);

                var fullUrl = BuildUrl(Api, Url);
                var response = await SendAsync(HttpMethod.Post, fullUrl, Api, content: formData);

                return response;
            }
        }

        public async Task<TResult> PostMultipartAsync<TResult>(string Url, string filePath, string headerName, APIServiceAbstract Api = null)
        {
            using (var resp = await PostMultipartRawAsync(Url, filePath, headerName, "multipart/form-data", Api))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    TResult result = await HttpDeserialization.Deserialize<TResult>(resp, Url);
                    return result;
                }

                throw new HttpStatusCodeException(resp.StatusCode, $"HTTP PostMultipartAsync {BuildShortUrl(Url)} Code {resp.StatusCode}");
            }
        }


        #endregion

        public async Task<TResponse> PostAsync<TBody, TResponse>(string Url, TBody value, APIServiceAbstract Api = null, HttpMethod method = null) where TBody : class
        {
            string str = HttpDeserialization.Serialize(value, Url);
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");

            string fullurl = BuildUrl(Api, Url);
            TResponse result = default(TResponse);
            using (var message = await SendAsync(method ?? HttpMethod.Post, fullurl, Api, stringContent))
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    throw new HttpStatusCodeException(message.StatusCode);

                result = await HttpDeserialization.Deserialize<TResponse>(message, Url);
            }
            return result;

        }


        public async Task<HttpResponseMessage> PostAsync<T>(string Url, T value, APIServiceAbstract Api = null, HttpMethod method = null) where T : class
        {
            string str = HttpDeserialization.Serialize(value, Url);
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");

            string fullurl = BuildUrl(Api, Url);
            var res = await SendAsync(method ?? HttpMethod.Post, fullurl, Api, stringContent);
            return res;

        }

        public async Task<HttpResponseMessage> PostAsync(string Url, APIServiceAbstract Api = null, HttpContent content = null, HttpMethod method = null)
        {
            HttpContent content_ = content ?? new StringContent("");

            string fullUrl_ = BuildUrl(Api, Url);
            using (var resp = await SendAsync(method ?? HttpMethod.Post, fullUrl_, Api, content_))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    return resp;
                }
                else
                {
                    throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Post {BuildShortUrl(Url)} Code {resp.StatusCode}");
                }
            }
        }

        public async Task<HttpResponseMessage> LoginAsync(string Url, string Login, string Password)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, Url))
            {

                var byteArray = new UTF8Encoding().GetBytes($"{Login}:{Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var formData = new List<KeyValuePair<string, string>>();
                formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                formData.Add(new KeyValuePair<string, string>("username", Login));
                formData.Add(new KeyValuePair<string, string>("password", Password));

                request.Content = new FormUrlEncodedContent(formData);
                var response = await client.SendAsync(request, CancellationToken);
                return response;
            }

        }

        public async Task<HttpResponseMessage> DeleteAsync(string Url, APIServiceAbstract Api = null, HttpContent content = null, string accessToken = null)
        {
            string fullurl = BuildUrl(Api, Url);

            using (var resp = await SendAsync(HttpMethod.Delete, fullurl, Api))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    return resp;
                }

                throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Delete {BuildShortUrl(Url)} Code {resp.StatusCode}");
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(string Url, T value, APIServiceAbstract Api = null, HttpMethod method = null) where T : class
        {
            string str = HttpDeserialization.Serialize(value, Url);
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");

            string fullurl = BuildUrl(Api, Url);
            var res = await SendAsync(method ?? HttpMethod.Delete, fullurl, Api, stringContent);
            return res;

        }

        public async Task<HttpResponseMessage> PutAsync<T>(string Url, T value, APIServiceAbstract Api = null, HttpMethod method = null) where T : class
        {
            string str = HttpDeserialization.Serialize(value, Url);
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");

            string fullurl = BuildUrl(Api, Url);
            var res = await SendAsync(method ?? HttpMethod.Put, fullurl, Api, stringContent);
            return res;
        }

        public async Task<TResponse> PutAsync<TBody, TResponse>(string Url, TBody value, APIServiceAbstract Api = null, HttpMethod method = null) where TBody : class
        {
            string str = HttpDeserialization.Serialize(value, Url);
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");

            string fullurl = BuildUrl(Api, Url);
            TResponse result = default(TResponse);
            using (var message = await SendAsync(method ?? HttpMethod.Put, fullurl, Api, stringContent))
            {
                if (message.StatusCode != HttpStatusCode.OK)
                    throw new HttpStatusCodeException(message.StatusCode);

                result = await HttpDeserialization.Deserialize<TResponse>(message, Url);
            }
            return result;

        }

        public async Task<T> PostAsync<T>(string Url, APIServiceAbstract Api = null, HttpContent content = null, HttpMethod method = null)
        {
            HttpContent content_ = content ?? new StringContent("");

            string fullUrl_ = BuildUrl(Api, Url);

            using (var resp = await SendAsync(method ?? HttpMethod.Post, fullUrl_, Api, content_))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    T val = await HttpDeserialization.Deserialize<T>(resp, fullUrl_);
                    return val;
                }
                else
                {
                    throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Post {BuildShortUrl(Url)} Code {resp.StatusCode}");
                }
            }
        }

        /*
        void checkIsAppRunning()
        {
            if(!CoreApp.AppIsRunning && CheckIsAppRunning)
            {
                throw new OperationCanceledException("App is not running. Http request's been cancelled.");
            }
        }
             */

        async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, APIServiceAbstract Api, HttpContent content = null)
        {
            Debug.WriteLine($"{prefix} {method} {url}");

            using (var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(url),
                Content = content
            })
            {
                if (Api != null && Api.IsAuthRequired && !_thisIsAuthorizationRequest)
                {
                    await Api.AddAuthenticationHeaderValue(requestMessage);
                    // requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Api.Scheme, Api.Token);
                }

                //_startTime = DateTime.Now;

                HttpResponseMessage httpResponse = await _client.SendAsync(requestMessage, CancellationToken);

                if (HttpRequest != null)
                {
                    HttpRequest.HttpStatusCode = httpResponse.StatusCode;
                }

                //Debug.WriteLine($"Timing: {api_method}  time {(DateTime.Now - _startTime).ToString()}");

                return httpResponse;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string Url, APIServiceAbstract Api = null)
        {
            string fullUrl_ = BuildUrl(Api, Url);

            HttpResponseMessage result = await SendAsync(HttpMethod.Get, fullUrl_, Api);

            return result;
        }

        public async Task<T> GetAsync<T>(string Url, APIServiceAbstract Api = null) where T : class
        {
            string fullUrl = BuildUrl(Api, Url);

            using (var resp = await GetAsync(Url, Api))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpStatusCodeException(resp.StatusCode, $"HTTP Get {BuildShortUrl(Url)} Code {resp.StatusCode}");
                }

                T value = await HttpDeserialization.Deserialize<T>(resp, BuildUrl(Api, Url));
                return value;

            }
        }

    }
}
