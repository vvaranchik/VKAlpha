using System;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VKAlpha.Extensions
{
    public static class ImageUrlToImageSource
    {
        public static async System.Threading.Tasks.Task<ImageSource> GetImageSource(this string Url)
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
