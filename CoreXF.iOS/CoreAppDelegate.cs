
using Foundation;
using ReactiveUI;
using System.IO;
using System;

namespace CoreXF.iOS
{
    public abstract class CoreAppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        
        public CoreAppDelegate()
        {
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());

            // temp path
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            CoreApp.TempFolder = Path.Combine(documents, "..", "Library", "Caches");

            ResourceLoader.GetStreamFromNativeResouces = filename =>
            {
                string ext = Path.GetExtension(filename);
                string filenameNoExt = filename.Substring(0, filename.Length - ext.Length);
                string path = filenameNoExt;// Path.Combine("my/folder", filenameNoExt);
                var resourcePathname = NSBundle.MainBundle.PathForResource(path, ext.Substring(1, ext.Length - 1));
                var fStream = new FileStream(resourcePathname, FileMode.Open, FileAccess.Read);
                return fStream;
            };
        }
    }
}