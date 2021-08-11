using System;
using System.IO;
using System.Windows;
using System.Linq;
using VKAlpha.Helpers;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VKAlpha
{
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            if (!LoadLang(MainViewModelLocator.Settings.lang))
                if (!LoadLang("en")) throw new FileNotFoundException("Language file not found or corrupted!");

            InitializeComponent();
            DataContext = MainViewModelLocator.MainViewModel;
        }

        private bool LoadLang(string lang)
        {
            bool result = default;
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

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            _Navigation.Service = FrameMain.NavigationService;
            if (string.IsNullOrEmpty(MainViewModelLocator.Settings.token))
            {
                _Navigation.Navigate("LoginView");
            }
            else
            {
                MainViewModelLocator.Vk.AccessToken.Token = MainViewModelLocator.Settings.token;
                MainViewModelLocator.Vk.AccessToken.UserId = MainViewModelLocator.Settings.userid;
                MainViewModelLocator.MainViewModel.SidebarVisible = true;
                _Navigation.Navigate("AudiosListView", new ViewModels.AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
            }
        }

        private void tbSearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = tbSearchQuery.Text.ToLower();
            dynamic page = (UserControl)FrameMain.Content;

            if (FrameMain.Content.GetType().Name == "AudiosListView")
            {
                var vm = (FrameMain.Content as UserControl).DataContext as ViewModels.AudiosListViewModel;
                if (query == "")
                {
                    page.AudiosList.ItemsSource = vm.collection;
                    return;
                }
                var result = from items in vm.collection
                             let data = items.FullData.ToLower()
                             where data.Contains(query)
                             select items;
                page.AudiosList.ItemsSource = result;
            }
            else if (FrameMain.Content.GetType().Name == "FriendsListView")
            {
                var vm = (FrameMain.Content as UserControl).DataContext as ViewModels.FriendsListViewModel;
                if (query == "")
                {
                    page.FriendsList.ItemsSource = vm.collection;
                    return;
                }
                var result = from items in vm.collection
                             let data = items.Name.ToLower()
                             where data.Contains(query)
                             select items;
                page.FriendsList.ItemsSource = result;
            }
            else return;
        }
    }
}
