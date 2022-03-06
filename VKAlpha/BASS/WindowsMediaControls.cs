using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VKAlpha.Extensions;
using VKAlpha.Helpers;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace VKAlpha.BASS
{
    public class WindowsMediaControls : IDisposable
    {
        private SystemMediaTransportControls systemMediaControls;
        private SystemMediaTransportControlsDisplayUpdater displayUpdater;
        private MusicDisplayProperties musicProperties;
        private volatile InMemoryRandomAccessStream artworkStream;

        private void MediaButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Stop:
                    MainViewModelLocator.BassPlayer.Stop();
                    break;
                case SystemMediaTransportControlsButton.Play:
                    MainViewModelLocator.MainViewModel.IsPlaying = true;
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    MainViewModelLocator.MainViewModel.IsPlaying = false;
                    break;
                case SystemMediaTransportControlsButton.Next:
                    MainViewModelLocator.MainViewModel.ToNextTrack();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    MainViewModelLocator.MainViewModel.ToPrevTrack();
                    break;
            }
        }

        private void PlaybackPositionChangeRequested(SystemMediaTransportControls sender, PlaybackPositionChangeRequestedEventArgs args) { }

        private void AutoRepeatModeChangeRequested(SystemMediaTransportControls sender, AutoRepeatModeChangeRequestedEventArgs args) { }

        private void ShuffleEnabledChangeRequested(SystemMediaTransportControls sender, ShuffleEnabledChangeRequestedEventArgs args) { }

        private void PlaybackRateChangeRequested(SystemMediaTransportControls sender, PlaybackRateChangeRequestedEventArgs args) { }

        public async void SetArtworkThumbnail(byte[] data)
        {
            if (data == null)
            {
                artworkStream.Dispose();
                displayUpdater.Update();
                return;
            }
            artworkStream = new InMemoryRandomAccessStream();
            await artworkStream.WriteAsync(data.AsBuffer());
            displayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromStream(artworkStream);
            displayUpdater.Update();
        }

        public void ChangeState(MediaPlaybackStatus state)
        {
            switch (state)
            {
                case MediaPlaybackStatus.Playing:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    systemMediaControls.IsEnabled = true;
                    break;
                case MediaPlaybackStatus.Paused:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaPlaybackStatus.Stopped:
                case MediaPlaybackStatus.Closed:
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    systemMediaControls.IsEnabled = false;
                    break;
            }
        }

        public void RefreshDisplayData(AudioModel audio)
        {
            artworkStream.Dispose();
            displayUpdater.Thumbnail = null;
            if (audio != null)
            {
                musicProperties.AlbumArtist = musicProperties.Artist = audio.Artist;
                musicProperties.AlbumTitle = musicProperties.Title = audio.Title;
            }
            displayUpdater.Update();
        }

        public WindowsMediaControls()
        {
            artworkStream = new InMemoryRandomAccessStream();
            systemMediaControls = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
            systemMediaControls.IsEnabled = false;
            systemMediaControls.IsPlayEnabled = true;
            systemMediaControls.IsPauseEnabled = true;
            systemMediaControls.IsStopEnabled = true;
            systemMediaControls.IsPreviousEnabled = true;
            systemMediaControls.IsNextEnabled = true;
            systemMediaControls.IsRewindEnabled = false;
            systemMediaControls.IsFastForwardEnabled = false;

            systemMediaControls.ButtonPressed += MediaButtonPressed;
            systemMediaControls.PlaybackPositionChangeRequested += PlaybackPositionChangeRequested;
            systemMediaControls.PlaybackRateChangeRequested += PlaybackRateChangeRequested;
            systemMediaControls.ShuffleEnabledChangeRequested += ShuffleEnabledChangeRequested;
            systemMediaControls.AutoRepeatModeChangeRequested += AutoRepeatModeChangeRequested;

            displayUpdater = systemMediaControls.DisplayUpdater;
            displayUpdater.Type = MediaPlaybackType.Music;
            musicProperties = displayUpdater.MusicProperties;
        }

        public void Dispose()
        {
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
            systemMediaControls.ButtonPressed -= MediaButtonPressed;
            systemMediaControls.PlaybackPositionChangeRequested -= PlaybackPositionChangeRequested;
            systemMediaControls.PlaybackRateChangeRequested -= PlaybackRateChangeRequested;
            systemMediaControls.ShuffleEnabledChangeRequested -= ShuffleEnabledChangeRequested;
            systemMediaControls.AutoRepeatModeChangeRequested -= AutoRepeatModeChangeRequested;

            RefreshDisplayData(null);
            displayUpdater.ClearAll();
            systemMediaControls = null;
        }
    }
}
