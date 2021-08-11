using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;
using VKAlpha.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace VKAlpha.Views.SubViews
{
    public partial class InterfaceView : UserControl
    {
        public IList<string> theme_items { get; set; }
        private IList<string> theme_short { get; set; }

        public IList<string> lang_items { get; set; }
        private IList<string> lang_code { get; set; }

        bool ingore = true;

        private async void InitSource()
        {
            theme_items = new List<string>();
            theme_short = new List<string>();

            lang_items = new List<string>();
            lang_code = new List<string>();

            LoadThemes();
            LoadLanguages();
            await Task.Delay(1500);
            ingore = false;
        }

        private void LoadLanguages()
        {
            var path = Path.Combine("Resources", "Lang/");
            var files = Directory.GetFiles(path, "*.json");

            var i = -1; 
            var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            foreach (var file in files)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<Resources.Lang.LangModel>(File.ReadAllText(file), settings);
                    lang_items.Add(obj.language);
                    lang_code.Add(obj.language);
                    if (MainViewModelLocator.AppLang.language == obj.language)
                    {
                        i = lang_items.Count - 1;
                    }
                }
                catch (JsonSerializationException) { }
            }
            LanguageCombo.SelectedIndex = i;
        }

        private void LoadThemes()
        {
            var path = Path.Combine("Resources", "Themes/");
            var files = Directory.GetFiles(path, "*.json");

            var i = -1; 
            var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            foreach (var file in files)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<Resources.Themes.ThemeModel>(File.ReadAllText(file), settings);
                    theme_items.Add(obj.name);
                    theme_short.Add(obj.shortname);
                    if (MainViewModelLocator.Settings.theme == obj.shortname)
                        i = theme_items.Count - 1;
                }
                catch (JsonSerializationException) { }
            }
            themeCombo.SelectedIndex = i;
        }

        public InterfaceView()
        {
            InitializeComponent();
            DataContext = this;
            InitSource();
        }

        private void ApplyTheme(string name)
        {
            MainViewModelLocator.Settings["theme"] = name;
            MainViewModelLocator.Settings.Save();
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.RestartRequiredDialog().RestartRequiredDial.DialogContent);
        }

        private void ApplyLang(string name)
        {
            MainViewModelLocator.Settings["lang"] = name;
            MainViewModelLocator.Settings.Save();
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.RestartRequiredDialog().RestartRequiredDial.DialogContent);
        }

        private void themeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ingore)
                return;
            
            ApplyTheme(theme_short[themeCombo.SelectedIndex]);
        }

        private void LangComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ingore)
                return;

            ApplyLang(lang_items[LanguageCombo.SelectedIndex]);
        }
    }
}
