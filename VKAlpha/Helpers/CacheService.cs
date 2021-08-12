using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using VKAlpha.Extensions;

namespace VKAlpha.Helpers
{
    public static class CacheService
    {
        public static async Task<ImageSource> GetCachedImage(string path, AudioModel model)
        {
            if (File.Exists(path))
            {
                var bi = new System.Windows.Media.Imaging.BitmapImage();
                var ms = new MemoryStream();
                using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    ms.SetLength(stream.Length);
                    await stream.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    ms.Flush();
                    stream.Flush();
                }
                model.ImageByteData = ms.GetBuffer();
                return bi;
            }

            BASS.WindowsMediaControls.SetArtworkThumbnail(null);
            return null;
        }

        public static async Task<ImageSource> SetCover(string url, string path, AudioModel model)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            using(var writer = File.OpenWrite(path))
            {
                var client = new RestClient(new Uri(url));
                var request = new RestRequest(Method.GET);
                request.ResponseWriter = stream => { using (stream) { stream.CopyTo(writer); } }; 
                client.DownloadData(request);
                await writer.FlushAsync();
            }

            //just to make sure
            if (File.Exists(path))
                return await GetCachedImage(path, model);

            return null;
        }

        public static string GetSafeFileName(string input)
        {
            var fileName = input;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }

            return fileName;
        }
    }
}
