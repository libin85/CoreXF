
using Acr.UserDialogs;
using Plugin.Connectivity.Abstractions;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{

    public class CacheParameters
    {
        public TimeSpan ExpirationTime { get; set; }
        public string Key { get; set; }
    }

    public class HttpRequest : HttpRequestAbstract
    {
        public HttpRequest(Func<HttpClientExt, Task<object>> request) : base()
        {
            _request = request;
        }

        public HttpRequest(Task<object> request)
        {
            AddRequestTaskToQueue(request);
        }

        public HttpRequest()
        {
        }

        public void AddRequestTaskToQueue(Task<object> request)
        {
            if(Requests == null)
            {
                Requests = new List<Task<object>>();
            }

            Requests.Add(request);
        }

        public override Task<object> RunRequest(HttpClientExt httpClient)
        {
            throw new NotImplementedException();
        }
    }

    //[PropertyChanged.DoNotNotifyAttribute]
    public abstract class HttpRequestAbstract : ObservableObject
    {

        public HttpStatusCode HttpStatusCode { get; set; }

        public Guid GroupId { get; set; }

        public const int NotDefined = -1;

        public int CurrentPage { get; set; } = NotDefined;
        public int PageSize { get; set; } = Device.Idiom == TargetIdiom.Phone ? 25 : 50;

        [PropertyChanged.DoNotNotify]
        public bool IsExecuting {
            get => _IsExecuting;
            set {
                _IsExecuting = value;
                if (CoreApp.IsOnMainThread)
                {
                    RaisePropertyChanged(nameof(IsExecuting));
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => RaisePropertyChanged(nameof(IsExecuting)));
                }
                //Debug.WriteLine($"HttpRequest IsExecute changed {_IsExecuted}");
            }
        }
        bool _IsExecuting;

        static Lazy<IConnectivity> _connectivity = new Lazy<IConnectivity>(() => Locator.CurrentMutable.GetService<IConnectivity>());
        static Lazy<IUserDialogs> _dialogs = new Lazy<IUserDialogs>(()=> Locator.CurrentMutable.GetService<IUserDialogs>());

        // Caching
        /*
        public CacheParameters CacheParam;
        public HttpRequestAbstract Cache(TimeSpan ExpirationTime,string Key = null)
        {
            CacheParam = new CacheParameters();
            if (string.IsNullOrEmpty(Key))
            {
                CacheParam.Key = this.GetType().ToString();
            }
            else
            {
                CacheParam.Key = Key;
            }
            CacheParam.ExpirationTime = ExpirationTime;
            return this;
        }
        */

        // Take previous result
        #region Take previous result

        // Take a previous result before execute request
        bool _takePreviousResultOption;
        Action<object> __takePreviousResultApplyResult;
        bool _IfThereIsValueInCacheDoNotExectuteHttpRequest;
        public HttpRequestAbstract TakePreviousResultAndRunRequest<TResult>(string key = null, Action<object> applyResult = null, bool ifThereIsValueInCacheDoNotExectuteHttpRequest = false)
        {
            _IfThereIsValueInCacheDoNotExectuteHttpRequest = ifThereIsValueInCacheDoNotExectuteHttpRequest;
            _takePreviousResultOption = true;
            __takePreviousResultApplyResult = applyResult;
            _cacheKey = key ?? GetDefaultCacheKey();
            _cacheResultType = typeof(TResult);
            return this;
        }

        #endregion

        // Progress 
        string _progressMessage;
        bool _showProgressWhenExecuting;
        bool _showCancel;
        public HttpRequestAbstract ShowProgress(string Message = "", bool showCancel = true)
        {
            _showProgressWhenExecuting = true;
            _progressMessage = Message;
            _showCancel = showCancel;
            return this;
        }

        // Put in queue
        public void PutInQueue(string title, Guid groupId = default(Guid))
        {
            GroupId = groupId;
            HttpQueueManager.Current.Add(this, title);
        }

        // Use cache
        bool _useCache;
        string _cacheKey;
        Type _cacheResultType;
        public HttpRequestAbstract UseCache<TResult>(string key = null,bool useCache = true)
        {
            _useCache = useCache;
            _cacheResultType = typeof(TResult);
            _cacheKey = key ?? GetDefaultCacheKey();
            _useCache = true;
            return this;
        }

        string GetDefaultCacheKey()
        {
            string name = this.GetType().Name;
            if(name == nameof(HttpRequest))
            {
                throw new Exception("The cache key have to be set for anonymous requests");
            }
            return name;
        }

        // Exceptions
        #region OnException

        bool _catchException;
        string _exceptionMessage;

        Action<Exception> _OnException;
        Func<Exception, Task> _OnExceptionAsync;
        public HttpRequestAbstract CatchException(string Message = null, Action<Exception> OnException = null, Func<Exception, Task> OnExceptionAsync = null)
        {
            _catchException = true;
            _exceptionMessage = Message ?? Tx.T("CommonMessages_UnknownError");
            _OnException = OnException;
            _OnExceptionAsync = OnExceptionAsync;
            return this;
        }

        async Task OnExceptionCalls(Exception ex)
        {
            if (!string.IsNullOrEmpty(_exceptionMessage))
            {
                await _dialogs.Value.AlertAsync(_exceptionMessage);
            }
            // Manual
            if (_OnExceptionAsync != null)
            {
                await _OnExceptionAsync(ex);
            }
            _OnException?.Invoke(ex);
        }

        #endregion

        // Finnaly
        Action _onFinnaly;
        public HttpRequestAbstract Finnaly(Action OnFinnaly)
        {
            _onFinnaly = OnFinnaly;
            return this;
        }

        // Default settings
        #region Default settings

        public static Action<HttpRequestAbstract> DefaultSettings { get; set; }

        public HttpRequestAbstract ApplyDefaultSettings()
        {
            DefaultSettings?.Invoke(this);
            return this;
        }

        #endregion

        // OnSuccess
        #region OnSuccess

        Func<Task> _OnSuccessAsync;
        Action _OnSuccess;
        public HttpRequestAbstract OnSuccess(Func<Task> OnSuccess)
        {
            _OnSuccessAsync = OnSuccess;
            return this;
        }
        public HttpRequestAbstract OnSuccess(Action OnSuccess)
        {
            _OnSuccess = OnSuccess;
            return this;
        }

        Func<object,Task> _OnSuccessWithResultAsync;
        public HttpRequestAbstract OnSuccess(Func<object,Task> OnSuccess)
        {
            _OnSuccessWithResultAsync = OnSuccess;
            return this;
        }
        Action<object> _OnSuccessWithResult;
        public HttpRequestAbstract OnSuccess(Action<object> OnSuccess)
        {
            _OnSuccessWithResult = OnSuccess;
            return this;
        }

        async Task OnSuccessCalls(object result)
        {
            // Manual
            if (_OnSuccessAsync != null)
            {
                await _OnSuccessAsync();
            }
            _OnSuccess?.Invoke();

            if (_OnSuccessWithResultAsync != null)
            {
                await _OnSuccessWithResultAsync(result);
            }
            _OnSuccessWithResult?.Invoke(result);
        }

        #endregion


        // OnCancel
        #region OnCancel

        Func<Task> _OnCancelAsync;
        Action _OnCancel;
        public HttpRequestAbstract OnCancel(Func<Task> OnCancel)
        {
            _OnCancelAsync = OnCancel;
            return this;
        }
        public HttpRequestAbstract OnCancel(Action OnCancel)
        {
            _OnCancel = OnCancel;
            return this;
        }

        async Task OnCancelCalls()
        {
            if (_OnCancelAsync != null)
            {
                await _OnCancelAsync();
            }
            _OnCancel?.Invoke();
        }

        #endregion

        public HttpRequestAbstract()
        {
            //HttpRequestStatus = HttpReqRes.Ok;
        }

        // Lambda request
        protected Func<HttpClientExt, Task<object>> _request;
        public List<Task<object>> Requests;

        // Deduplication
        #region Deduplication

        bool _deduplicate;
        static List<string> _deduplicateList = new List<string>();
        string _deduplicate_key;
        public HttpRequestAbstract Deduplicate(string key = null)
        {
            _deduplicate = true;
            _deduplicate_key = key ?? this.GetType().Name;
            return this;
        }

        #endregion
      
        // On connection
        #region OnConnection

        bool _onConnectionErrorFlag;

        Action _onConnectionError;
        string _onConnectionErrorMessage;
        public HttpRequestAbstract OnConnectionError(string Message = null, Action onConnectionError = null)
        {
            _onConnectionErrorFlag = true;
            _onConnectionErrorMessage = Message;
            _onConnectionError = onConnectionError;
            return this;
        }

        Func<Task> _onConnectionErrorAsync;
        public HttpRequestAbstract OnConnectionErrorAsync(string Message = null, Func<Task> onConnectionErrorAsync = null)
        {
            _onConnectionErrorMessage = Message;
            _onConnectionErrorFlag = true;
            _onConnectionErrorAsync = onConnectionErrorAsync;
            return this;
        }

        async Task OnConnectionErrorCalls()
        {
            if (!string.IsNullOrEmpty(_onConnectionErrorMessage))
            {
                await _dialogs.Value.AlertAsync(_onConnectionErrorMessage);
            }

            // Manual
            if (_onConnectionErrorAsync != null)
            {
                await _onConnectionErrorAsync();
            }
            _onConnectionError?.Invoke();
        }
        #endregion


        // on server error
        #region OnServerError

        bool _onServerErrorFlag;

        Action _onServerError;
        string _onServerErrorMessage;
        public HttpRequestAbstract OnServerError(string Message = null, Action onServerError = null)
        {
            _onServerErrorFlag = true;
            _onServerErrorMessage = Message;
            _onServerError = onServerError;
            return this;
        }

        Func<Task> _onServerErrorAsync;
        public HttpRequestAbstract OnServerErrorAsync(string Message = null, Func<Task> onServerErrorAsync = null)
        {
            _onServerErrorFlag = true;
            _onServerErrorMessage = Message;
            _onServerErrorAsync = onServerErrorAsync;
            return this;
        }

        async Task OnServerErrorCalls()
        {
            if (!string.IsNullOrEmpty(_onServerErrorMessage))
            {
                await _dialogs.Value.AlertAsync(_onServerErrorMessage);
            }

            // Manual
            if (_onServerErrorAsync != null)
            {
                await _onServerErrorAsync();
            }
            _onServerError?.Invoke();
        }

        #endregion

        // Retry
        bool _retry;
        TimeSpan _retryInterval;
        int _retryCount;
        Action _retryOnRetry;
        public HttpRequestAbstract Retry(int RetryCount = 3,TimeSpan RetryInterval = default(TimeSpan), Action OnRetry = null)
        {
            _retry = true;
            _retryInterval = RetryInterval == default(TimeSpan) ? TimeSpan.FromSeconds(1) : RetryInterval;
            _retryCount = RetryCount;
            _retryOnRetry = OnRetry;
            return this;
        }

        CancellationTokenSource _cts_external;
        CancellationTokenSource _cts_internal;
        public HttpRequestAbstract SetCancellationTokenSource(CancellationTokenSource cts)
        {
            _cts_external = cts;
            return this;
        }

        public abstract Task<object> RunRequest(HttpClientExt httpClient);

        async Task<object> runRequest(CancellationTokenSource cts)
        {
            HttpClientExt httpClient = new HttpClientExt();
            httpClient.HttpRequest = this;

            if (cts != null)
            {
                httpClient.CancellationToken = cts.Token;
            }
            if((Requests?.Count ?? 0) > 0)
            {
                await Task.WhenAll(Requests.ToArray());
                return null;
            }
            else if (_request != null)
            {
                var t1 = await Task.Run<object>(async () => await _request(httpClient));
                return t1;
            }
            else
            {
                var exceptions = new List<Exception>();
                Exception lastException = null;
                for (var retry = -1; retry < _retryCount; retry++)
                {
                    try
                    {
                        return await RunRequest(httpClient);//.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                            
                        if (!ExceptionManager.IsConnectionProblem(ex))
                            throw;

                        lastException = ex;

                        _retryOnRetry?.Invoke();
                        exceptions.Add(ex);
                    }

                    await Task.Delay(_retryInterval);//.ConfigureAwait(false);
                }

                throw lastException;
            }
        }

        public void Cancel()
        {
            _cts_external?.Cancel();
            _cts_internal?.Cancel();
        }

        IProgressDialog _progress = null;
        public void HideProgressActivity()
        {
            if (_progress != null)
            {
                _progress.Hide();
                _progress.Dispose();
                _progress = null;
            }
        }

        public void FireAndForget() => Evaluate().ConfigureAwait(false);
        public async Task<object> Evaluate()
        {
          
            if (_cts_external == null)
                _cts_internal = new CancellationTokenSource();

            // Deduplication
            if (_deduplicate)
            {
                if (_deduplicateList.Contains(_deduplicate_key))
                {
                    return null;
                }
                else
                {
                    _deduplicateList.Add(_deduplicate_key);
                }
            }

            try
            {

                // Check connection
                bool hereIsConnectProblem = false;
                if (!_connectivity?.Value?.IsConnected ?? false)
                {
                    await OnConnectionErrorCalls();
                    hereIsConnectProblem = true;
                }

                // Show progress
                if (_showProgressWhenExecuting && !hereIsConnectProblem)
                {
                    if (!_showCancel)
                    {
                        _progress = _dialogs.Value.Loading(_progressMessage);
                    }
                    else
                    {
                        _progress = _dialogs.Value.Loading(
                            title: _progressMessage,
                            cancelText: Tx.T("Dialogs_Cancel"),
                            onCancel: async () =>
                            {
                                _cts_external?.Cancel(throwOnFirstException: true);
                                _cts_internal?.Cancel(throwOnFirstException: true);

                                await OnCancelCalls();
                            });
                    }
                }

                IsExecuting = true;

                // Take previous result 
                object objectFromCache = null;
                bool thereIsObjectInCacheAndDontNeedExecuteHttp = false;

                if (_takePreviousResultOption || (_useCache && hereIsConnectProblem ))
                {
                    objectFromCache = await HttpCache.TryToGetFromCache(_cacheResultType, _cacheKey);
                    if(objectFromCache != null)  
                    {

                        __takePreviousResultApplyResult?.Invoke(objectFromCache);

                        thereIsObjectInCacheAndDontNeedExecuteHttp = _IfThereIsValueInCacheDoNotExectuteHttpRequest;
                    }

                }

                object result = null;
                if (!thereIsObjectInCacheAndDontNeedExecuteHttp && !hereIsConnectProblem)
                {

                    result = await runRequest(_cts_external ?? _cts_internal);
                    
                    // Take previous result 
                    if (_takePreviousResultOption || _useCache)
                    {
                        __takePreviousResultApplyResult?.Invoke(result);
                        bool needToSaveToCache = true;

                        if (needToSaveToCache)
                        {
                            string serial = HttpDeserialization.Serialize(result, "");
                            await HttpCache.AddToCache(_cacheKey, serial);
                        }
                    }
                }
                else
                {
                    result = objectFromCache;
                }

                await OnSuccessCalls(result);

                return result;
            }
            catch (Exception ex)
            {
                HideProgressActivity();

                //await Task.Delay(50);

                HttpStatusCodeException hsce = ex as HttpStatusCodeException;

                if (hsce?.Code == HttpStatusCode.InternalServerError)
                {
                    await OnServerErrorCalls();
                }
                else if(ExceptionManager.IsConnectionProblem(ex))
                {
                    await OnConnectionErrorCalls();
                }
                else
                {
                    await OnExceptionCalls(ex);
                }

                ExceptionManager.SendError(ex);

                

                if (!_catchException)
                {
                    throw;
                }

                if (_useCache)
                {
                    object result = await HttpCache.TryToGetFromCache(_cacheResultType, _cacheKey);
                    return result;
                }

                return null;
            }
            finally
            {
                IsExecuting = false;

                 _onFinnaly?.Invoke();


                HideProgressActivity();

                if (_cts_internal != null)
                {
                    _cts_internal.Dispose();
                    _cts_internal = null;
                }

                if (_deduplicate && _deduplicateList.Contains(_deduplicate_key))
                {
                    _deduplicateList.Remove(_deduplicate_key);
                }
            }
        }
    }
    
}
