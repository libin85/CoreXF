
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{

    [ContentProperty("FontSize")]
    public class FontSizeExtension : IMarkupExtension
    {
        public double FontSize { get; set; }
        //public double Factor { get; set; } = 0;
        public double Mini { get; set; } = -1;

        public double SizeiOS { get; set; } = -1;
        public double iOSmini { get; set; } = -1;
        //public double FactoriOS { get; set; } = -1;

        public double Android { get; set; } = -1;
        public double AndroidMini { get; set; } = -1;
        //public double FactorAndroid { get; set; } = -1;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:

                    if (Device.Info.PixelScreenSize.Width > 640)
                    {
                        return // by priority
                            SizeiOS > -1 ? SizeiOS : 
                            FontSize;
                    }
                    else
                    {
                        return // by priority
                            iOSmini > -1 ? iOSmini :
                            Mini > -1 ? Mini : 
                            SizeiOS > -1 ? SizeiOS : 
                            FontSize;
                    }

                case Device.Android:
                    if (Device.Info.PixelScreenSize.Width > 500)
                    {
                        return // by priority
                            Android > -1 ? Android : 
                            FontSize;
                    }
                    else
                    {
                        return // by priority
                            AndroidMini > -1 ? AndroidMini :
                            Mini > -1 ? Mini :
                            Android > -1 ? Android : 
                            FontSize;
                    }
                default:
                    return FontSize;
            }
        }
    }
}
