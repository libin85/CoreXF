
using System.Globalization;

namespace CoreXF
{
    public class CultureInfoExt : CultureInfo
    {
        public string NameExt { get; set; }

        public CultureInfoExt(string name) : base(name)
        {
            NameExt = $"{NativeName.Substring(0, 1).ToUpper()}{NativeName.Substring(1).ToLower()}";
        }

        public override string ToString() => $"{NameExt} ({TwoLetterISOLanguageName})";

    }
}
