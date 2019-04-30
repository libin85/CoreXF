
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    //  5s         6-8          6+ 8+       X               Samsung J1      XHPDI             XXHPDI            XXXHDPI
    //  320x568    375x667      414x736     375х812            
    //  640x1136   750x1334     1242x2208   1125x2436       480x854 (115)   720x1280 (319)    1080x1920 (118)   1440x2560 (32)
    //                                                      480x800 (40)    750x1334 (13)
    //                                                      960x540 (16)

    public abstract class AbstractSizeExtension
    {
        public double Mini { get; set; } = -1;

        public double iOS { get; set; } = -1;
        public double iOSmini { get; set; } = -1;

        public double iPhone5s { get; set; } = -1;
        public double iPhone6 { get; set; } = -1;
        public double iPhonePlus { get; set; } = -1;
        public double iPhoneX { get; set; } = -1;

        public double Android { get; set; } = -1;
        public double AndroidMini { get; set; } = -1;

        public double Android480 { get; set; } = -1;
        public double Android720 { get; set; } = -1;
        public double Android1080 { get; set; } = -1;
        public double Android1440 { get; set; } = -1;

        public double Percent { get; set; } = -1;

        public double Delta { get; set; } = 0;

        public double Max { get; set; } = -1;
        public double Min { get; set; } = -1;

        protected double ByCertainPlatform()
        {
            double dispHeight = Device.Info.PixelScreenSize.Height;
            double dispWidth  = Device.Info.PixelScreenSize.Width;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    if (dispHeight == 1136) return iPhone5s;
                    if (dispHeight == 1334) return iPhone6;
                    if (dispHeight == 2208) return iPhonePlus;
                    if (dispHeight == 2436) return iPhoneX;
                    break;

                case Device.Android:
                    if (dispWidth < 600)  return Android480;
                    if (dispWidth < 800)  return Android720;
                    if (dispWidth < 1100) return Android1080;
                    if (dispWidth < 1500) return Android1440;
                    break;
            }
            return -1;
        }

        protected object ConvertValue(double res,IServiceProvider serviceProvider)
        {
            IProvideValueTarget targ = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            BindableProperty bp = targ?.TargetProperty as BindableProperty;

            double res2 = res;
            if (Max > -1 && res > Max)
                res2 = Max;
            if (Min > -1 && res < Min)
                res2 = Min;

            switch (bp?.ReturnType)
            {
                case Type gl when gl == typeof(GridLength):
                    return new GridLength(res2, GridUnitType.Absolute);

                //case Type db when db == typeof(double):
                //    return res;

                default:
                    return res2;
                    //throw new Exception("AdjustByScreenHeightExtension : Unknown type");
            }
        }

    }

    [ContentProperty(nameof(Height))]
    public class HeightExtension : AbstractSizeExtension, IMarkupExtension
    {

        public double Height { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            double res = Height;

            double byCertainPltf = ByCertainPlatform();
            if (byCertainPltf > -1)
                return ConvertValue(byCertainPltf + Delta, serviceProvider);

            if (Percent > -1)
                return ConvertValue(Math.Round(Device.Info.PixelScreenSize.Height / Device.Info.ScalingFactor / 100 * Percent, 0) + Delta, serviceProvider);

            switch (Device.RuntimePlatform)
            {

                case Device.iOS:
                    if(Device.Info.PixelScreenSize.Height < 1200)
                    {
                        res =  // by priority
                            iOSmini > -1 ? iOSmini : 
                            Mini > -1 ?Mini :
                            iOS > -1 ? iOS : 
                            Height;
                    } else
                    {
                        res= // by priority
                            iOS > -1 ? iOS : 
                            Height;
                    }
                    break;

                case Device.Android:
                    if (Device.Info.PixelScreenSize.Height < 1000)
                    {
                        res = // by priority
                            AndroidMini > -1 ? AndroidMini :
                            Mini > -1 ? Mini :
                            Android > -1 ? Android : 
                            Height;
                    }
                    else
                    {
                        res = // by priority
                            Android > -1 ? Android : 
                            Height;
                    }
                    break;

            }

            return ConvertValue(res + Delta, serviceProvider);
        }
    }

    [ContentProperty(nameof(Width))]
    public class WidthExtension : AbstractSizeExtension, IMarkupExtension
    {
        public double Width { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            double res = Width;

            double byCertainPltf = ByCertainPlatform();
            if (byCertainPltf > -1) return ConvertValue(byCertainPltf + Delta, serviceProvider);

            if (Percent > -1)
                return ConvertValue(Math.Round(Device.Info.PixelScreenSize.Width / Device.Info.ScalingFactor / 100 * Percent, 0) + Delta, serviceProvider);

            switch (Device.RuntimePlatform)
            {

                case Device.iOS:
                    if (Device.Info.PixelScreenSize.Width < 750)
                    {
                        res =  // by priority
                            iOSmini > -1 ? iOSmini :
                            Mini > -1 ? Mini :
                            iOS > -1 ? iOS :
                            Width;
                    }
                    else
                    {
                        res = // by priority
                            iOS > -1 ? iOS :
                            Width;
                    }
                    break;

                case Device.Android:
                    if (Device.Info.PixelScreenSize.Width < 700)
                    {
                        res = // by priority
                            AndroidMini > -1 ? AndroidMini :
                            Mini > -1 ? Mini :
                            Android > -1 ? Android :
                            Width;
                    }
                    else
                    {
                        res = // by priority
                            Android > -1 ? Android :
                            Width;
                    }
                    break;

            }

            return ConvertValue(res + Delta, serviceProvider);
        }
    }

    [ContentProperty(nameof(Value))]
    public abstract class SizeExtension<T> : IMarkupExtension
    {
        public T Value { get; set; }

        public T Mini { get; set; }

        public T iOS { get; set; }
        public T iOSmini { get; set; }

        public T Android { get; set; } 
        public T AndroidMini { get; set; }


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            T res = Value;

            switch (Device.RuntimePlatform)
            {

                case Device.iOS:
                    if (Device.Info.PixelScreenSize.Height < 1200)
                    {
                        res =  // by priority
                            !EqualityComparer<T>.Default.Equals(iOSmini,default(T)) ? iOSmini :
                            !EqualityComparer<T>.Default.Equals(Mini, default(T)) ? Mini :
                            !EqualityComparer<T>.Default.Equals(iOS, default(T))? iOS :
                            Value;
                    }
                    else
                    {
                        res = // by priority
                            !EqualityComparer<T>.Default.Equals(iOS, default(T)) ? iOS :
                            Value;
                    }
                    break;

                case Device.Android:
                    if (Device.Info.PixelScreenSize.Height < 1000)
                    {
                        res = // by priority
                            !EqualityComparer<T>.Default.Equals(AndroidMini, default(T)) ? AndroidMini :
                            !EqualityComparer<T>.Default.Equals(Mini, default(T)) ? Mini :
                            !EqualityComparer<T>.Default.Equals(Android, default(T)) ? Android :
                            Value;
                    }
                    else
                    {
                        res = // by priority
                            !EqualityComparer<T>.Default.Equals(Android, default(T)) ? Android :
                            Value;
                    }
                    break;

            }

            return res;
        }
    }

    [ContentProperty(nameof(Value))]
    public class MarginExtension : SizeExtension<Thickness>
    {

    }
}
