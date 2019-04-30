using CoreXF.Droid.Services;
using Splat;

namespace CoreXF.Droid
{
    public static class Bootstrap
    {
        public static void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterLazySingleton<ISimpleAudioPlayer>(() => new AndroidExoPlayer());
        }

        public static void MainActivity_OnCreate()
        {
            CoreApp.ILocalize = new Localize();
        }
    }
}