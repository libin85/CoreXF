using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace CoreXF
{
    /// <summary>
    /// Возвращает слова в падеже, зависимом от заданного числа 
    /// </summary>
    /// <param name="number">Число от которого зависит выбранное слово</param>
    /// <param name="nominativ">Именительный падеж слова. Например "день"</param>
    /// <param name="genetiv">Родительный падеж слова. Например "дня"</param>
    /// <param name="plural">Множественное число слова. Например "дней"</param>
    /// <returns></returns>
    public class DeclensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(parameter as string))
                return "";

            if(!int.TryParse(value.ToString(),out int number))
            {
                return "";
            }

            string param = parameter as string;
            if (param.StartsWith("{}"))
                param = param.Substring(2);

            var forms = (param as string).Split(',');

            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                return forms[2];
            }

            var i = number % 10;
            string currentForm = null;
            switch (i)
            {
                case 1:
                    currentForm = forms[0];
                    break;

                case 2:
                case 3:
                case 4:
                    currentForm = forms[1];
                    break;

                default:
                    currentForm = forms[2];
                    break;
            }
            if (!string.IsNullOrEmpty(currentForm))
            {
                return string.Format(currentForm, number);
            }
            return currentForm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
