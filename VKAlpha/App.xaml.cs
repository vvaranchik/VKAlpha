using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using VKAlpha.Helpers;

namespace VKAlpha
{
    public partial class App : Application
    {

        /// <param name="lang">ISO 639-1 lang code</param>
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

        public static void RaiseException(string err_msg, string caption = "ERROR")
        {
            MessageBox.Show(err_msg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            throw new System.Exception(err_msg);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!LoadLang(MainViewModelLocator.Settings.lang))
            {
                if (!LoadLang("en"))
                {
                    RaiseException("The current installation of VK Alpha is broken. Please, reinstall the app.");
                }
            }
            if (!LoadTheme(MainViewModelLocator.Settings.theme))
            {
                if (!LoadTheme("default"))
                {
                    RaiseException("The current installation of VK Alpha is broken. Please, reinstall the app.");
                }
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainViewModelLocator.BassPlayer.Unload();
            MainViewModelLocator.Settings.spexpire = MainViewModelLocator.SpotifyHelper.AccessToken.ExpireTime;
            MainViewModelLocator.Settings.sptoken = MainViewModelLocator.SpotifyHelper.AccessToken.Token;
            MainViewModelLocator.Settings.Save();
            base.OnExit(e);
        }
    }
}
