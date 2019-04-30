
using System.Linq;

namespace CoreXF
{
    public class UriHelper
    {
        // http://stackoverflow.com/questions/372865/path-combine-for-urls

        public static string Combine(params string[] uriParts)
        {
            string uri = string.Empty;
            if (uriParts != null && uriParts.Any())
            {
                char[] trims = new char[] { '\\', '/' };
                uri = (uriParts[0] ?? string.Empty).TrimEnd(trims);
                for (int i = 1; i < uriParts.Count(); i++)
                {
                    uri = $"{uri.TrimEnd(trims)}/{(uriParts[i] ?? string.Empty).TrimStart(trims)}";
                }
            }
            return uri;
        }
    }
}
