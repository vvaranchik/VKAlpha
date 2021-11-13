using System.Collections.Generic;
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
                return _cover;
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
            {
                collection.Add(new AudioModel()
                {
                    AlbumId = vkAudio.AlbumId,
                    Artist = vkAudio.Artist,
                    Title = vkAudio.Title,
                    Cover = null,
                    ImageByteData = null,
                    Date = vkAudio.Date,
                    Duration = vkAudio.Duration,
                    GenreId = vkAudio.GenreId,
                    Id = vkAudio.Id,
                    LyricsId = vkAudio.LyricsId,
                    OwnerId = vkAudio.OwnerId,
                    Url = vkAudio.Url
                });
            }
            return collection;
        }

        private void GetCover()
        {
            if (requesting || string.IsNullOrEmpty(CoverPath) || Artist == "Artist")
                return;

            requesting = true;

            CoverHelper.RequestCover(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private System.Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
