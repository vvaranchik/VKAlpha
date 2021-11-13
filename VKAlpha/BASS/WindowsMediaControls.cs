using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Threading;
using VKAlpha.Extensions;
using VKAlpha.Helpers;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace VKAlpha.BASS
{
    [Obsolete]
    public static class WindowsMediaControls
    {
        private static SystemMediaTransportControls systemMediaControls;
        private static SystemMediaTransportControlsDisplayUpdater displayUpdater;
        private static MusicDisplayProperties musicProperties;
        private static InMemoryRandomAccessStream artworkStream;

        private static void MediaButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Stop:
                    Dispatcher.CurrentDispatcher.Invoke(() => MainViewModelLocator.BassPlayer.Stop());
                    break;
                case SystemMediaTransportControlsButton.Play:
                case SystemMediaTransportControlsButton.Pause:
                   Dispatcher.CurrentDispatcher.Invoke(() => MainViewModelLocator.BassPlayer.PauseResume(true));
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Dispatcher.CurrentDispatcher.Invoke(() => MainViewModelLocator.BassPlayer.Next());
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Dispatcher.CurrentDispatcher.Invoke(() => MainViewModelLocator.BassPlayer.Prev());
                    break;
            }
        }

        private static void PlaybackPositionChangeRequested(SystemMediaTransportControls sender, PlaybackPositionChangeRequestedEventArgs args) { }

        private static void AutoRepeatModeChangeRequested(SystemMediaTransportControls sender, AutoRepeatModeChangeRequestedEventArgs args) { }

        private static void ShuffleEnabledChangeRequested(SystemMediaTransportControls sender, ShuffleEnabledChangeRequestedEventArgs args) { }

        private static void PlaybackRateChangeRequested(SystemMediaTransportControls sender, PlaybackRateChangeRequestedEventArgs args) { }

        public async static void SetArtworkThumbnail(byte[] data)
        {
            if (artworkStream != null)
            {
                artworkStream.Dispose();
            }
            if (data == null)
            {
                artworkStream = null;
                displayUpdater.Thumbnail = null;
                return;
            }
            artworkStream = new InMemoryRandomAccessStream();
            await artworkStream.WriteAsync(data.AsBuffer());
            displayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromStream(artworkStream);
            displayUpdater.Update();
        }

        public static void ChangeState(MediaPlaybackStatus state)
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
                    systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    systemMediaControls.IsEnabled = false;
                    break;
            }
        }

        public static void RefreshDisplayData(AudioModel audio)
        {
            if (displayUpdater.Type != MediaPlaybackType.Music)
            {
                displayUpdater.ClearAll();
                displayUpdater.Type = MediaPlaybackType.Music;
            }
            if (audio != null)
            {
                musicProperties.AlbumArtist = musicProperties.Artist = audio.Artist;
                musicProperties.AlbumTitle = musicProperties.Title = audio.Title;
            }
            displayUpdater.Update();
        }

        public static void Initalize()
        {
            systemMediaControls = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
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

        public static void Dispose()
        {
            SetArtworkThumbnail(null);
            displayUpdater.ClearAll();
            systemMediaControls = null;
        }
    }
}
