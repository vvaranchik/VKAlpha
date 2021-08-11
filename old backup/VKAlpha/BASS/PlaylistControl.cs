using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VKAlpha.Extensions;

namespace VKAlpha.BASS
{
    public class PlaylistControl
    {
        private bool shuffled = false;

        public ObservableCollection<MonoVKLib.VK.Models.VKAudioModel> PlayingPlaylist { get; private set; }

        public PlaylistControl()
        {
            PlayingPlaylist = new ObservableCollection<MonoVKLib.VK.Models.VKAudioModel>();
        }
        
        private void LoadPlayingPlaylist(ObservableCollection<MonoVKLib.VK.Models.VKAudioModel> collection)
        {
            PlayingPlaylist.Clear();
            foreach (var audio in collection)
            {
                if (!string.IsNullOrEmpty(audio.Url))
                    PlayingPlaylist.Add(audio);
            }
        }

        public void CheckPlaylist(ObservableCollection<MonoVKLib.VK.Models.VKAudioModel> collection)
        {
            if (PlayingPlaylist.FirstOrDefault() == default(MonoVKLib.VK.Models.VKAudioModel))
            {
                LoadPlayingPlaylist(collection);
                return;
            }
            if (collection[0].OwnerId != PlayingPlaylist[0].OwnerId)
            {
                LoadPlayingPlaylist(collection);
            }
        }

        public void ShuffleCheck()
        {
            string data = Helpers.MainViewModelLocator.BassPlayer.CurrentTrack.FullData;
            if (Helpers.MainViewModelLocator.BassPlayer.IsShuffled)
            {
                if (shuffled)
                    return;
                PlayingPlaylist.Shuffle();
                shuffled = true;
            }
            else
            {
                PlayingPlaylist = new ObservableCollection<MonoVKLib.VK.Models.VKAudioModel>(PlayingPlaylist.OrderByDescending(d => d.Date));
                shuffled = false;
            }
            Helpers.MainViewModelLocator.BassPlayer.SelectTrack(data);
        }
    }
}
