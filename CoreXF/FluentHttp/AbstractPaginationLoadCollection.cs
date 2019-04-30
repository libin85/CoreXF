
using CoreXF.NavigationAbstraction;
using DLToolkit.Forms.Controls;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CoreXF
{
    public enum PageCollectionMode { Initial, NextPage }

    public abstract class AbstractPaginationLoadCollection<TModel,TRequest> : FlowObservableCollection<TModel>, IDisposable where TRequest : HttpRequestAbstract
    {
        public TRequest Request { get; set; }

        public ReactiveCommand NextPageCommand { get; }
        public ReactiveCommand PullToRefreshCommand { get; }

        public bool IsEndReached { get; set; }
        public bool IsInitialLoadExecutedSuccessful { get; set; }

        protected abstract Task<IEnumerable<TModel>> InternalLoad(PageCollectionMode LoadMode);

        bool _executing;

        DisposableItemsCollection _dispElm;

        public virtual void ProcessCollection(IEnumerable<TModel> collection)
        {
        }

        public AbstractPaginationLoadCollection(TRequest Request, CommonPage CommonPage)
        {
            this.Request = Request;

            DisposableItemsCollection disp = CommonPage?.DisposableItems ?? new DisposableItemsCollection();
            if(CommonPage == null)
            {
                _dispElm = disp;
            }
            else
            {
                disp += this;
            }
            
            disp += NextPageCommand = ReactiveCommand.CreateFromTask<TModel>(async model => await NextPage());

            disp += PullToRefreshCommand = ReactiveCommand.CreateFromTask<TModel>(async model => await InitialLoad());

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                InitialLoad().ConfigureAwait(false);
            }
        }

        bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

            _dispElm?.Dispose();
        }

        public async virtual Task Reload()
        {
            await InitialLoad();
        }

        public async Task InitialLoad()
        {
            if (_executing)
            {
                Debug.WriteLine("DLC: InitialLoad skipped");
                return;
            }

            _executing = true;

            Debug.WriteLine("DLC: InitialLoad");

            try
            {
                Request.CurrentPage = 0;

                IEnumerable<TModel> coll = await InternalLoad(PageCollectionMode.Initial);

                if (coll == null)
                    return;

                this.Repopulate(coll);

                if (coll.Count() > 0)
                {
                    IsInitialLoadExecutedSuccessful = true;
                    IsEndReached = false;
                }
            }
            finally
            {
                _executing = false;
            }

        }

        public async Task NextPage()
        {
            if (!IsInitialLoadExecutedSuccessful)
            {
                Debug.WriteLine("DLC: Initial loading isn't executed successful");
                return;
            }
            if (IsEndReached)
            {
                Debug.WriteLine("DLC: End is reached");
                return;
            }

            if (_executing)
            {
                Debug.WriteLine("DLC: NextPage skipped");
                return;
            }

            _executing = true;

            Debug.WriteLine("DLC: NextPage");

            try
            {
                Request.CurrentPage++;

                IEnumerable<TModel> coll = await InternalLoad(PageCollectionMode.NextPage);
                if (coll == null)
                {
                    return;
                }

                this.AddRange(coll);
                if (coll.Count() == 0)
                {
                    IsEndReached = true;
                }
            }
            finally
            {
                _executing = false;
            }

        }

    }

}
