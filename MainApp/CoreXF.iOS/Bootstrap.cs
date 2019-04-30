using Splat;

namespace CoreXF.iOS
{
    public static class Bootstrap
    {
        public static void RegisterDependencies()
        {
            Locator.CurrentMutable.RegisterLazySingleton<ISimpleAudioPlayer>(() => new SimpleAudioPlayerImplementation());
        }

        public static void OnFinishedLaunching_Start()
        {
            CoreApp.ILocalize = new Localize();

        }
    }
}