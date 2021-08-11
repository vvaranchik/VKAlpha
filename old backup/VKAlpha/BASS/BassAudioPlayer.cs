using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Un4seen.Bass;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.BASS
{
    public class BassAudioPlayer : INotifyPropertyChanged
    {
        private MonoVKLib.VK.Models.VKAudioModel _current;
        private bool _shuffle = false;
        private bool _repeat = false;
        private bool _playing = false;
        private bool _muted = false;
        private int nowPlaying = 0;
        private int stream = 0;
        private float _vol = MainViewModelLocator.Settings.volume;
        private float _oldVol;

        private double _sliderValue = 0;
        private DispatcherTimer _sliderUpdate;
        private object lockRoot = new object();

        private SYNCPROC syncOnStreamEnd;

        public bool IsShuffled { get => _shuffle; set { this.MutateVerbose(ref _shuffle, value, RaisePropertyChanged()); MainViewModelLocator.PlaylistControl.ShuffleCheck(); } }

        public bool IsRepeated { get => _repeat; set => this.MutateVerbose(ref _repeat, value, RaisePropertyChanged()); }

        public bool IsPlaying { get => _playing; set => this.MutateVerbose(ref _playing, value, RaisePropertyChanged()); }

        public MonoVKLib.VK.Models.VKAudioModel CurrentTrack { get => _current; set => this.MutateVerbose(ref _current, value, RaisePropertyChanged()); }

        public int CurrentTrackIndex => nowPlaying;

        public bool IsMuted
        {
            get => _muted;
            set
            {
                if (_muted == value)
                    return;
                this.MutateVerbose(ref _muted, value, RaisePropertyChanged());
                MuteRestoreVol();
            }
        }

        public double SliderValue
        {
            get => Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));
            set
            {
                Bass.BASS_ChannelSetPosition(stream, value);
                this.MutateVerbose(ref _sliderValue, value, RaisePropertyChanged());
            }
        }

        public float Volume
        {
            get => _vol;
            set
            {
                if (_vol == value)
                    return;
                this.MutateVerbose(ref _vol, value, RaisePropertyChanged());
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, value);
                MainViewModelLocator.Settings.volume = _vol;
                if (IsMuted && _vol > 0) IsMuted = false;
                MainViewModelLocator.Settings.Save();
            }
        }

        private void SliderUpdateTick(object sender, EventArgs args)
        {
            //   if (stream == 0)
            //       return;
            //   SliderValue = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));
            OnPropChanged(nameof(SliderValue));
        }

        public BassAudioPlayer()
        {
            _current = new MonoVKLib.VK.Models.VKAudioModel() { Artist = "Artist", Title = "Title", OwnerId = -1, Duration = TimeSpan.FromSeconds(0) };
            Bass.LoadMe();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_STEREO, App.MainWindowHandle);
            syncOnStreamEnd = new SYNCPROC(OnEndStream);

            _sliderUpdate = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            _sliderUpdate.Tick += SliderUpdateTick;
            _sliderUpdate.Start();
        }

        private void OnEndStream(int handle, int channel, int data, IntPtr user)
        {
            if (!IsRepeated)
            {
                Next();
                return;
            }
            Bass.BASS_ChannelSetPosition(stream, 0.0);
            Bass.BASS_ChannelPlay(stream, false);
        }

        public void Stop(bool dispose = false)
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING) Bass.BASS_ChannelStop(stream);
            if (_playing) _playing = false;
            if (stream != 0) Bass.BASS_StreamFree(stream);
            if (!dispose)
                return;
            _sliderUpdate.Stop();
            _sliderUpdate = null;
            Bass.FreeMe();
        }

        public void SelectTrack(string data)
        {
            for (var i = 0; i < MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count; i++)
            {
                if (data != MainViewModelLocator.PlaylistControl.PlayingPlaylist[i].FullData)
                    continue;
                nowPlaying = i;
                break;
            }
        }

        public void PlaySelected(int trackindex)
        {
            Stop();
            if (nowPlaying != trackindex) nowPlaying = trackindex;
            Play();
        }

        public void FindAndPlay(string fulldata)
        {
            Task.Run(() => SelectTrack(fulldata)).Wait();
            PlaySelected(nowPlaying);
        }

        private async void Play()
        {
            await Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(()=> MainViewModelLocator.PlaylistControl.CheckPlaylist((((App.Current.MainWindow as MainWindow).FrameMain.Content as Views.AudiosListView).DataContext as ViewModels.AudiosListViewModel).collection));
                stream = Bass.BASS_StreamCreateURL(MainViewModelLocator.PlaylistControl.PlayingPlaylist[nowPlaying].Url, 0, BASSFlag.BASS_STREAM_PRESCAN, null, App.MainWindowHandle);
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, syncOnStreamEnd, App.MainWindowHandle);
                Bass.BASS_ChannelPlay(stream, false);
            });
            CurrentTrack = MainViewModelLocator.PlaylistControl.PlayingPlaylist[nowPlaying];
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        public void PauseResume()
        {
            _playing = !_playing;
            if (!_playing) Bass.BASS_ChannelPlay(stream, false);
            else Bass.BASS_ChannelPause(stream);
        }

        public void Next()
        {
            nowPlaying++;
            if (nowPlaying >= MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count)
                nowPlaying = 0;
            PlaySelected(nowPlaying);
        }

        public void Prev()
        {
            nowPlaying--;
            if (nowPlaying < 0)
                nowPlaying = MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count - 1;
            PlaySelected(nowPlaying);
        }

        public void MuteRestoreVol()
        {
            if (IsMuted)
            {
                _oldVol = Volume;
                Volume = 0;
            }
            else
            {
                Volume = _oldVol;
                _oldVol = 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        private void OnPropChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
