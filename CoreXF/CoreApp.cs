
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace CoreXF
{
    public abstract class CoreApp : Application
    {
        public const string CoreVersion = "2.0.0";

        public static NavigationPage NavigationPage;

        public static Assembly MainAssembly;
        public static string MainAssemblyName;

        public static string TempFolder {get;set;}

        public static Command LogoutCommand { get; set; }

        public static INavigationService NavigationService;

        public static IUser CurrentUser { get; set; }

        public static ILocalize ILocalize;
        public CultureInfoExt CurrentCulture { get; set; }
        public CultureInfoExt NeutralCulture { get; set; }
        public static CultureInfo DefaultCulture;

        public List<CultureInfoExt> LanguageList { get; set; }

        public static int MainThreadId;
        public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;

        public static CoreApp Current { get; protected set; }

        public Thickness SafeArea { get; set; }

        public abstract Page CreateLoginPage();

        // Приложение запущенно, пользователь залогинился, база данных инициализирована
        public static bool AppIsRunning;

        public virtual CultureInfoExt AdjustCulture(CultureInfo cultureCode)
        {
            return new CultureInfoExt(cultureCode.Name);
        }

        public static readonly List<ResourceManager> StringResourceManagers = new List<ResourceManager>();

        public abstract List<CultureInfoExt> BuildCultureList();

        public virtual void SetLanguage(string newCode = null)
        {
            try
            {
                if (newCode != null)
                {
                    CoreUserSettings.PreferredLanguage = newCode;
                }

                CultureInfoExt targetCulture = null;
                if (!string.IsNullOrEmpty(CoreUserSettings.PreferredLanguage))
                {
                    targetCulture = new CultureInfoExt(CoreUserSettings.PreferredLanguage);
                }
                else
                {
                    var cc = ILocalize.GetCurrentCultureInfo();
                    targetCulture = AdjustCulture(cc);
                }

                if (ILocalize.GetCurrentCultureInfo().TwoLetterISOLanguageName != targetCulture.TwoLetterISOLanguageName)
                {
                    ILocalize.SetLocale(targetCulture);
                }
                CurrentCulture = targetCulture;
            }
            catch(Exception ex)
            {
                ExceptionManager.SendError(ex, "Can't set language");
            }
        }

        public virtual void UpdateApp(int newVersion,int oldVersion)
        {
        }

        void SetSafeArea()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    SafeArea = new Thickness(0, 24, 0, 0);
                    break;

                case Device.iOS:
                    bool isXphone = DeviceInfo.DeviceName.Contains("X");
                    switch (Device.info.CurrentOrientation)
                    {
                        case DeviceOrientation.Other:
                        case DeviceOrientation.Portrait:
                        case DeviceOrientation.PortraitDown:
                        case DeviceOrientation.PortraitUp:
                            SafeArea = new Thickness(0, isXphone ? 44 : 20, 0, isXphone ? 34 : 0);
                            break;

                        case DeviceOrientation.Landscape:
                        case DeviceOrientation.LandscapeLeft:
                        case DeviceOrientation.LandscapeRight:
                            SafeArea = new Thickness(0, 0, 0, 0);
                            break;

                    }
                    break;
            }
        }

        public CoreApp()
        {
            Current = this;

            SetSafeArea();
            Device.info.PropertyChanged += DeviceInfo_PropertyChanged;

            // Main thread Id
            MainThreadId = Environment.CurrentManagedThreadId;

            // Unique app Id
            if (string.IsNullOrEmpty(CoreUserSettings.AppUUID))
                CoreUserSettings.AppUUID = Guid.NewGuid().ToString();

            // Update
            var arr = DeviceInfo.AppVersion.Split('.');
            int.TryParse(arr[arr.Length - 1],out int curVersion);
            int oldVersion = CoreUserSettings.CurrentAppVersion;
            if (curVersion > oldVersion)
            {
                UpdateApp(curVersion, oldVersion);
                CoreUserSettings.CurrentAppVersion = curVersion;
            }

            SystemDB.Initialize();

            // Localization
            LanguageList = BuildCultureList();
            SetLanguage();

            
            LogoutCommand = new Command(async () => await Logout());

        }

        private void DeviceInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Xamarin.Forms.Internals.DeviceInfo.CurrentOrientation):
                    SetSafeArea();
                    break;
            }
        }

        public async virtual Task Logout(bool DoNotClearPassword = false)
        {
            AppIsRunning = false;

            CoreUserSettings.AccessToken = null;
            if (!DoNotClearPassword)
            {
                CoreUserSettings.LastPasswordSecure = null;
            }

            //SystemDB.Close();
        }

        public virtual Task OnStartUserSession(int userId)
        {
            return Task.CompletedTask;
        }


    }
}
