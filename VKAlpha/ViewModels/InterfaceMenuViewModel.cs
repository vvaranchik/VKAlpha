using System;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using Newtonsoft.Json;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class InterfaceMenuViewModel : INotifyPropertyChanged
    {
        bool ignore_on_startup = true;
        IList<string> languages;
        IList<string> themes;

        IList<string> themes_filenames;

        string sCurrent_theme, sCurrent_lang;
        bool loadUserAvatars = MainViewModelLocator.Settings.load_user_avatars, loadTrackCovers = MainViewModelLocator.Settings.load_track_covers, cacheTrackCovers = MainViewModelLocator.Settings.cache_track_covers;

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentTheme { get => sCurrent_theme; set { sCurrent_theme = value; if (!ignore_on_startup) SetTheme.Execute(themes_filenames[themes.IndexOf(sCurrent_theme)]); } }
        public string CurrentLang { get => sCurrent_lang; set { sCurrent_lang = value; if (!ignore_on_startup) SetLang.Execute(CurrentLang); } }

        public bool LoadUserAvatars { get => loadUserAvatars; set { loadUserAvatars = MainViewModelLocator.Settings.load_user_avatars = value; RaisePropertyChanged()?.Invoke(new PropertyChangedEventArgs(null)); } }
        public bool LoadTrackCovers { get => loadTrackCovers; set { loadTrackCovers = MainViewModelLocator.Settings.load_track_covers = value; RaisePropertyChanged()?.Invoke(new PropertyChangedEventArgs(null)); } }
        public bool CacheTrackCovers { get => cacheTrackCovers; set { cacheTrackCovers = MainViewModelLocator.Settings.cache_track_covers = value; RaisePropertyChanged()?.Invoke(new PropertyChangedEventArgs(null)); } }

        public IList<string> Languages { get => languages; set => this.MutateVerbose(ref languages, value, RaisePropertyChanged()); }
        public IList<string> Themes { get => themes; set => this.MutateVerbose(ref themes, value, RaisePropertyChanged()); }

        public ICommand SetTheme { get; private set; }
        public ICommand SetLang { get; private set; }

        void InitCommands()
        {
            SetTheme = new RelayCommand((o) =>
            {
                MainViewModelLocator.Settings["theme"] = (string)o;
                MainViewModelLocator.Settings.Save();
                MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.RestartRequiredDialog().RestartRequiredDial.DialogContent);
            });
            SetLang = new RelayCommand((o) =>
            {
                MainViewModelLocator.Settings["lang"] = (string)o;
                MainViewModelLocator.Settings.Save();
                MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.RestartRequiredDialog().RestartRequiredDial.DialogContent);
            });
        }

        void InitLists()
        {
            languages = new List<string>();
            themes = new List<string>();
            themes_filenames = new List<string>();

            var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            if (!Directory.Exists(".\\Resources\\Lang\\"))
                App.CreateDefaultSetup(true, false);
            var langFiles = Directory.GetFiles(".\\Resources\\Lang\\", "*.json");
            foreach (var langFile in langFiles)
            {
                try
                {
                    var jsonLang = JsonConvert.DeserializeObject<Resources.Lang.LangModel>(File.ReadAllText(langFile), settings);
                    languages.Add(jsonLang.language);
                    if (MainViewModelLocator.Settings.lang == jsonLang.language)
                        CurrentLang = jsonLang.language;
                }
                catch (JsonSerializationException) { continue; }
            }

            if (!Directory.Exists(".\\Resources\\Themes\\"))
                App.CreateDefaultSetup(false, true);
            var themeFiles = Directory.GetFiles(".\\Resources\\Themes\\", "*.json");
            foreach (var themeFile in themeFiles)
            {
                try
                {
                    var jsonTheme = JsonConvert.DeserializeObject<Resources.Themes.ThemeModel>(File.ReadAllText(themeFile), settings);
                    themes.Add(jsonTheme.name);
                    themes_filenames.Add(jsonTheme.shortname);
                    if (MainViewModelLocator.Settings.theme == jsonTheme.shortname)
                        CurrentTheme = jsonTheme.name;
                }
                catch (JsonSerializationException) { continue; }
            }
            ignore_on_startup = false;
        }

        Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);

        public InterfaceMenuViewModel()
        {
            InitCommands();
            InitLists();
        }
    }
}
