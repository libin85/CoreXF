using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreXF
{
    public static class HttpCache
    {
        public static async Task<string> TryToGetFromCache(string key)
        {
            string func()
            {

                HttpCacheModel model = SystemDB.Connection.Find<HttpCacheModel>(key);
                if (model == null)
                    return null;

                /*
                if (model.Expired != default(DateTimeOffset) && model.Expired < DateTime.Now)
                {
                    // Remove from cache
                    SystemDB.Realm.Write(() => SystemDB.Realm.Remove(model));
                    return null;
                }
                */
                return model.Content;
            };

            if (CoreApp.IsOnMainThread)
            {
                return func();
            }
            else
            {
                return await DeviceHelper.RunOnMainThreadAsync<string>(func);
            }

        }

        public async static Task AddToCache(string key, string value, TimeSpan? ExpirationTime = null)
        {

            void action()
            { 

                HttpCacheModel model = SystemDB.Connection.Find<HttpCacheModel>(key);
                if(model == null)
                {
                    model = new HttpCacheModel { Key = key };
                }
                model.Content = value;
                model.Started = DateTime.Now;

                /*
                if(ExpirationTime != null)
                {
                    model.Expired = DateTime.Now.Add((TimeSpan)ExpirationTime);
                }
                */

                //HttpCacheModel model = DB.Connection.Find<HttpCacheModel>(key);

                SystemDB.Connection.InsertOrReplace(model);
            };

            if (CoreApp.IsOnMainThread)
            {
                action();
            }
            else
            {
                await DeviceHelper.RunOnMainThreadAsync(action);
            }
        }

        public static async Task RemoveFromCache(string key)
        {
            void action ()
            {
                HttpCacheModel model = SystemDB.Connection.Find<HttpCacheModel>(key);
                if (model == null)
                    return;

                SystemDB.Connection.Delete(model);

            };

            if (CoreApp.IsOnMainThread)
            {
                action();
            }
            else
            {
                await DeviceHelper.RunOnMainThreadAsync(action);
            }
        }

        public static async Task<T> TryToGetFromCache<T>(string key) where T : class =>
            await TryToGetFromCache(typeof(T), key) as T;


        public static async Task<object> TryToGetFromCache(Type type,string key)
        {
            string result = await HttpCache.TryToGetFromCache(key);
            if (string.IsNullOrEmpty(result))
                return null;

            try
            {

                return HttpDeserialization.Deserialize(type,result, key);
            }
            catch
            {
                //  the cache is perhaps obsolete and can't be deserialized
                await HttpCache.RemoveFromCache(key);
            }
            return null;
        }

    }
}
