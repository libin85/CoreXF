
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class AbstractConverter : IValueConverter
    {
        abstract public object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public static bool ValueToBool(object value)
        {
            switch (value)
            {
                case bool vbool:
                    return vbool;

                case int vint:
                    return vint > 0;

            }
            return false;
        }


    }
}
