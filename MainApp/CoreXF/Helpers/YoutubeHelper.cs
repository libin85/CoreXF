
using System.Text.RegularExpressions;

namespace CoreXF
{
    public class YoutubeHelper
    {
        static Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");

        // https://stackoverflow.com/questions/3652046/c-sharp-regex-to-get-video-id-from-youtube-and-vimeo-by-url
        //Youtube: youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a - zA - Z0 - 9 - _]+)
        //Vimeo: vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)

        public static string GetYoutubeVideoId(string url)
        {
            Match youtubeMatch = YoutubeVideoRegex.Match(url);

            if (youtubeMatch.Success)
            {
                return youtubeMatch.Groups[1].Value;
            }

            return null;
        }
    }
}
