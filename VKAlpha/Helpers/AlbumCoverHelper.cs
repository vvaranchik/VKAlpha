using System.IO;
using System.Threading;
using VKAlpha.Extensions;

namespace VKAlpha.Helpers
{
    public static class CoverHelper
    {
        private static string path;
        private static CancellationTokenSource token = new CancellationTokenSource();
        private static bool isRequesting = false;

        public static void CancelCover()
        {
            isRequesting = false;
            token.Cancel();
            token = new CancellationTokenSource();
        }

        public static void RequestCover(AudioModel model)
        {
            if (isRequesting)
            {
                CancelCover();
            }
            isRequesting = true;
            Process(token.Token, model);
        }

        private async static void Process(CancellationToken token, AudioModel model) 
        {
            if (token.IsCancellationRequested)
            {
                CancelCover();
                return;
            }

            if (!Directory.Exists("./Cache") || !Directory.Exists("./Cache/covers"))
            {
                Directory.CreateDirectory("./Cache/covers/");
            }

            path = Path.Combine("Cache", $"covers/{model.Id}.jpg");

            if (token.IsCancellationRequested)
            {
                CancelCover();
                return;
            }

            if (File.Exists(path))
            {
                model.Cover = await CacheService.GetCachedImage(path, model);
                isRequesting = false;
                return;
            }

            var imageUri = await MainViewModelLocator.SpotifyHelper.Covers.GetAlbumCover(model.Artist, model.Title);
            if (imageUri != null)
            {
                if (token.IsCancellationRequested)
                {
                    CancelCover();
                    return;
                }
                model.Cover = await CacheService.SetCover(imageUri.GetResult(), path, model);
                isRequesting = false;
            }
        }
    }
}
