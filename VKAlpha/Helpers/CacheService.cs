using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using VKAlpha.Extensions;

namespace VKAlpha.Helpers
{
    public static class CacheService
    {
        public static async Task<System.Windows.Media.Imaging.BitmapImage> GetCachedImage(string path, AudioModel model = null)
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
                }
                return bi;
            }
            return null;
        }

        public static async void SetCover(string url, string path, AudioModel model)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
                return;
            System.Windows.Media.Imaging.BitmapImage image = null;
            if (MainViewModelLocator.Settings.load_track_covers)
            {
                if (!File.Exists(path))
                {
                    if (model.ImageByteData != null)
                    {
                        image = new System.Windows.Media.Imaging.BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(model.ImageByteData);
                        image.EndInit();
                    }
                    else
                    {
                        image = await url.GetImageSource();
                        var bytes = image.ToByteArray();

                        if (MainViewModelLocator.Settings.cache_track_covers)
                        {
                            using (var writer = File.OpenWrite(path))
                            {
                                await writer.WriteAsync(bytes, 0, bytes.Length);
                                await writer.FlushAsync();
                            }
                        }
                        model.ImageByteData = bytes;
                    }
                    model.Cover = image;
                }
                else 
                    model.Cover = await GetCachedImage(path, model);
            }
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
