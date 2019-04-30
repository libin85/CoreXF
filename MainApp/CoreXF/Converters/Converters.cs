
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace CoreXF
{
    public class CapitalizeByPlatformConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string))
                return null;

            if (Device.RuntimePlatform == Device.Android)
            {
                return (value as string).ToUpper();
            }
            else
            {
                return (value as string);
            }
        }
    }

    public class ToTitleCaseConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = null;
            if (parameter is string)
            {
                res = string.Format(parameter as string, value);
            }
            else
            {
                res = value is string ? value as string : value.ToString();
            }

            return res?.FirstLetterToUpper();
        }
    }

    public class BoolToValueConverter : AbstractConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool _value = AbstractConverter.ValueToBool(value);
            return _value ? TrueValue : FalseValue;
        }
    }

    public class BoolToStringConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            bool _value = AbstractConverter.ValueToBool(value);

            string str = parameter as string;

            int poz = str.IndexOf(',');
            if (_value)
            {
                return str.Substring(0, poz);
            }
            else
            {
                return str.Substring(poz + 1);
            }
        }
    }

    public class ToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            switch (value)
            {
                case string svalue:
                    return svalue.NotNullAndEmpty();

                case int ivalue:
                    return !(ivalue == 0);

                case long lvalue:
                    return !(lvalue == 0);

                case DateTimeOffset dtoValue:
                    return dtoValue != default(DateTimeOffset);

                case IList list:
                    return list.Count != 0;

                case IEnumerable ienum:
                    return ienum.Count() != 0;

                default:
                    return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }

    public class ToVisibleNotConverter : IValueConverter
    {
        static ToVisibleConverter _conv = new ToVisibleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !((bool)_conv.Convert(value, targetType, parameter, culture));
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }

    public class ToStringConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            if (value is string)
                return value as string;

            string result = "";

            var coll = value as IEnumerable;
            if (coll != null)
            {
                foreach (var elm in coll)
                {
                    if (elm == null)
                        continue;

                    result += (result == "" ? "" : ", ") + elm.ToString();
                }
                return result;
            }
            else if (value.GetType().IsValueType)
            {
                result = value.ToString();
            }
            else
            {
                result = value?.ToString();
            }
            return result;
        }

    }

    public class EqualConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string valstr = value.ToString();
            string parstr = parameter is string ? (string)parameter : parameter.ToString();

            if (parstr.Contains("||"))
            {
                foreach (var elm in parstr.Replace("||", "|").Split('|'))
                {
                    if (string.Compare(elm.Trim(), valstr) == 0)
                        return true;
                }
                return false;
            }
            else
            {
                int intres = string.Compare(valstr, parstr);
                return intres == 0;
            }
        }
    }

    public class BoolToTxStringConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = parameter as string;
            if (!(value is bool) || str == null)
                return null;

            int poz = str.IndexOf(',');
            if ((bool)value)
            {
                return Tx.T(str.Substring(0, poz));
            }
            else
            {
                return Tx.T(str.Substring(poz + 1));
            }
        }
    }

    public class BoolToColorConverter : AbstractConverter
    {
        static ToVisibleConverter toVisibleConverter = new ToVisibleConverter();

        public Color FalseColor { get; set; }
        public Color TrueColor { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isItTrue = false;
            if (value is bool)
            {
                isItTrue = (bool)value;
            }
            else
            {
                isItTrue = (bool)toVisibleConverter.Convert(value, targetType, null, culture);
            }

            return isItTrue ? TrueColor : FalseColor;
        }

    }


    public class IsNullConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return String.IsNullOrEmpty(value as string);
            }
            else
            {
                return value == null;
            }

        }
    }

    public class NotNullConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case string val_str:
                    return !String.IsNullOrEmpty(val_str);

                case DateTime val_dt:
                    bool res = val_dt != default(DateTime);
                    return res;

                case DateTimeOffset val_dto:
                    bool res1 = val_dto != default(DateTimeOffset);
                    return res1;

                case IEnumerable val_enumerable:
                    return val_enumerable.GetEnumerator().MoveNext();

                default:
                    return value != null;


            }
        }
    }

    public class NotConverter : AbstractConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }
    }
}
