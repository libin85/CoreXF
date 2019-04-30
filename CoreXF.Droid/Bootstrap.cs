
using Splat;

namespace CoreXF.Droid
{
    public static class Bootstrap
    {
        public static void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterLazySingleton<ISimpleAudioPlayer>(() => new SimpleAudioPlayerImplementation());
        }

        public static void MainActivity_OnCreate()
        {
            CoreApp.ILocalize = new Localize();
        }
    }
}