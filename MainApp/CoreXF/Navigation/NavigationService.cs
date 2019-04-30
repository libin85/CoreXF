
using CoreXF.NavigationAbstraction;
using Microsoft.AppCenter.Analytics;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{

    public interface IDialogXF
    {
        void Initialize(DialogParameters param);
        Command CloseCommand { get; set; }
    }

    public interface IDialogXF2 : IDialogXF
    {
        void StartFocus();
        void StartUnfocus();
    }

    public class DialogParameters : PageParameters, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public interface INavigationService
    {
        //IReadOnlyList<Page> NavigationStack { get; }

        Task NavigateToAsync<TPage>(PageParameters parameters = null) where TPage : Page;
        Task NavigateToAsync(Type type, PageParameters parameters = null);
        Task NavigateToAsync(Page page);
        Task NavigateBackAsync();

        Page CreatePage(Type type, PageParameters parameters = null, IMasterDetailController masterDetailController = null);
        TPage CreatePage<TPage>(PageParameters parameters = null) where TPage : Page;

        Task PopUntilDestination<T>() where T : Page;

        Task OpenFirstPage(Type StartPage, Type StartFragment);

        //Task PushAsync(Page page);
        Task PopToRootAsync(bool animation);

        void ShowAlert(string title, string message, string cancel);
        void ShowDialog<T>(DialogParameters parameters) where T : View, IDialogXF, new();
        //void ShowExceptionDialog(CaughtExceptionModel model);

        /// <summary>
        /// Removes page from the navigation stack
        /// </summary>
        /// <param name="page"></param>
        void RemovePage(Page page);

        /// <summary>
        /// Removes all T type pages from the navigation stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemovePage<T>() where T : Page;

        /// <summary>
        /// Removes all pages in the navigation stack till type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemovePagesTill<T>() where T : Page;
    }

    public class NavigationService : INavigationService
    {
        INavigation _Navigation => CoreApp.NavigationPage.Navigation;

        public NavigationService() {
        }

        void DestroyPage(Page topPage)
        {
            ((IPageLifeCycle)topPage).OnDestroyPage();
            (topPage as IDisposable)?.Dispose();
        }

        public async Task NavigateBackAsync()
        {
            if(_Navigation.NavigationStack.Count > 1)
            {
                DestroyPage(_Navigation.NavigationStack[_Navigation.NavigationStack.Count - 1]);
            }
            await popAsync();
        }


        async Task popAsync()
        {
            SetNavigationBackFlag();
            await _Navigation.PopAsync();
        }
        //public async Task PushAsync(Page page) => await _Navigation.PushAsync(page);

        //public IReadOnlyList<Page> NavigationStack => _Navigation.NavigationStack;

        void SetNavigationBackFlag()
        {
            if(_Navigation.NavigationStack.Count > 1)
            {
                CommonPage commonPage = FindPage<CommonPage>(_Navigation.NavigationStack[_Navigation.NavigationStack.Count - 2]);
                if(commonPage != null)
                {
                    commonPage.IsBackNavigation = true;
                    _SetOffNavgigationFlag(new WeakReference<CommonPage>(commonPage)).ConfigureAwait(false);
                }
            }

            async Task _SetOffNavgigationFlag(WeakReference<CommonPage> ref_page)
            {
                await Task.Delay(1000);
                if (ref_page.TryGetTarget(out CommonPage page))
                {
                    page.IsBackNavigation = false;
                }
            }

        }

        T FindPage<T>(Page previousPage = null) where T : Page
        {
            if (previousPage == null)
            {
                int navCount = _Navigation?.NavigationStack?.Count ?? 0;
                if (navCount > 0)
                {
                    previousPage = FindPage<ContentPage>(_Navigation.NavigationStack[navCount - 1]);
                }
            }
            if (previousPage == null)
                return null;

            T commonPage = null;
            switch (previousPage)
            {
                case T cpage:
                    commonPage = cpage;
                    break;

                case MasterDetailPage mdPage:
                    commonPage = FindPage<T>(mdPage.Detail);
                    break;

                case NavigationPage nPage:
                    int navcnt = nPage?.Navigation?.NavigationStack?.Count ?? 0;
                    if (navcnt > 0)
                    {
                        commonPage = FindPage<T>(nPage.Navigation.NavigationStack[navcnt - 1]);
                    }
                    break;
            }
            return commonPage;
        }

        public Grid GetTopGrid(ContentPage page = null)
        {
            if (page == null)
            {
                page = FindPage<ContentPage>();
            }
            if (page == null)
            {
                return null;
            }
                
            Grid grd = page.Content as Grid;
            if (grd == null)
            {
                ExceptionManager.SendError(new Exception($"ShowDialog cannot find Grid in {grd}"));
                return null;
            }
            return grd;
        }

        public void ShowDialog<T>(DialogParameters parameters) where T : View, IDialogXF, new()
        {
            Grid topGrid = GetTopGrid();
            if (topGrid == null)
            {
                ExceptionManager.SendError(new Exception("ShowDialog cannot find root page"));
                return;
            }

            View dlg = new T();
            IDialogXF xfDialog = dlg as IDialogXF;
            if (xfDialog == null)
            {
                ExceptionManager.SendError(new Exception("ShowDialog cannot find IDialogXF"));
                return;
            }

            xfDialog.CloseCommand = new Command(async () =>
            {
                (xfDialog as IDialogXF2)?.StartUnfocus();

                //    var ani = new Animation(v =>
                //    {
                //        dialogTemplate.
                //        TranslationY = 40 * v;
                //    }, IsActive ? 1 : 0, IsActive ? 0 : 1, null, null);
                //    ani.Commit(this, "sdfs1", length: 250, easing: Easing.Linear);
                //}

                dlg.IsVisible = false;
                await Task.Delay(50);
                topGrid.Children.Remove(dlg);
                xfDialog.CloseCommand = null;
            });
            xfDialog.Initialize(parameters);
            dlg.BindingContext = dlg;

            topGrid.Children.Add(dlg,
                0,
                topGrid.ColumnDefinitions.Count == 0 ? 1 : topGrid.ColumnDefinitions.Count,
                0,
                topGrid.RowDefinitions.Count == 0 ? 1 : topGrid.RowDefinitions.Count);

            (xfDialog as IDialogXF2)?.StartFocus();
        }

        public void ShowAlert(string title,string message,string cancel)
        {
            ContentPage contentPage = FindPage<ContentPage>();
            contentPage.DisplayAlert(title,message,cancel);
        }

        public async Task NavigateToAsync<TPage>(PageParameters parameters = null) where TPage : Page => 
            await NavigateToAsync(typeof(TPage), parameters);

        public async Task NavigateToAsync(Type type,PageParameters parameters = null)
        {
            //Debug.WriteLine($"NAV: NavigateToAsync {type} { parameters?.ToString()}");
            var dict = new Dictionary<string, string>();
            dict.Add("Page", type.ToString());

#if RELEASE
            Analytics.TrackEvent("OpenPage",dict);
#endif
            Page page = CreatePage(type, parameters);
            await NavigateToAsync(page);
        }

        public async Task NavigateToAsync(Page page)
        {
            var task = _Navigation?.PushAsync(page);
            if (task != null) await task;
        }

        public static void InjectDependencies(Page page,Type type = null)
        {
            Type baseType = type == null ? page.GetType() : type;

            // Injector
            Type injectType = typeof(InjectAttribute);
            foreach (PropertyInfo sourceProp in baseType.GetRuntimeProperties())
            {
                // Inject dependencies
                var attr = sourceProp.GetCustomAttribute(injectType);
                if (attr != null)
                {
                    object value = Locator.CurrentMutable.GetService(sourceProp.PropertyType);
                    sourceProp.SetValue(page, value);
                    continue;
                }
            }

            if (type == null)
                InjectDependencies(page, baseType.BaseType);

        }

        public TPage CreatePage<TPage>(PageParameters parameters = null) where TPage : Page => CreatePage(typeof(TPage), parameters) as TPage;

        public Page CreatePage(Type type,PageParameters parameters = null, IMasterDetailController masterDetailController = null)
        {
            Page page = null;
            try
            {
                page = Activator.CreateInstance(type) as Page;
            }
            catch (Exception ex)
            {
                ExceptionManager.SendError(ex,$"Cannot create page {type}");
            }

            if (page == null)
                throw new Exception("Navigation service target page is null");

            // Injector
            Type injectType = typeof(InjectAttribute);
            foreach (FieldInfo sourceProp in page.GetType().GetRuntimeFields())
            {
                // Inject parameters
                if (sourceProp.FieldType.BaseType == typeof(CoreXF.PageParameters))
                {
                    sourceProp.SetValue(page, parameters);
                    continue;
                }
            }
            // Inject parameters
            foreach (PropertyInfo sourceProp in page.GetType().GetRuntimeProperties())
            {
                if (sourceProp.PropertyType.BaseType == typeof(CoreXF.PageParameters))
                {
                    sourceProp.SetValue(page, parameters);
                    break;
                }
            }

            InjectDependencies(page);

            page.BindingContext = page;

            if(masterDetailController != null)
            {
                CommonPage frag = page as CommonPage;
                if(frag != null)
                {
                    frag.MasterDetailController = masterDetailController;
                }
            }


            (page as IPageLifeCycle)?.Initialize();

            return page;

        }

        /*
        public void ShowExceptionDialog(CaughtExceptionModel model)
        {
            ShowDialog<ExceptionAlert>(new ExceptionAlert.Parameters { ExceptionModel = model });
        }
        */

        public async Task PopUntilDestination<T>() where T : Page
        {
            int LeastFoundIndex = 0;
            int PagesToRemove = 0;

            for (int index = _Navigation.NavigationStack.Count - 2; index > 0; index--)
            {
                //if (_Navigation.NavigationStack[index].GetType().Equals(typeof(T)))
                if(_Navigation.NavigationStack[index] as T != null)
                {
                    break;
                }
                else
                {
                    LeastFoundIndex = index;
                    PagesToRemove++;
                }
            }

            for (int index = 0; index < PagesToRemove; index++)
            {
                Page pg = _Navigation.NavigationStack[LeastFoundIndex];
                DestroyPage(pg);
                _Navigation.RemovePage(pg);
            }
            await popAsync();
        }
 
        public async Task PopToRootAsync(bool animation)
        {
            Debug.WriteLine($"NAV: PopToRootAsync {_Navigation.NavigationStack.Count}");
            for (int i = _Navigation.NavigationStack.Count - 1; i > 0; i--)
            {
                DestroyPage(_Navigation.NavigationStack[i]);
            }
            await CoreApp.NavigationPage.PopToRootAsync(false);
        }

        public async Task OpenFirstPage(Type StartPage,Type StartFragment)
        {
            AbstractMasterDetailPage startPage = CreatePage(StartPage) as AbstractMasterDetailPage;
            AbstractMasterPage Master = startPage.Master as AbstractMasterPage;

            await Master.OpenFragmentAsync(StartFragment);

            await NavigateToAsync(startPage);
        }

        public void RemovePage(Page page)
        {
            _Navigation.RemovePage(page);
            DestroyPage(page);
        }

        public void RemovePage<T>() where T : Page
        {
            var pages = _Navigation.NavigationStack.Where(x => x is T).ToArray();
            foreach(var page in pages)
            {
                RemovePage(page);
            }
        }

        public void RemovePagesTill<T>() where T : Page
        {
            List<Page> pages = new List<Page>();
            for(int i = 0; i < _Navigation.NavigationStack.Count; i++)
            {
                if (_Navigation.NavigationStack[i] is T)
                    break;

                pages.Add(_Navigation.NavigationStack[i]);
            }
            foreach (var page in pages)
                RemovePage(page);
        }

    }

}


