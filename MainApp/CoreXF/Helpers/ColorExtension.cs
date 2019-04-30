
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public static class ColorExtension
    {

        public static string ToHexRGB(this Color color)
        {
            int red = (int)(color.R * 255);
            int green = (int)(color.G * 255);
            int blue = (int)(color.B * 255);
            int alpha = (int)(color.A * 255);
            return String.Format("#{0:X2}{1:X2}{2:X2}", red, green, blue);
        }

        public static string ToHexRGBA(this Color color)
        {
            int red = (int)(color.R * 255);
            int green = (int)(color.G * 255);
            int blue = (int)(color.B * 255);
            int alpha = (int)(color.A * 255);
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", red, green, blue, alpha);
        }

    }
}
