using System.Collections.ObjectModel;
using System.ComponentModel;
using VKAlpha.BASS;
using VKAlpha.Helpers;

namespace VKAlpha.Extensions
{
    public class AudioModel : MonoVKLib.VK.Models.VKAudioModel, INotifyPropertyChanged
    {
        private bool _coverRequested;
        private System.Windows.Media.ImageSource _cover = null;
        private byte[] _bitmapImage = null;
        bool requesting = false;

        private string CoverPath => System.IO.Path.Combine("Cache", $"covers/{Id}.jpg");

        public byte[] ImageByteData
        {
            get => _bitmapImage;
            set => this.MutateVerbose(ref _bitmapImage, value, RaisePropertyChanged());
        }

        private System.Windows.Media.ImageSource GetCoverFromBytes()
        {
            if (ImageByteData == null)
                return null;
            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new System.IO.MemoryStream(ImageByteData);
            bi.EndInit();
            return bi;
        }

        public System.Windows.Media.ImageSource Cover
        {
            get
            {
                if (!_coverRequested)
                {
                    _coverRequested = true;
                    GetCover();
                }
                WindowsMediaControls.SetArtworkThumbnail(ImageByteData);
                return _cover ?? GetCoverFromBytes();
            }
            set => this.MutateVerbose(ref _cover, value, RaisePropertyChanged());
        }

        public static AudioModel VKModelToAudio(MonoVKLib.VK.Models.VKAudioModel vk)
            => new AudioModel()
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
            if (!MainViewModelLocator.Settings.load_track_covers || requesting || string.IsNullOrEmpty(CoverPath) || Artist == "Artist")
                return;

            requesting = true;
            CoverHelper.RequestCover(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private System.Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
