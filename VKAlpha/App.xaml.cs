using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Newtonsoft.Json;
using VKAlpha.Helpers;

namespace VKAlpha
{
    public partial class App : Application
    {
        private bool LoadLang(string lang)
        {
            bool result = false;
            Task.Run(() =>
            {
                if (!File.Exists($"./Resources/Lang/{lang}.json"))
                    return false;
                try
                {
                    var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
                    MainViewModelLocator.AppLang = JsonConvert.DeserializeObject<Resources.Lang.LangModel>(File.ReadAllText($"./Resources/Lang/{lang}.json"), settings);
                }
                catch (JsonSerializationException)
                {
                    return false;
                }
                return true;
            }).ContinueWith(tsk =>
            {
                result = tsk.Result;
            }).Wait();
            return result;
        }

        private bool LoadTheme(string theme)
        {
            if (!File.Exists($"./Resources/Themes/{theme}.json"))
                return false;
            try
            {
                var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
                MainViewModelLocator.Theme = JsonConvert.DeserializeObject<Resources.Themes.ThemeModel>(File.ReadAllText($"./Resources/Themes/{theme}.json"), settings);
            }
            catch (JsonSerializationException)
            {
                return false;
            }
            return true;
        }

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 0xE0000;

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!LoadLang(MainViewModelLocator.Settings.lang))
                if (!LoadLang("en")) throw new FileNotFoundException("Language file not found or corrupted!");

            if (!LoadTheme(MainViewModelLocator.Settings.theme))
                if (!LoadTheme("default"))
                    throw new FileNotFoundException("Theme file not found or corrupted!");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainViewModelLocator.BassPlayer.Stop(true);
            MainViewModelLocator.Settings.spexpire = MainViewModelLocator.SpotifyHelper.AccessToken.ExpireTime;
            MainViewModelLocator.Settings.sptoken = MainViewModelLocator.SpotifyHelper.AccessToken.Token;
            MainViewModelLocator.Settings.Save();
            base.OnExit(e);
        }
    }
}
