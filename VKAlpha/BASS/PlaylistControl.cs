using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VKAlpha.Extensions;

namespace VKAlpha.BASS
{
    public class PlaylistControl
    {
        public PlaylistControl() { }
        private bool shuffled = false;

        public bool RequstShuffle { get; set; } = false;

        public ObservableCollection<AudioModel> PlayingPlaylist { get; private set; } = new ObservableCollection<AudioModel>();
        
        private void LoadPlayingPlaylist(ICollection<AudioModel> collection)
        {
            PlayingPlaylist.Clear();
            shuffled = false;
            foreach (var audio in collection)
            {
                if (!string.IsNullOrEmpty(audio.Url))
                    PlayingPlaylist.Add(audio);
            }
            ShuffleCheck();
        }

        public void CheckPlaylist(ICollection<AudioModel> collection)
        {
            if (PlayingPlaylist.FirstOrDefault() == default(AudioModel) ||
                collection.First().OwnerId != PlayingPlaylist[0].OwnerId ||
                (collection.First().OwnerId == PlayingPlaylist[0].OwnerId &&
                collection.First().Id != PlayingPlaylist[0].Id))
            {
                LoadPlayingPlaylist(collection);
            }
        }

        public void NullCheckPlaylist(ICollection<AudioModel> collection)
        {
            if (PlayingPlaylist.FirstOrDefault() == default(AudioModel))
            {
                LoadPlayingPlaylist(collection);
            }
        }

        public void ShuffleCheck()
        {
            string data = Helpers.MainViewModelLocator.BassPlayer.CurrentTrack.FullData;
            if (Helpers.MainViewModelLocator.BassPlayer.IsShuffled)
            {
                if (shuffled && !RequstShuffle)
                    return;
                shuffled = true;
                PlayingPlaylist.Shuffle();
            }
            else
            {
                PlayingPlaylist = new ObservableCollection<AudioModel>(PlayingPlaylist.OrderByDescending(d => d.Date));
                shuffled = false;
            }
            RequstShuffle = false;
            Helpers.MainViewModelLocator.BassPlayer.SelectTrack(data);
        }
    }
}
