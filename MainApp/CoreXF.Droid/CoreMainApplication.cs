
using Android.App;
using Android.OS;
using System;
using Android.Runtime;

namespace CoreXF.Droid
{

    public class CoreMainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static bool IsActive;

        public CoreMainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {

            var b = ReactiveUI.HandlerScheduler.MainThreadScheduler;
            ReactiveUI.RxApp.MainThreadScheduler = ReactiveUI.HandlerScheduler.MainThreadScheduler;

            ResourceLoader.GetStreamFromNativeResouces = filename => Assets.Open(filename);
        }

        public virtual void OnActivityDestroyed(Activity activity)
        {
        }

        public virtual void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public virtual void OnActivityPaused(Activity activity)
        {
            IsActive = false;
        }

        public virtual void OnActivityResumed(Activity activity)
        {
            IsActive = true;

        }

        public virtual void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public virtual void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}