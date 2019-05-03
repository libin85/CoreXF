using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreXF
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpper(this string str)
        {
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static string Set(this string str,params object[] args) => string.Format(str, args);

        public static bool NotNullAndEmpty(this string str) => !string.IsNullOrWhiteSpace(str);

        // https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string
        public static Stream ToStream(this string str, Encoding enc = null)
        {
            enc = enc ?? Encoding.UTF8;
            return new MemoryStream(enc.GetBytes(str ?? ""));
        }

        public static string ToSanitizedKey(this string key)
        {
            return new string(key.ToCharArray()
                .Where(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                .ToArray());
        }
    }
}
