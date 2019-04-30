
using Xamarin.Forms;

namespace CoreXF
{
    public class ImageSourceHelper
    {
        public static ImageSource FromResources(string shortPath)
        {
            if (string.IsNullOrEmpty(shortPath)) return null;

            string path = $"{CoreApp.MainAssemblyName}.Resources.{shortPath}";
            return ImageSource.FromResource(path, CoreApp.MainAssembly);
        }
    }
}
