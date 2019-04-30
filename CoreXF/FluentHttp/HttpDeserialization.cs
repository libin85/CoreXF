using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreXF
{

    public class HttpDeserializationException : Exception
    {
        public HttpDeserializationException(string message) : base(message)
        {
        }
    }

    public class HttpSerializationException : Exception
    {
        public HttpSerializationException(string message) : base(message)
        {

        }
    }

    public static class HttpDeserialization
    {
        static JsonSerializerSettings _JsonSerializeSettings = new JsonSerializerSettings
        {
            //DateFormatString = "'yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"
            DateFormatString = "yyyy-MM-dd'T'HH:mm:ss",
            Formatting = Formatting.Indented
        };

        public async static Task<T> Deserialize<T>(HttpResponseMessage response, string url)
        {
            if (response.Content == null)
                throw new HttpDeserializationException($"No content for {url}");

            T val = default(T);
// TODO Need to release why second variant doesnt' work
//#if DEBUG
            string res = await response.Content.ReadAsStringAsync();
            val = Deserialize<T>(res, url);
//#else
//            using (var stream = await response.Content.ReadAsStreamAsync())
//            {
//                val = Deserialize<T>(stream, url);
//            }
//#endif
            return val;
        }

        public static T Deserialize<T>(Stream content, string url)
            => (T)Deserialize(typeof(T), content, url);

        public static object Deserialize(Type type, Stream stream, string url)
        {
            try
            {
                using (var streamreader = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(streamreader))
                {
                    return new JsonSerializer().Deserialize(jsonTextReader,type);
                }
            }
            catch (Exception ex)
            {
                throw new HttpDeserializationException($"Deserializatrion error {url} <stream> msg {ex.Message}");
            }
        }

        public static T Deserialize<T>(string content, string url) 
            => (T)Deserialize(typeof(T), content, url);

        public static object Deserialize(Type type, string content, string url)
        {
            if(type == typeof(string))
            {
                return content;
            }
                
            object rvalue = null;
            try
            {
                rvalue = JsonConvert.DeserializeObject(content,type);
            }
            catch (Exception ex)
            {
                throw new HttpDeserializationException($"Deserializatrion error {url} {content} msg {ex.Message}");
            }
            return rvalue;
        }

        public static string Serialize<T>(T obj, string url) where T : class
            => Serialize(typeof(T), obj, url);

        public static string Serialize(Type type,object obj,string url)
        {
            if((obj as string) != null)
            {
                return obj as string;
            }

            string res = null;
            try
            {
                res = JsonConvert.SerializeObject(obj, _JsonSerializeSettings);
            }
            catch(Exception ex)
            {
                throw new HttpSerializationException($"Serialization error {url} obj {obj} msg {ex.Message}");
            }
            return res;
        }
    }
}
