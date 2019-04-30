
using Windows.Storage;
using Windows.UI.Xaml;

namespace CoreXF.UWP
{
    public class CoreApplication : Application
    {

        public CoreApplication()
        {
            // temp path
            StorageFolder rootFolder = ApplicationData.Current.TemporaryFolder;
            CoreApp.TempFolder = rootFolder.Path;

        }

    }
}
