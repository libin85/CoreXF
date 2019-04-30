
using System;
using System.IO;
using System.Reflection;

namespace CoreXF
{

    public abstract class ResourceLoader
    {

        public static readonly Assembly CoreAssembly = typeof(CoreApp).GetTypeInfo().Assembly;
        public static readonly string CoreAssemblyName = typeof(CoreApp).GetTypeInfo().Assembly.GetName().Name;

        public static string CoreResourcePath => "core:";

        // <summary>
        /// Get a resource stream from an assembly
        /// </summary>
        /// <param name="path">Path to a resource without assembly name. For example Resource.avatar.png</param>
        /// <param name="asm">Resources' assemlby. by default will be using main portable assembly</param>
        /// <returns></returns>
        public static Stream GetStream(string path)
        {
            string asmName = CoreApp.MainAssemblyName;
            string realPath = path;
            Assembly selectedAssembly = CoreApp.MainAssembly;

            if (path.StartsWith(CoreResourcePath))
            {
                asmName = CoreAssemblyName;
                realPath = path.Replace(CoreResourcePath, "");
                selectedAssembly = CoreAssembly;
            }

            Stream stream = selectedAssembly.GetManifestResourceStream($"{asmName}.Resources.{realPath}");
            if (stream == null) throw new Exception($"Error retrieving {path} from Assembly {asmName}. Make sure Build Action is Embedded Resource");
            return stream;
        }

        public static Func<string,Stream> GetStreamFromNativeResouces;

    }
}
