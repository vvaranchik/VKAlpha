using System;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace VKAlpha.Extensions
{
    public static class ImageUrlToImageSource
    {

        public static byte[] ToByteArray(this BitmapSource imageSource)
        {
            byte[] bytes = null;

            if (imageSource != null)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.GetBuffer();
                }
            }

            return bytes;
        }

        public static async System.Threading.Tasks.Task<BitmapImage> GetImageSource(this string Url)
        {
            var imageData = await new WebClient().DownloadDataTaskAsync(new Uri(Url, UriKind.RelativeOrAbsolute));

            var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageData);
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
