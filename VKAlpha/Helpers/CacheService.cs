using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VKAlpha.Extensions;
using Debug = System.Diagnostics.Trace;

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
                //BASS.WindowsMediaControls.SetArtworkThumbnail(model.ImageByteData);
                return bi;
            }

            BASS.WindowsMediaControls.SetArtworkThumbnail(null);
            return null;
        }

        public static async Task<ImageSource> SetCover(string url, string path, AudioModel model)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                using (var stream = await new HttpClient().GetStreamAsync(url))
                {
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        await SaveStream(ms, path);
                    }
                }

                return await GetCachedImage(path, model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

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

        private static Task SaveStream(Stream stream, string fileName)
        {
            using (var fileStream = File.OpenWrite(fileName))
            {
                var buffer = new byte[1024];

                while (stream.Read(buffer, 0, buffer.Length) > 0)
                {
                    fileStream.Write(buffer, 0, buffer.Length);
                }

                fileStream.Flush();
            }
            return Task.CompletedTask;
        }
    }
}
