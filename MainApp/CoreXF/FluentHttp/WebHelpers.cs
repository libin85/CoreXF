using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreXF
{
    public static class WebHelpers
    {

        // http://stackoverflow.com/questions/21169573/how-to-implement-progress-reporting-for-portable-httpclient
        // await DownloadFileAsync("http://www.dotpdn.com/files/Paint.NET.3.5.11.Install.zip", progress, cancellationToken.Token);
        public static async Task DownloadFileAsync(string url, Action<int> progressAction, IFile file, CancellationToken token)
        {
            using (HttpClient client = new HttpClient())
            {

                var accessToken = await AuthManager.GetAccessTokenIfNeeded(token);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Add("client_id", NaumenConfig.AuthClientId);
                client.DefaultRequestHeaders.Add("client_secret", NaumenConfig.AuthClientSecret);

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpException($"The request returned with HTTP status code {response.StatusCode}", response.StatusCode);
                    }

                    long total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;

                    using (Stream
                        stream = await response.Content.ReadAsStreamAsync(),
                        writestream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)
                        )
                    {
                        int totalRead = 0;
                        byte[] buffer = new byte[4096];
                        bool isMoreToRead = true;

                        do
                        {
                            token.ThrowIfCancellationRequested();

                            var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                //var data = new byte[read];
                                //buffer.ToList().CopyTo(0, data, 0, read);

                                // TODO: put here the code to write the file to disk
                                await writestream.WriteAsync(buffer, 0, read);

                                totalRead += read;

                                progressAction?.Invoke(Convert.ToInt32((totalRead * 1d) / (total * 1d) * 100));

                            }
                        } while (isMoreToRead);
                        await writestream.FlushAsync();
                    }
                }
            };
        }

    }
}
