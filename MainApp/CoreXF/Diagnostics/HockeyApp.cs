
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    public static class HockeyAppExtension
    {
        public static string HockeyAppId;

        static bool _inSync;
        public static async Task Sync()
        {
            if (_inSync) return;
            _inSync = true;

            try
            {
                foreach (var elm in SystemDB.Connection.Table<CaughtExceptionModel>().ToList())
                {
                    if (await SendHandledException(elm))
                    {
                        SystemDB.Connection.Delete(elm);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _inSync = false;
            }
        }

        static void RemoveQuotes(HttpContent content)
        {
            var contentTypeString = content.Headers.ContentType.ToString().Replace("\"", "");
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", contentTypeString);
        }

        // https://support.hockeyapp.net/kb/api/api-crashes
        public static async Task<bool> SendHandledException(CaughtExceptionModel model)
        {
            string version = DeviceInfo.AppVersion;
            var arr = version.Split('.');
            string short_ver = arr[arr.Length - 1];
            var deviceInfo = Plugin.DeviceInfo.CrossDeviceInfo.Current;

            StringBuilder sb = new StringBuilder(12000);
            sb.Append("Message: ").AppendLine(model.Message);

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    sb
                    .Append("Package: ").AppendLine(AppConfig.PackageId)
                    .Append("Version Code: ").AppendLine(short_ver)
                    .Append("Version Name: ").AppendLine(version)
                    .Append("Android: ").AppendLine(DeviceInfo.OsVersion)
                    .Append("Android Build: ").AppendLine(DeviceInfo.OsBuild)
                    .Append("Manufacturer: ").AppendLine(DeviceInfo.Manufacturer)
                    .Append("Model: ").Append(deviceInfo?.Model).Append("  ").AppendLine(deviceInfo?.Idiom.ToString())
                    .Append("Thread: Thread-unknown").AppendLine()
                    //.Append($"CrashReporter Key: F6F367AF-6EEF-70F7-48FE-506D752CAB8FB4766815").AppendLine()
                    .Append("CrashReporter Key: ").AppendLine(CoreUserSettings.AppUUID)
                    //.Append($"Start Date: Sun Sep 17 19:22:35 GMT+03:00 2017").AppendLine()
                    .Append($"Start Date: {model.DateTime.ToUniversalTime().ToString("r")} {model.DateTime.ToString("yyyy")}").AppendLine()
                    //.Append($"Date: Sun Sep 17 19:26:08 GMT+03:00 2017").AppendLine()
                    .Append($"Date: {model.DateTime.ToUniversalTime().ToString("r")} {model.DateTime.ToString("yyyy")}").AppendLine()
                    .Append("Format: Xamarin").AppendLine()
                    .AppendLine("Caught: true")
                    .Append("").AppendLine()
                    .Append(model.Body).AppendLine();
                    break;

                case Device.iOS:
                    sb
                    .Append("Package: ").AppendLine(AppConfig.PackageId)
                    .Append("Version Code: ").AppendLine(DeviceInfo.AppVersion)
                    .Append("Version Name: ").AppendLine("iOS")
                    .Append("Android: ").AppendLine(DeviceInfo.OsVersion)
                    .Append("Android Build: ").AppendLine(DeviceInfo.OsBuild)
                    .Append("Manufacturer: ").AppendLine("Apple")
                    .Append("Model: ").AppendLine(DeviceInfo.DeviceName)
                    .Append("Thread: Thread-unknown").AppendLine()
                    //.Append($"CrashReporter Key: F6F367AF-6EEF-70F7-48FE-506D752CAB8FB4766815").AppendLine()
                    .Append("CrashReporter Key: ").AppendLine(CoreUserSettings.AppUUID)
                    //.Append($"Start Date: Sun Sep 17 19:22:35 GMT+03:00 2017").AppendLine()
                    .Append($"Start Date: {model.DateTime.ToUniversalTime().ToString("r")} {model.DateTime.ToString("yyyy")}").AppendLine()
                    //.Append($"Date: Sun Sep 17 19:26:08 GMT+03:00 2017").AppendLine()
                    .Append($"Date: {model.DateTime.ToUniversalTime().ToString("r")} {model.DateTime.ToString("yyyy")}").AppendLine()
                    .Append("Format: Xamarin").AppendLine()
                    .AppendLine("Caught: true")
                    .Append("").AppendLine()
                    .Append(model.Body).AppendLine();
                    break;
            }

            HttpClient client = new HttpClient();
            var content1 = new MultipartFormDataContent();

            var stringContent = new StringContent(sb.ToString());

            stringContent.Headers.Add("Content-Disposition", "form-data; name=\"log\"; filename=\"log.txt\"");
            RemoveQuotes(stringContent);

            content1.Add(stringContent);
            RemoveQuotes(content1);

            try
            {
                using (var message = await client.PostAsync($"https://rink.hockeyapp.net/api/2/apps/{HockeyAppId}/crashes/upload?userID={WebUtility.UrlEncode(CoreUserSettings.LastLogin)}", content1))
                {
                    return message.StatusCode == System.Net.HttpStatusCode.Created;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }












        /*
         

            string srt =
            @"
Xamarin caused by: android.runtime.JavaProxyThrowable: System.AggregateException: Fuckup A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ---> System.Net.Sockets.SocketException: Connection timed out
at System.Net.Sockets.SocketAsyncResult.CheckIfThrowDelayedException () [0x00014] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Net.Sockets.Socket.EndConnect (System.IAsyncResult asyncResult) [0x0002c] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Net.Sockets.TcpClient.EndConnect (System.IAsyncResult asyncResult) [0x0000c] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Threading.Tasks.TaskFactory`1[TResult].FromAsyncCoreLogic (System.IAsyncResult iar, System.Func`2[T,TResult] endFunction, System.Action`1[T] endAction, System.Threading.Tasks.Task`1[TResult] promise, System.Boolean requiresSynchronization) [0x00019] in <896ad1d315ca4ba7b117efb8dacaedcf>:0
";
        
         
        string srt = model.Body;

            @"
Xamarin caused by: android.runtime.JavaProxyThrowable: System.AggregateException: Fuckup A Task's exception(s) were not observed either by Waiting on the Task or accessing its Exception property. As a result, the unobserved exception was rethrown by the finalizer thread. ---> System.Net.Sockets.SocketException: Connection timed out
at System.Net.Sockets.SocketAsyncResult.CheckIfThrowDelayedException () [0x00014] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Net.Sockets.Socket.EndConnect (System.IAsyncResult asyncResult) [0x0002c] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Net.Sockets.TcpClient.EndConnect (System.IAsyncResult asyncResult) [0x0000c] in <6c708cf596db438ebfc6b7e012659eee>:0
at System.Threading.Tasks.TaskFactory`1[TResult].FromAsyncCoreLogic (System.IAsyncResult iar, System.Func`2[T,TResult] endFunction, System.Action`1[T] endAction, System.Threading.Tasks.Task`1[TResult] promise, System.Boolean requiresSynchronization) [0x00019] in <896ad1d315ca4ba7b117efb8dacaedcf>:0
";
*/

        /*
         
POST /api/2/apps/APP_ID/crashes/upload? HTTP/1.1
Host: rink.hockeyapp.net
X-HockeyAppToken: TOKEN
Cache-Control: no-cache
Postman-Token: fb33191f-fa8d-e57f-af66-4b72841e807
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW




------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="userID"




MyId
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="contact"




MyContact
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="log"; filename=""
Content-Type: 




------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="description"; filename=""
Content-Type: 




------WebKitFormBoundary7MA4YWxkTrZu0gW--         
         
         
         
         */

        /*
byte[] image = null;

var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-HockeyAppToken","a2998cd25c374d648d14289343e49cf8");
using (var r = await client.GetAsync("https://rink.hockeyapp.net/api/2/apps/31a38efd87504c11b0d48f2980b5ab30/crash_reasons"))
{
    string res = await r.Content.ReadAsStringAsync();
}
*/

    }
}


