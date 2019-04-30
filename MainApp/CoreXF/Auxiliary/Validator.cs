
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreXF
{

    public static class Validator
    {

        //static Regex ValidEmailRegex = CreateValidEmailRegex();

        /// <summary>
        /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
        /// </summary>
        /// <returns></returns>
        /// https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        public static bool EmailIsValid(string emailAddress)
        {
            if (!emailAddress.NotNullAndEmpty())
                return false;

            bool result = new EmailAddressAttribute().IsValid(emailAddress);
            return result;
        }

        public static bool PasswordIsValid(string password)
        {
            return password.NotNullAndEmpty() && password.Length > 2;
        }

        public static bool StringIsValid(string str)
        {
            return str.NotNullAndEmpty();
        }

        public static bool UrlIsValid(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}
