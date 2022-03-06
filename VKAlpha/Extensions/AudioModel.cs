using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using VKAlpha.Helpers;

namespace VKAlpha.Extensions
{
    public class AudioModel : MonoVKLib.VK.Models.VKAudioModel, INotifyPropertyChanged
    {
        private bool _coverRequested = false;
        private System.Windows.Media.ImageSource _cover = null;
        private byte[] _imageByteData = null;
        private bool requesting = false;

        public static bool IsAudioValid(AudioModel model)
            => model != null && model != default;
        
        private string CoverPath
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                    return null;
                return Path.Combine("Cache", $"covers/{Id}.jpg");
            }
        }

        public byte[] ImageByteData
        {
            get => _imageByteData;
            set
            {
                this.MutateVerbose(ref _imageByteData, value, RaisePropertyChanged());
            }
        }

        private System.Windows.Media.ImageSource GetCoverFromBytes()
        {
            if (ImageByteData == null)
                return null;
            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(ImageByteData);
            bi.EndInit();
            return bi;
        }

        public bool IsPlaying { get; set; } = false;

        public System.Windows.Media.ImageSource Cover
        {
            get
            {
                if (!_coverRequested)
                {
                    _coverRequested = true;
                    GetCover();
                }
                return _cover ?? GetCoverFromBytes();
            }
            set => this.MutateVerbose(ref _cover, value, RaisePropertyChanged());
        }

        public static AudioModel VKModelToAudio(MonoVKLib.VK.Models.VKAudioModel vk)
            => new AudioModel
            {
                AlbumId = vk.AlbumId,
                Artist = vk.Artist,
                Title = vk.Title,
                Cover = null,
                ImageByteData = null,
                Date = vk.Date,
                Duration = vk.Duration,
                GenreId = vk.GenreId,
                Id = vk.Id,
                LyricsId = vk.LyricsId,
                OwnerId = vk.OwnerId,
                Url = vk.Url
            };

        public static Collection<AudioModel> VKArrayToAudioCollection(MonoVKLib.VK.Models.VKItemResponseBase<MonoVKLib.VK.Models.VKAudioModel> vkCollection)
        {
            var collection = new Collection<AudioModel>();
            foreach (MonoVKLib.VK.Models.VKAudioModel vkAudio in vkCollection)
                collection.Add(VKModelToAudio(vkAudio));

            return collection;
        }

        private void GetCover()
        {
            if (!MainViewModelLocator.Settings.load_track_covers || requesting || string.IsNullOrEmpty(CoverPath))
                return;

            requesting = true;
            token.Cancel();
            token = new CancellationTokenSource();
            _GetCover(token.Token);
        }

        private static readonly string[] separators = new[] { ",", "&", "&&", "feat", "feat.", "ft", "ft."/*, "("*/ };

        private static CancellationTokenSource token = new CancellationTokenSource();

        private async void _GetCover(CancellationToken token)
        {
            var path = Path.Combine("Cache", $"covers/{this.Id}.jpg");

            if (File.Exists(path))
            {
                if (token.IsCancellationRequested)
                {
                    requesting = false;
                    _coverRequested = false;
                    return;
                }
                this.Cover = await CacheService.GetCachedImage(path);
            }

            var imageUri = await MainViewModelLocator.SpotifyHelper.Covers.GetAlbumCover(
                 this.Artist
                    .Replace(new[] { "[", "]" })
                    .Split(separators, System.StringSplitOptions.RemoveEmptyEntries)[0],
                this.Title);
            if (token.IsCancellationRequested)
            {
                requesting = false;
                _coverRequested = false;
                return;
            }
            if (!string.IsNullOrEmpty(imageUri))
            {
                if (MainViewModelLocator.Settings.load_track_covers)
                {
                    System.Windows.Media.Imaging.BitmapImage image;
                    if (!File.Exists(path))
                    {
                        image = await imageUri.GetImageSource();
                        if (token.IsCancellationRequested)
                        {
                            requesting = false;
                            _coverRequested = false;
                            return;
                        }
                        var bytes = image.ToByteArray();

                        if (MainViewModelLocator.Settings.cache_track_covers)
                        {
                            if (token.IsCancellationRequested)
                            {
                                requesting = false;
                                _coverRequested = false;
                                return;
                            }
                            using (var writer = File.OpenWrite(path))
                            {
                                await writer.WriteAsync(bytes, 0, bytes.Length);
                                await writer.FlushAsync();
                            }
                        }
                        if (token.IsCancellationRequested)
                        {
                            requesting = false;
                            _coverRequested = false;
                            return;
                        }
                        this.Cover = image;
                        this.ImageByteData = bytes;
                        MainViewModelLocator.SMCProvider.SetArtworkThumbnail(this.ImageByteData);
                    }
                    else
                    {
                        if (token.IsCancellationRequested)
                        {
                            requesting = false;
                            _coverRequested = false;
                            return;
                        }
                        image = await CacheService.GetCachedImage(path);
                        var bytes = image.ToByteArray();
                        this.ImageByteData = bytes;
                        MainViewModelLocator.SMCProvider.SetArtworkThumbnail(bytes);
                        this.Cover = image;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private System.Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
