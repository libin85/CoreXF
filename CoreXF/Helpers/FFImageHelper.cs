
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using FFImageLoading.Work;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;

namespace CoreXF
{
    public class FFImageHelper
    {

        // Sample
        //
        // string filePath = await FFImageHelper.AddFileToCache("http://hello.com/12.jpg", mediaFile.Path);
        // ImageSrc = ImageSource.FromUri(new Uri("http://hello.com/12.jpg"));
        //
        public static async Task<string> AddFileToCache(string Url, string FileSource, TimeSpan Duration = default(TimeSpan))
        {
            Configuration config = ImageService.Instance.Config;
            string key = config.MD5Helper.MD5(Url);

            if (!await config.DiskCache.ExistsAsync(key))
            {
                await config.DiskCache.RemoveAsync(key);
            }

            // Add a dummy to cache
            byte[] img = { 1, 3, 5 };
            await config.DiskCache.AddToSavingQueueIfNotExistsAsync(key, img, Duration == default(TimeSpan) ? TimeSpan.FromDays(1) : Duration);

            await Task.Delay(100);

            // Copy file to cache
            string filePath = await config.DiskCache.GetFilePathAsync(key);

            File.Copy(FileSource, filePath, overwrite: true);

            return filePath;
        }

        public static Task InvalidateCache(string key)
        {
            return ImageService.Instance.InvalidateCacheEntryAsync(key, CacheType.All);
        }

        public static async Task<string> GetFilePath(string Url)
        {
            Configuration config = ImageService.Instance.Config;
            string key = config.MD5Helper.MD5(Url);

            string filePath = await config.DiskCache.GetFilePathAsync(key);
            return filePath;
        }

        // Sample
        //
        //  string url = "https://visualstudio.microsoft.com/wp-content/uploads/2016/05/ExcitingBenefits_636x300-1-600x283.png";
        //  FFImageLoading.Cache.CacheStream stream = await FFImageHelper.Download(url, FileWriteFinished: info =>
        //  {
        //      ImageSrc = ImageSource.FromFile(info.FilePath);
        //  });
        //  stream.ImageStream.Dispose();
        //
        public static async Task Download(string Url, Action<FileWriteInfo> FileWriteFinished = null)
        {
            Configuration config = ImageService.Instance.Config;
            string key = config.MD5Helper.MD5(Url);

            // If file has been downloaded previously
            string filePath = await config.DiskCache.GetFilePathAsync(key);
            if (filePath.NotNullAndEmpty())
            {
                FileWriteFinished.Invoke(new FileWriteInfo(filePath, Url));
                return;
            }

            var param = TaskParameter.FromUrl(Url);

            if (FileWriteFinished != null)
            {
                // Fired only when file downloaded first time
                param.FileWriteFinished(async fileWriteInfo =>
                {
                    filePath = await config.DiskCache.GetFilePathAsync(key);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FileWriteFinished(new FileWriteInfo(filePath, key));
                    });

                });
            }

            CacheStream stream = await config.DownloadCache.DownloadAndCacheIfNeededAsync(Url, param, config, CancellationToken.None);
            stream.ImageStream?.Dispose();
        }

        public static Task<bool> Exists(string Url)
        {
            var config = ImageService.Instance.Config;
            var key = config.MD5Helper.MD5(Url);

            return config.DiskCache.ExistsAsync(key);
        }
    }
}
