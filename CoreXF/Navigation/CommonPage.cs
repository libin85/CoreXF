
using Plugin.Connectivity.Abstractions;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF.NavigationAbstraction
{

    public abstract class CommonPage : ContentPage, IAppLifeCycle, IPageLifeCycle
    {
        public IMasterDetailController MasterDetailController { get; set; }

        public Command BackCommand { get; set; }

        public bool IsBackNavigation { get; set; }

        public DisposableItemsCollection DisposableItems = new DisposableItemsCollection();
        static List<IDisposable> DelayedDisposableItems = new List<IDisposable>();

        public bool IsConnected { get; set; }
        IConnectivity _connectivity;
        
        public virtual void OnConnectivityChanged(bool isConnected)
        {
        }

        public CommonPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackCommand = new Command(async () => {
                OnBackCommand();
                await CoreApp.NavigationService.NavigateBackAsync();

            });

            _connectivity = Locator.CurrentMutable.GetService<IConnectivity>();
            IsConnected = _connectivity.IsConnected;
            _connectivity.ConnectivityChanged += _connectivity_ConnectivityChanged;

        }

        void _connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.IsConnected;
            OnConnectivityChanged(e.IsConnected);
        }

        public virtual void Initialize()
        {
        }

        public virtual void OnFirstAppearing()
        {
        }

        public virtual void OnBackCommand()
        {
        }

        bool _firstAppearing = true;
        protected override void OnAppearing()
        {
            if (_firstAppearing)
            {
                OnFirstAppearing();
                _firstAppearing = false;
            }
            base.OnAppearing();
        }

        public virtual void OnAppResume()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                ForceLayout();
            }
        }

        public virtual void OnDestroyPage()
        {
            _connectivity.ConnectivityChanged -= _connectivity_ConnectivityChanged;

            CleanupResourses().ConfigureAwait(false);
        }


        async Task CleanupResourses()
        {
            await Task.Delay(200);

            UnapplyBindings();

            Content?.DisposeAllViews();

            // Dispose delayed objects
            for (int i = 0; i < DelayedDisposableItems.Count; i++)
            {
                DelayedDisposableItems[i]?.Dispose();
            }
            DelayedDisposableItems.Clear();

            // Dispose objects
            for (int i = 0; i < (DisposableItems?.Items?.Count ?? 0); i++)
            {
                IDisposable obj = DisposableItems.Items[i];
                if (obj == null)
                    return;

                switch (obj)
                {
                    case ReactiveUI.ReactiveCommand rc:
                        DelayedDisposableItems.Add(obj);
                        break;

                    default:
                        obj.Dispose();
                        break;
                }
            }
            DisposableItems?.Items?.Clear();
        }
    }
}
