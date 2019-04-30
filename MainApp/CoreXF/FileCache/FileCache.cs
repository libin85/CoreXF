using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace CoreXF
{
    public class FileCache
    {
        public static string CacheFolder { get; }

        public static FileCache Instance => _instance ?? (_instance = new FileCache());
        static FileCache _instance;

        static FileCache()
        {
            CacheFolder = Path.Combine(CoreApp.TempFolder, "FileCache");
        }
    }
}
