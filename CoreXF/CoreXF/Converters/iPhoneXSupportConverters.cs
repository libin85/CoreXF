using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{

    public class PlusSafeAreaToBottomMarginConverter : IValueConverter
    {

        static ThicknessTypeConverter converter = new ThicknessTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = new Thickness();

            if (!(value is Thickness) || string.IsNullOrEmpty(parameter as string))
            {
                margin.Bottom += CoreApp.Current.SafeArea.Bottom;
                return margin;
            }

            Thickness param = (Thickness)converter.ConvertFromInvariantString(parameter as string);
            Thickness safeArea = (Thickness)value;

            var result = new Thickness(param.Left, param.Top, param.Right, param.Bottom + safeArea.Bottom);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlusSafeAreaToTopMarginConverter : IValueConverter
    {

        static ThicknessTypeConverter converter = new ThicknessTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = new Thickness();

            if (!(value is Thickness) || string.IsNullOrEmpty(parameter as string))
            {
                margin.Top += CoreApp.Current.SafeArea.Top;
                return margin;
            }

            Thickness param = (Thickness)converter.ConvertFromInvariantString(parameter as string);
            Thickness safeArea = (Thickness)value;

            var result = new Thickness(param.Left, param.Top + safeArea.Top, param.Right, param.Bottom);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    

    /*

    public class MinusSafeAreaIOSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Thickness))
                return new Thickness(0);

            if (Device.RuntimePlatform != Device.iOS
                || Device.Idiom == TargetIdiom.Tablet
                //|| !DeviceInfo.DeviceName.Contains("X")
                )
                return new Thickness(0);

            Thickness safeArea = (Thickness)value;
            var result = new Thickness(-safeArea.Left, -safeArea.Top, -safeArea.Right, -safeArea.Bottom);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlusSafeAreaConverter : IValueConverter
    {
        static ThicknessTypeConverter converter = new ThicknessTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Thickness))
                return new Thickness(0);

            if (string.IsNullOrEmpty(parameter as string))
                return new Thickness(0);

            Thickness param = (Thickness)converter.ConvertFromInvariantString(parameter as string);
            Thickness safeArea = (Thickness)value;

            var result = new Thickness(param.Left + safeArea.Left, param.Top + safeArea.Top, param.Right + safeArea.Right, param.Bottom + safeArea.Bottom);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    */
}
