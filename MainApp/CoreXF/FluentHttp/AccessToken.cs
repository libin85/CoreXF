
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace CoreXF
{
    public class AccessToken
    {
        public string Token { get; set; }

        public DateTimeOffset Expires { get; set; }

        public DateTimeOffset Issued { get; set; }

        public string UserName {get;set;}

        public bool Expired() => 
            Expires <= DateTime.UtcNow;

        public override string ToString() => $"AccessToken  UserName={UserName}  Expires={Expires}  Issued={Issued} ";

        public static async Task<AccessToken> CreateToken(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            AccessTokenDto tokenDto = JsonConvert.DeserializeObject<AccessTokenDto>(content);

            AccessToken token = new AccessToken();
            token.Token = tokenDto.access_token;
            token.UserName = Regex.Replace(tokenDto.userName, "[^a-zA-Z0-9_+.]+", "");
            token.Expires = DateTimeOffset.ParseExact(tokenDto.expires, "r", System.Globalization.CultureInfo.InvariantCulture);
            token.Issued = DateTimeOffset.ParseExact(tokenDto.issued, "r", System.Globalization.CultureInfo.InvariantCulture);

            Debug.WriteLine(token);

            return token;
        }
    }

    [Preserve(AllMembers = true)]
    public class AccessTokenDto
    {

        public string access_token { get; set; }

        public string token_type { get; set; }

        //public string expires_in { get;set;}

        public string userName { get; set; }

        [JsonProperty(PropertyName = ".issued")]
        public string issued { get; set; }

        [JsonProperty(PropertyName = ".expires")]
        public string expires { get; set; }

    }
}
