using System;
using MonoVKLib.VK;
using VKAlpha.BASS;
using VKAlpha.Views;

namespace VKAlpha.Helpers
{
    public class _Locator
    {
        private static readonly Properties.Settings _settings = new Properties.Settings();
        private static readonly VK _vk = new VK("2274003", "hHbZxrka2uZ6jB1inYsH");
        private static readonly BassAudioPlayer _bass = new BassAudioPlayer();
        private static readonly PlaylistControl _pc = new PlaylistControl();
        private static readonly Dialogs.WindowDialogs _d = new Dialogs.WindowDialogs();

        internal static Properties.Settings Settings => _settings;

        public static VK Vk => _vk;

        public static Dialogs.WindowDialogs WindowDialogs => _d;

        public static PlaylistControl PlaylistControl => _pc;

        public static BassAudioPlayer BassPlayer => _bass;

        public static Resources.Lang.LangModel AppLang { get; set; } = new Resources.Lang.LangModel();

        public _Locator() { }
    }
}
