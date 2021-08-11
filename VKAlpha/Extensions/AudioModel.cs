using System.ComponentModel;
using System.Runtime.CompilerServices;
using VKAlpha.BASS;
using VKAlpha.Helpers;

namespace VKAlpha.Extensions
{
    public class AudioModel : MonoVKLib.VK.Models.VKAudioModel, INotifyPropertyChanged
    {
        private bool _coverRequested;
        private System.Windows.Media.ImageSource _cover = null;
        private byte[] _bitmapImage = null;

        private string CoverPath => System.IO.Path.Combine("Cache", $"covers/{Id}.jpg");

        public static bool operator true(AudioModel model)
        {
            return model != null || model != default;
        }
        public static bool operator false(AudioModel model)
        {
            return model == null || model == default;
        }


        public byte[] ImageByteData 
        {
            get
            {
                return _bitmapImage;
            }
            set
            {
                if (_bitmapImage == value)
                    return;
                _bitmapImage = value;
                OnPropertyChanged();
            }
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
            set
            {
                if (_cover == value)
                    return;
                _cover = value;
                OnPropertyChanged();
            }
        }

        public static AudioModel VKModelToAudio(MonoVKLib.VK.Models.VKAudioModel vk)
        {
            return new AudioModel()
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
        }

        bool requesting = false;

        private void GetCover()
        {
            if (requesting || string.IsNullOrEmpty(CoverPath) || Artist == "Artist")
                return;

            requesting = true;

            CoverHelper.RequestCover(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
