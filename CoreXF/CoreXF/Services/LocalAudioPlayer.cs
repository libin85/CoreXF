using System.IO;
using Splat;
using Xamarin.Forms;

namespace CoreXF
{
    public static class LocalAudioPlayer
    {
        class Cache : GenericCache<string, ISimpleAudioPlayer>
        {
            public override ISimpleAudioPlayer CreateValue(string key)
            {
                var _audioPlayer = Locator.CurrentMutable.GetService<ISimpleAudioPlayer>();

                using (var resourceStream = ResourceLoader.GetStream(key))
                {
                    _audioPlayer.Load(resourceStream);
                }

                return _audioPlayer;
            }
        }

        static Cache cache = new Cache();

        public static void PlaySound(string name)
        {
            // is this simulator ?
            if (Device.RuntimePlatform == Device.iOS && Xamarin.Essentials.DeviceInfo.Model == "x86_64")
                return;

            var result = cache.GetOrCreateValue(name);
            result?.Play();
        }
    }
}