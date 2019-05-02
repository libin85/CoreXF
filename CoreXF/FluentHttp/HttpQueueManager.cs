
using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    [Table(TableName)]
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class HttpQueueItemModel : DBModelAbsrtact
    {
        public const string TableName = "httpqueue";

        [SQLite.PrimaryKey]
        public string Id { get; set; }

        public string GroupId { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }

        public string Assembly { get; set; }
        public string Type { get; set; }
        public string Request { get; set; }

        public int ErrorCount { get; set; }
        public bool Skip { get; set; }
        public string ErrorMessage { get; set; }

        [SQLite.Ignore]
        public HttpRequestAbstract HttpRequest { get; set; }

        public HttpRequestAbstract GetRequestObject()
        {
            HttpRequestAbstract _request = null;
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Assembly))
            {
                Type type = System.Type.GetType($"{Type}, {Assembly}");
                _request = JsonConvert.DeserializeObject(Request, type) as HttpRequestAbstract;
            }
            return _request;
        }

    }

    [Table(TableName)]
    public class HttpSharedDataItemModel : DBModelAbsrtact
    {
        public const string TableName = "httpshareddata";

        public string GroupId { get; set; }

        public string Data { get; set; }
    }

    public class SharedInfoStorage
    {
        //List<HttpSharedDataItemModel> _sharedData { get; set; } = new List<HttpSharedDataItemModel>();

        public void Add(HttpSharedDataItemModel info)
        {
            //_sharedData.Add(info);
            SystemDB.Connection.Insert(info);
        }

        public void Clear()
        {
            //_sharedData.Clear();
            SystemDB.Connection.DeleteAll<HttpSharedDataItemModel>();
        }

        public void Initialize()
        {
            //var sharedData = SystemDB.Connection.Table<HttpSharedDataItemModel>();
            //_sharedData.AddRange(sharedData);
        }

        public void Remove(string GroupId)
        {
            SystemDB.Connection.Table<HttpSharedDataItemModel>().Delete(x => x.GroupId == GroupId);
        }

        public List<T> GetData<T>(string GroupId)
        {
            List<T> result = new List<T>();
            var list = SystemDB.Connection.Table<HttpSharedDataItemModel>()
                .Where(x => x.GroupId == GroupId);
            foreach(var elm in list)
            {
                try
                {
                    T data = JsonConvert.DeserializeObject<T>(elm.Data);
                    result.Add(data);
                }
                catch (Exception ex)
                {
                    ExceptionManager.SendError(ex);
                }

            }

            return result;
        }

    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class HttpQueueManager
    {
        // Bindable properties
        public static HttpQueueManager Current { get; private set; }

        public ObservableCollection<HttpQueueItemModel> Queue { get; private set; }

        public bool IsExecuting { get; set; }

        public string CurrentOperationText { get; set; }

        // Internal 
        CancellationTokenSource cts;

        // For sharing data between http requests
        static SharedInfoStorage _sharedData = new SharedInfoStorage();

        public HttpQueueManager()
        {

            Queue = new ObservableCollection<HttpQueueItemModel>();

            Initialize();

            // Run sync if status of internet connection was changed
            CrossConnectivity.Current.ConnectivityChanged += (sender,e) =>
            {
                if (Queue.Count == 0)
                    return;

                if (e.IsConnected)
                {
                    RunSync().ConfigureAwait(false);
                }
            };

            // Run sync every 60 сек
            Device.StartTimer(TimeSpan.FromMinutes(60), () =>
            {
                if (Queue.Count == 0)
                    return true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    RunSync().ConfigureAwait(false);
                });

                return true;
            });
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            cts.Cancel();
        }

        static HttpQueueManager()
        {
            Current = new HttpQueueManager();
        }

        public void Skip(bool skip, HttpQueueItemModel model)
        {
            model.Skip = skip;
            model.Save();
        }

        bool _isInitialized;
        public void Initialize()
        {
            var httpQuery = SystemDB.Connection.Table<HttpQueueItemModel>().ToList();
            foreach(var elm in httpQuery.OrderBy(x => x.Date))
            {
                Queue.Add(elm);
            }

            _sharedData.Initialize();

            _isInitialized = true;
        }

        public void Add(HttpRequestAbstract HttpRequest, string title)
        {
            var item = new HttpQueueItemModel
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = HttpRequest.GroupId.ToString(),
                Date = DateTime.Now,
                Request = JsonConvert.SerializeObject(HttpRequest),
                Title = title
            };
            Type type = HttpRequest.GetType();
            item.Assembly = type.Assembly.ToString();
            item.Type = type.FullName;
            item.HttpRequest = HttpRequest;
            item.Save();
            Queue.Add(item);
            

            RunSync().ConfigureAwait(false);
        }

        public void ClearQueue()
        {
            foreach (var elm in Queue)
                Delete(elm);
        }

        public void Delete(HttpQueueItemModel model)
        {

            // if it's the last query try to delete ALL shared data
            if(Queue.Count == 1)
            {
                SystemDB.Connection.DeleteAll<HttpSharedDataItemModel>();
                _sharedData.Clear();
            }

            Queue.Remove(model);
            model.Delete();
        }

        bool _executing;
        public async Task RunSync()
        {
            if (_executing)
                return;

            int skippingItemsCount = 0;

            try
            {

                CurrentOperationText = "Синхронизация...";

                if (!CrossConnectivity.Current.IsConnected)
                {
                    CurrentOperationText = "Отсутствует интернет соединение";
                    return;
                }

                _executing = true;
                IsExecuting = true;
                cts = new CancellationTokenSource();

                
                HttpQueueItemModel currentItem;

                while ((currentItem = GetNext()) != null)
                {
                    try
                    {
                        HttpRequestAbstract request = currentItem.HttpRequest;
                        if (request == null)
                        {
                            request = currentItem.GetRequestObject();
                        }
                        await request
                            .SetCancellationTokenSource(cts)
                            .Evaluate();

                        Delete(currentItem);
                    }
                    catch (Exception ex)
                    {

                        bool skipThisRequest = false;
                        if(ex as HttpStatusCodeException != null)
                        {
                            HttpStatusCodeException htce = ex as HttpStatusCodeException;
                            if (htce.Code == HttpStatusCode.BadRequest)
                            {
                                skipThisRequest = true;
                            }
                        }

                        currentItem.ErrorCount++;
                        currentItem.ErrorMessage = "Ошибка отправки";// ex.Message;
                        currentItem.Skip = skipThisRequest;
                        currentItem.Save();

                        if (!skipThisRequest)
                        {
                            CurrentOperationText = "Ошибка синхронизации";// :" + ex.Message;

                            ExceptionManager.SendError(ex);
                            return;
                        }

                    }

                    if (cts.IsCancellationRequested)
                    {
                        CurrentOperationText = "Синхронизация отменена";
                        break;
                    }
                }
                CurrentOperationText = "";
            }
            finally
            { 
                cts.Dispose();
                IsExecuting = false;
                _executing = false;
            }

            HttpQueueItemModel GetNext()
            {
                if (Queue.Count == skippingItemsCount)
                    return null;

                HttpQueueItemModel item = Queue[skippingItemsCount];
                if (item.Skip)
                {
                    skippingItemsCount++;
                    return GetNext();
                }

                return item;
            }
        }

        public static void AddSharedData<T>(Guid GroupId, T Data) => AddSharedData<T>(GroupId.ToString(), Data);
        public static void AddSharedData<T>(string GroudId,T Data)  
        {
            HttpSharedDataItemModel model = new HttpSharedDataItemModel
            {
                GroupId = GroudId,
                Data = JsonConvert.SerializeObject(Data)
            };
            _sharedData.Add(model);
        }

        public static List<T> GetSharedData<T>(Guid GroupId) => _sharedData.GetData<T>(GroupId.ToString());
        public static List<T> GetSharedData<T>(string GroupId) => _sharedData.GetData<T>(GroupId);

    }
}
