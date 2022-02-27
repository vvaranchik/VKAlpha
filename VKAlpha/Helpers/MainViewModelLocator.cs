using System;
using MonoVKLib.VK;
using MonoSpotifyLib.Spotify;
using VKAlpha.BASS;

namespace VKAlpha.Helpers
{
    public class MainViewModelLocator
    {
        private static readonly Properties.Settings _settings = new Properties.Settings();
        private static readonly VK _vk = new VK("2274003", "hHbZxrka2uZ6jB1inYsH");
        private static readonly Spotify _spotify = new Spotify(_settings.sptoken, _settings.spexpire);
        private static readonly BassAudioPlayer _bass = new BassAudioPlayer();
        private static readonly PlaylistControl _pc = new PlaylistControl();
        private static readonly Lazy<Dialogs.WindowDialogs> _d = new Lazy<Dialogs.WindowDialogs>(() => new Dialogs.WindowDialogs());
        private static readonly Lazy<MainViewModel> _mvm = new Lazy<MainViewModel>(() => new MainViewModel());

        internal static Properties.Settings Settings => _settings;

        public static MainViewModel MainViewModel => _mvm.Value;

        public static VK Vk => _vk;

        public static Spotify SpotifyHelper => _spotify;

        public static Dialogs.WindowDialogs WindowDialogs => _d.Value;

        public static PlaylistControl PlaylistControl => _pc;

        public static BassAudioPlayer BassPlayer => _bass;

        public static Resources.Lang.LangModel AppLang { get; set; } = new Resources.Lang.LangModel();

        public static Resources.Themes.ThemeModel Theme { get; set; } = new Resources.Themes.ThemeModel();
    }
}
