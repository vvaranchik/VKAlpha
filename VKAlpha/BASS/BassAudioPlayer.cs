using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows.Threading;
using Un4seen.Bass;
using VKAlpha.Extensions;
using VKAlpha.Helpers;
using Windows.Media;

namespace VKAlpha.BASS
{
    public class BassAudioPlayer : INotifyPropertyChanged
    {
        private bool _repeat = false;
        private bool _muted = false;
        private int stream = 0;
        private float _vol = MainViewModelLocator.Settings.volume;
        private float _oldVol;

        private DispatcherTimer _sliderUpdate;

        private readonly SYNCPROC syncOnStreamEnd;

        private object _isWorking = new object();

        public bool IsRepeated { get => _repeat; set => this.MutateVerbose(ref _repeat, value, RaisePropertyChanged()); }

        public bool IsMuted
        {
            get => _muted;
            set
            {
                this.MutateVerbose(ref _muted, value, RaisePropertyChanged());
                if (_vol > 0.0f && _muted)
                {
                    _oldVol = _vol;
                    Volume = 0;
                }
                else if (_vol == 0.0f && !_muted)
                {
                    Volume = _oldVol;
                    _oldVol = 0;
                }
            }
        }

        public double SliderValue
        {
            get => Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream, BASSMode.BASS_POS_BYTE));
            set
            {
                Bass.BASS_ChannelSetPosition(stream, value);
                OnPropChanged();
            }
        }

        public float Volume
        {
            get => _vol;
            set
            {
                if (_vol == value || value > 1.0f || value < 0.0f)
                    return;
                this.MutateVerbose(ref _vol, value, RaisePropertyChanged());
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, _vol);
                MainViewModelLocator.Settings.volume = _vol;
                if (IsMuted && _vol > 0.0f) IsMuted = false;
                else if (!IsMuted && _vol <= 0.0f) IsMuted = true;
            }
        }

        private void SliderUpdateTick(object sender, EventArgs args) => OnPropChanged(nameof(SliderValue));
        

        public BassAudioPlayer()
        {
            string err_bass_dll = "VK Alpha could not locate 'bass.dll' in its working directory. Reinstall app, if error persist - report on git.";
            string err_bass_plugin_not_found = "Cant open file \'basshls.dll\'";
            string err_bass_plugin = "File \'basshls.dll\' is not a plugin file";

            if (!Bass.LoadMe())
            {
                App.RaiseException(err_bass_dll);
            }

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_DEV_DEFAULT, true);
            Bass.BASS_Init(-1, 48000, BASSInit.BASS_DEVICE_STEREO, MainWindow.Handle);
            var plugin_load = Bass.BASS_PluginLoad("basshls.dll");
            switch ((BASSError)plugin_load)
            {
                case BASSError.BASS_ERROR_FILEOPEN:
                    App.RaiseException(err_bass_plugin_not_found);
                    break;
                case BASSError.BASS_ERROR_FILEFORM:
                    App.RaiseException(err_bass_plugin);
                    break;
            }

            syncOnStreamEnd = new SYNCPROC(OnEndStream);

            _sliderUpdate = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(250) };
            _sliderUpdate.Tick += SliderUpdateTick;
            _sliderUpdate.Start();
        }

        private void OnEndStream(int handle, int channel, int data, IntPtr user)
        {
            if (!IsRepeated)
            {
                MainViewModelLocator.MainViewModel.ToNextTrack();
                return;
            }
            Bass.BASS_ChannelSetPosition(stream, 0.0);
            Bass.BASS_ChannelPlay(stream, false);
        }

        public void Unload()
        {
            lock (_isWorking)
            {
                Stop();
                MainViewModelLocator.SMCProvider.Dispose();
                _sliderUpdate.Stop();
                _sliderUpdate.Tick -= SliderUpdateTick;
                Bass.BASS_Free();
                Bass.FreeMe();
            }
        }

        public void Stop()
        {
            lock (_isWorking)
            {
                var state = Bass.BASS_ChannelIsActive(stream);
                if (state == BASSActive.BASS_ACTIVE_PLAYING ||
                    state == BASSActive.BASS_ACTIVE_STALLED ||
                    state == BASSActive.BASS_ACTIVE_PAUSED)
                {
                    Bass.BASS_ChannelStop(stream);
                    Bass.BASS_StreamFree(stream);
                }
            }
        }

        public void Load(string url)
        {
            lock (_isWorking)
            {
                stream = Bass.BASS_StreamCreateURL(url, 0, BASSFlag.BASS_STREAM_PRESCAN, null, MainWindow.Handle);
                if (stream == 0 && Bass.BASS_ErrorGetCode() == BASSError.BASS_ERROR_NONET)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("No internet connection available");
                    return;
                }
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, Volume);
                Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, syncOnStreamEnd, MainWindow.Handle);
            }
        }

        public void Play()
        {
            lock (_isWorking)
            {
                var state = Bass.BASS_ChannelIsActive(stream);
                if (state == BASSActive.BASS_ACTIVE_PLAYING || stream == 0)
                    return;
                Bass.BASS_ChannelPlay(stream, false);
            }
        }

        public void PauseResume(bool playing)
        {
            lock (_isWorking)
            {
                var state = Bass.BASS_ChannelIsActive(stream);
                if (playing && state != BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelPlay(stream, false);
                }
                else if (!playing && state == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelPause(stream);
                }
            }
        }

        public void MuteRestoreVol()
        {
            if (IsMuted)
            {
                _oldVol = Volume;
                Volume = 0.0f;
            }
            else
            {
                Volume = _oldVol;
                _oldVol = 0.0f;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);

        private void OnPropChanged([CallerMemberName] string name = null) => RaisePropertyChanged().Invoke(new PropertyChangedEventArgs(name));
    }
}
