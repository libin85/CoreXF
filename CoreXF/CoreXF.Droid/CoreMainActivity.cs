
using Android.Content;
using Microsoft.AppCenter.Push;

namespace CoreXF.Droid
{
    public class CoreMainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static CoreMainActivity Current;

        public CoreMainActivity()
        {
            // Temp folder
            var context = new Android.Content.ContextWrapper(Android.App.Application.Context);
            CoreApp.TempFolder = context.CacheDir.AbsolutePath;
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }
    }
}