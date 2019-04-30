

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [ContentProperty(nameof(Text))]
    public class CapitilizeByPlatformExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return Text?.ToUpper();
            }
            else
            {
                return Text;
            }
        }
    }

}
