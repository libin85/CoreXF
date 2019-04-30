using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CoreXF
{
    public static class ThicknessExtensions
    {
        public static Thickness Clone(this Thickness thickness,double addLeft = 0,double addTop = 0, double addRight = 0, double addBottom = 0)
        {
            return new Thickness(
                thickness.Left + addLeft,
                thickness.Top + addTop, 
                thickness.Right + addRight,
                thickness.Bottom + addBottom);
        }
    }
}
