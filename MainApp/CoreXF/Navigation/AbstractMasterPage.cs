
using Acr.UserDialogs;
using Newtonsoft.Json;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{

    public class MenuColorConverter : AbstractConverter
    {
        public Color DefaultColor { get; set; }
        public Color SelectedColor { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == null || value as string == null)
                return null;

            if (string.Compare(parameter as string, value as string) == 0)
            {
                return SelectedColor;
            }
            else
            {
                return DefaultColor;
            }
        }
    }

    public abstract class AbstractMasterPage : ContentPage, IMasterDetailController, IPageLifeCycle
    {
        public Command OpenLeftSideMenu { get; set; }
        public Command CloseLeftSideMenu { get; set; }

        // Commands
        public Command LogoutCommand { get; set; }

        [Inject] IUserDialogs _userDialogs { get; set; }

        Dictionary<string, Page> _pageCache = new Dictionary<string, Page>();

        public void OpenFragment<T>(PageParameters param = null) where T : Page, IPageLifeCycle => 
            OpenFragmentAsync<T>(param).ConfigureAwait(false);
        bool _inOpeningProcess;
        public virtual async Task OpenFragmentAsync<T>(PageParameters param = null) where T : Page, IPageLifeCycle => 
            await OpenFragmentAsync(typeof(T),param);
        public virtual async Task OpenFragmentAsync(Type type, PageParameters param = null) 
        {
            if (_inOpeningProcess)
                return;

            try
            {
                _inOpeningProcess = true;

                string key = type + (param == null ? "" : JsonConvert.SerializeObject(param));

                if (_pageCache.ContainsKey(key))
                {
                    await SetDetailPage(_pageCache[key]);
                    return;
                }

                Page page = CoreApp.NavigationService.CreatePage(type, param, this) as Page;
                NavigationPage nav = new NavigationPage(page);
                

                //if(Device.RuntimePlatform == Device.Android)
                //{
                    //await Task.Delay(200);
                //}

                await SetDetailPage(nav);

                _pageCache.Add(key, nav);

            }
            catch (Exception ex)
            {
                ExceptionManager.SendErrorAndShowExceptionDialog(ex, Tx.T("CommonMessages_UnknownError"));
            }
            finally
            {
                _inOpeningProcess = false;
            }

        }

        public AbstractMasterPage(MasterDetailPage masterDetailPage_)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _masterDetailPage = masterDetailPage_;

            LogoutCommand = new Command(async () => await LogoutCmd());
            OpenLeftSideMenu = new Command(() => _masterDetailPage.IsPresented = true);
            CloseLeftSideMenu = new Command(() => _masterDetailPage.IsPresented = false);
        }

        protected MasterDetailPage _masterDetailPage;

        public async Task LogoutCmd()
        {
            CoreApp.LogoutCommand.Execute(null);

            Page loginPage = CoreApp.Current.CreateLoginPage();
            INavigationService navigationService = Locator.CurrentMutable.GetService<INavigationService>();
            await navigationService.PopToRootAsync(animation: false);
            //CoreApp.NavigationPage.Navigation.InsertPageBefore(loginPage,CoreApp.NavigationService.NavigationStack[0]);

            await CoreApp.NavigationService.PopToRootAsync(false);
        }

        async Task SetDetailPage(Page page_)
        {
            _masterDetailPage.Detail = page_;

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);

        }

        void TryToDestroyPage(Page page)
        {
            IPageLifeCycle ilc = page as IPageLifeCycle;
            ilc?.OnDestroyPage();

            NavigationAbstraction.CommonPage commonPage = page as NavigationAbstraction.CommonPage;
            commonPage?.OnDestroyPage();

        }

        public virtual void OnDestroyPage()
        {
            UnapplyBindings();
            

            foreach (var item in _pageCache)
            {
                TryToDestroyPage(item.Value);

                if (item.Value is NavigationPage)
                {
                    foreach(var page in (item.Value as NavigationPage).Navigation.NavigationStack)
                    {
                        TryToDestroyPage(page);
                    }
                }
            }
        }

        public virtual void Initialize()
        {
        }
    }
}
