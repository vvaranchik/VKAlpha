using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using VKAlpha.Helpers;
using Windows.System;

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

        [System.Obsolete("Soon this will be removed")]
        public static void CreateDefaultSetup(bool lang, bool theme)
        {
            if (lang)
            {
                Directory.CreateDirectory("./Resources/Lang/");
                if (File.Exists("./Resources/Lang/en.json"))
                    File.Delete("./Resources/Lang/en.json");
                File.WriteAllText("./Resources/Lang/en.json",
                    @"{
  ""language"": ""en"",
  ""Friends"": ""Friends"",
  ""LogOut"": ""Log Out"",
  ""MyAudios"": ""My Audios"",
  ""Settings"": ""Settings"",
  ""Play"": ""Play"",
  ""Pause"": ""Pause"",
  ""Next"": ""Next"",
  ""Prev"": ""Previous"",
  ""ShuffleOff"": ""Shuffle"",
  ""ShuffleOn"": ""Don't shuffle"",
  ""RepeatOff"": ""Don't repeat"",
  ""RepeatOn"": ""Repeat"",
  ""Mute"": ""Mute"",
  ""MaxVol"": ""Maximum Volume"",
  ""EditSong"": ""Edit"",
  ""DelSong"": ""Remove song from Your Playlist"",
  ""AddSong"": ""Add song to Your Playlist"",
  ""Lyrics"": ""Lyrics"",
  ""GetFriends"": ""Friends of "",
  ""Login"": ""Login"",
  ""Password"": ""Password"",
  ""aLogIn"": ""Log In"",
  ""AuthFailed"": ""Auth failed. Check login and password."",
  ""AccessDenied"": ""Access Denied"",
  ""GoBack"": ""Back"",
  ""UpdateMenu"": ""Updates"",
  ""AboutMenu"": ""About"",
  ""HotkeyMenu"": ""Hotkeys"",
  ""AccountMenu"": ""Account"",
  ""InterfaceMenu"": ""Interface"",
  ""Language"": ""Language"",
  ""CurrentTheme"": ""Current Theme"",
  ""AppearanceCat"": ""Appearance"",
  ""Playlists"": ""Playlists"",
  ""My"": ""My"",
  ""UploadAudio"": ""Upload audio to VK.com"",
  ""strFormatFriends"": ""[user] [friends]""
}");
                if (File.Exists("./Resources/Lang/ru.json"))
                    File.Delete("./Resources/Lang/ru.json");
                File.WriteAllText("./Resources/Lang/ru.json", @"
{
  ""language"": ""ru"",
  ""Friends"": ""Друзья"",
  ""LogOut"": ""Выйти"",
  ""MyAudios"": ""Моя музыка"",
  ""Settings"": ""Настройки"",
  ""Play"": ""Слушать"",
  ""Pause"": ""Пауза"",
  ""Next"": ""Вперед"",
  ""Prev"": ""Назад"",
  ""ShuffleOff"": ""Перемешать"",
  ""ShuffleOn"": ""Не перемешивать"",
  ""RepeatOff"": ""Не повторять"",
  ""RepeatOn"": ""Повторять трек"",
  ""Mute"": ""Выкл. звук"",
  ""MaxVol"": ""Включить звук"",
  ""EditSong"": ""Редактировать"",
  ""DelSong"": ""Удалить из Ваших аудиозаписей"",
  ""AddSong"": ""Добавить к Вашим аудиозаписям"",
  ""Lyrics"": ""Текст"",
  ""GetFriends"": ""Друзья "",
  ""Login"": ""Логин"",
  ""Password"": ""Пароль"",
  ""aLogIn"": ""Войти"",
  ""AuthFailed"": ""Ошибка. Проверьте Ваш логин и пароль."",
  ""AccessDenied"": ""Доступ запрещен"",
  ""GoBack"": ""Назад"",
  ""UpdateMenu"": ""Обновление"",
  ""AboutMenu"": ""О VKAlpha"",
  ""HotkeyMenu"": ""Хоткеи"",
  ""AccountMenu"": ""Аккаунт"",
  ""InterfaceMenu"": ""Интерфейс"",
  ""Language"": ""Язык"",
  ""CurrentTheme"": ""Текущая тема"",
  ""AppearanceCat"": ""Внешний вид"",
  ""Playlists"": ""Плейлисты"",
  ""My"": ""Мои"",
  ""UploadAudio"": ""Загрузить аудио в VK.com"",
  ""strFormatFriends"": ""[friends] [user]""
}
");
            }
            if (theme)
            {
                Directory.CreateDirectory("./Resources/Themes/");
                if (File.Exists("./Resources/Themes/default.json"))
                    File.Delete("./Resources/Themes/default.json");
                File.WriteAllText("./Resources/Themes/default.json", @"
{
	""name"": ""Default [VK Alpha] Theme"",
	""shortname"": ""default"",
	""author"": ""VaDa"",
	""desc"": ""Please, DO NOT edit this file while messing with themes. Editing this file without knowledge can cause errors!"",
	""MainBackground"": ""#FFffffff"",
	""ForegroundMenu"": ""#FF000000"",
	""ForegroundElements"": ""#FFffffff"",
	""PlayerForegroundBottom"": ""#FFffffff"",
	""TitleListForeground"": ""#FFffffff"",
	""OtherListForeground"": ""#FFffffff"",
	""HintForeground"": ""#FF808080"",
	""TextBoxBackground"": ""#FFffffff"",
	""TextBoxForeground"": ""#FF000000"",
	""BelowMain"": ""#FF4169FF"",
	""AboveMain"": ""#FF4169FF"",
	""btnHovered"": ""#FF142155"",
	""checkBoxUncheked"": ""#FFffffff"",
	""checkBoxChecked"": ""#FF142155"",
    ""listboxItemHovered"": ""FFE2E2E2""
}
");
                if (File.Exists("./Resources/Themes/defaultdark.json"))
                    File.Delete("./Resources/Themes/defaultdark.json");
                File.WriteAllText("./Resources/Themes/defaultdark.json", @"
{
	""name"": ""Default DARK [VK Alpha] Theme"",
	""shortname"": ""defaultdark"",
	""author"": ""VaDa"",
	""desc"": ""Please, DO NOT edit this file while messing with themes. Editing this file without knowledge can cause errors!"",
	""MainBackground"": ""#FF202124"",
	""ForegroundMenu"": ""#FFAEB2B7"",
	""ForegroundElements"": ""#FFffffff"",
	""PlayerForegroundBottom"": ""#FFffffff"",
	""TitleListForeground"": ""#FFffffff"",
	""OtherListForeground"": ""#FFffffff"",
	""HintForeground"": ""#FFffffff"",
	""TextBoxBackground"": ""#FF202124"",
	""TextBoxForeground"": ""#FFAEB2B7"",
	""BelowMain"": ""#FF142155"",
	""AboveMain"": ""#FF142155"",
	""btnHovered"": ""#FF4169FF"",
	""checkBoxUncheked"": ""#FFffffff"",
	""checkBoxChecked"": ""#FF4169FF"",
    ""listboxItemHovered"": ""FF2A2A2A""
}
");
            }
        }

        public static void RaiseException<T>(string err_msg, T expception, string caption = "ERROR") where T : System.Exception
        {
            MessageBox.Show(err_msg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            var e = new System.Exception(err_msg, expception);
            throw e;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!LoadLang(MainViewModelLocator.Settings.lang))
                if (!LoadLang("en")) 
                {
                    CreateDefaultSetup(true, false);
                    LoadLang("en");
                }

            if (!LoadTheme(MainViewModelLocator.Settings.theme))
                if (!LoadTheme("default"))
                {
                    CreateDefaultSetup(false, true);
                    LoadTheme("default");
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
