using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Un4seen.Bass;
using VKAlpha.Extensions;
using VKAlpha.Helpers;
using Windows.Media;

namespace VKAlpha.BASS
{
    public class BassAudioPlayer : INotifyPropertyChanged
    {
        private AudioModel _current = new AudioModel() { Artist = "Artist", Title = "Title", OwnerId = -1, Duration = TimeSpan.FromSeconds(0) };
        private bool _shuffle = false;
        private bool _repeat = false;
        private bool _playing = false;
        private bool _muted = false;
        private int nowPlaying = 0;
        private int stream = 0;
        private float _vol = MainViewModelLocator.Settings.volume;
        private float _oldVol;
        private bool is_bass_loaded = false;

        private DispatcherTimer _sliderUpdate;

        private readonly SYNCPROC syncOnStreamEnd;
        private readonly SYNCPROC syncOnDeviceChanged;

        private object _isWorking = new object();

        public bool IsShuffled { get => _shuffle; set { this.MutateVerbose(ref _shuffle, value, RaisePropertyChanged()); MainViewModelLocator.PlaylistControl.ShuffleCheck(); } }

        public bool IsRepeated { get => _repeat; set => this.MutateVerbose(ref _repeat, value, RaisePropertyChanged()); }

        public bool IsPlaying { get => _playing; set => this.MutateVerbose(ref _playing, value, RaisePropertyChanged()); }

        public AudioModel CurrentTrack { 
            get => _current;
            private set => this.MutateVerbose(ref _current, value, RaisePropertyChanged());
        }

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
            get => Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream, BASSMode.BASS_POS_BYTE));
            set
            {
                Bass.BASS_ChannelSetPosition(stream, value);
                RaisePropertyChanged();
            }
        }

        public float Volume
        {
            get => _vol;
            set
            {
                if (_vol == value || value > 1.0f || value < 0.0f)
                    return;
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, value);
                MainViewModelLocator.Settings.volume = value;
                if (IsMuted && _vol > 0.0f) IsMuted = false;
                else if (value == 0.0f) IsMuted = true;
                this.MutateVerbose(ref _vol, value, RaisePropertyChanged());
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
            if (!Bass.LoadMe())
            {
                System.Windows.MessageBox.Show("VK Alpha could not locate 'bass.dll' in its working directory. Reinstall app, if error persist - report on git.", "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                throw new DllNotFoundException("VK Alpha could not locate 'bass.dll' in its working directory. Reinstall app, if error persist - report on git.");
            }
            
            syncOnStreamEnd = new SYNCPROC(OnEndStream);
            syncOnDeviceChanged = new SYNCPROC(onDeviceChange);

            _sliderUpdate = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(250) };
            _sliderUpdate.Tick += SliderUpdateTick;
            _sliderUpdate.Start();
            WindowsMediaControls.Initalize();
        }
        
        private bool tryChangeDevice()
        {
            Thread.Sleep(1500);
            return Bass.BASS_SetDevice(1);
        }

        private void onDeviceChange(int handle, int channel, int data, IntPtr user)
        {
            var shouldContinue = IsPlaying;
            if (IsPlaying)
            {
                PauseResume();
            }

            var isDeviceChanged = tryChangeDevice();
            for(var attempts = 5; attempts > 0 && !isDeviceChanged; --attempts)
            {
                isDeviceChanged = tryChangeDevice();
            }
            if (isDeviceChanged)
            {
                if (shouldContinue)
                    PauseResume();
            }
            else 
            {
                Stop();
                MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("No available audio device found!"); 
            }
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
            lock (_isWorking)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING) Bass.BASS_ChannelStop(stream);
                if (_playing) _playing = false;
                if (stream != 0) Bass.BASS_StreamFree(stream);
            }
            WindowsMediaControls.ChangeState(MediaPlaybackStatus.Stopped);
            if (!dispose)
                return;
            MainViewModelLocator.Discord?.destroy();
            WindowsMediaControls.Dispose();
            _sliderUpdate.Stop();
            _sliderUpdate.Tick -= SliderUpdateTick;
            Bass.BASS_Free();
            Bass.FreeMe();
        }

        public void SelectTrack(string data)
        {
            foreach (var track in MainViewModelLocator.PlaylistControl.PlayingPlaylist)
            {
                if (track.FullData != data)
                    continue;
                nowPlaying = MainViewModelLocator.PlaylistControl.PlayingPlaylist.IndexOf(track);
                break;
            }
        }

        public void PlaySelected(int trackindex, bool skipPlaylistCheck = false)
        {
            Stop();
            if (nowPlaying != trackindex) nowPlaying = trackindex;
            Play(skipPlaylistCheck);
        }

        public void FindAndPlay(string fulldata)
        {
            Task.Run(() => SelectTrack(fulldata)).Wait();
            PlaySelected(nowPlaying, true);
        }

        private void Play(bool skipPlaylistCheck = false)
        {
            lock (_isWorking)
            {
                if (!is_bass_loaded)
                {
                    Bass.BASS_Init(-1, 48000, BASSInit.BASS_DEVICE_STEREO, MainWindow.Handle);
                    var plugin_load = Bass.BASS_PluginLoad("basshls.dll");
                    switch ((BASSError)plugin_load)
                    {
                        case BASSError.BASS_ERROR_FILEOPEN:
                            System.Windows.MessageBox.Show("Cant open file \'basshls.dll\'");
                            throw new DllNotFoundException("Cant open file \'basshls.dll\'");
                        case BASSError.BASS_ERROR_FILEFORM:
                            System.Windows.MessageBox.Show("File \'basshls.dll\' is not a plugin file");
                            throw new AggregateException("File \'basshls.dll\' is not a plugin file");
                    }
                    is_bass_loaded = true;
                }
                    
                if (!skipPlaylistCheck)
                {
                    dynamic vm = ((App.Current.MainWindow as MainWindow).FrameMain.Content as System.Windows.Controls.UserControl).DataContext;
                    App.Current.Dispatcher.Invoke(() => MainViewModelLocator.PlaylistControl.CheckPlaylist(vm.collection));
                }

                stream = Bass.BASS_StreamCreateURL(MainViewModelLocator.PlaylistControl.PlayingPlaylist[nowPlaying].Url, 0, BASSFlag.BASS_STREAM_PRESCAN, null, MainWindow.Handle);
                if (stream == 0 && Bass.BASS_ErrorGetCode() == BASSError.BASS_ERROR_NONET)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("No Internet Connection Available");
                    return;
                }
                
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, syncOnStreamEnd, MainWindow.Handle);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_DEV_FAIL, 0, syncOnDeviceChanged, MainWindow.Handle);
                Bass.BASS_ChannelPlay(stream, false);
                CurrentTrack = MainViewModelLocator.PlaylistControl.PlayingPlaylist[nowPlaying];
                if (!IsPlaying)
                {
                    IsPlaying = true;
                }
                WindowsMediaControls.RefreshDisplayData(CurrentTrack);
                WindowsMediaControls.ChangeState(MediaPlaybackStatus.Playing);
                MainViewModelLocator.Discord?.updatePresence(CurrentTrack.Artist, CurrentTrack.Title, CurrentTrack.Duration);
            }
        }

        public void PauseResume(bool notify = false)
        {
            lock (_isWorking)
            {
                _playing = !_playing;
                if (!_playing)
                {
                    WindowsMediaControls.ChangeState(MediaPlaybackStatus.Playing);
                    Bass.BASS_ChannelPlay(stream, false);
                    MainViewModelLocator.Discord?.updatePresence(CurrentTrack.Artist, CurrentTrack.Title, TimeSpan.FromSeconds(CurrentTrack.Duration.TotalSeconds - SliderValue));
                }
                else
                {
                    WindowsMediaControls.ChangeState(MediaPlaybackStatus.Paused);
                    Bass.BASS_ChannelPause(stream);
                    MainViewModelLocator.Discord?.setDefaultPresence();
                }
                if (notify) OnPropChanged(nameof(IsPlaying));
            }
        }

        public void Next()
        {
            lock (_isWorking)
            {
                if (++nowPlaying >= MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count)
                    nowPlaying = 0;
                PlaySelected(nowPlaying, true);
            }
        }

        public void Prev()
        {
            lock (_isWorking)
            {
                if (--nowPlaying < 0)
                    nowPlaying = MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count - 1;
                PlaySelected(nowPlaying, true);
            }
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

        private void OnPropChanged([CallerMemberName] string name = null) => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
