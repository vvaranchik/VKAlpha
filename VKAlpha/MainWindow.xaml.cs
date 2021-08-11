using System;
using System.Linq;
using System.Windows.Interop;
using VKAlpha.Helpers;
using System.Windows.Controls;
using VKAlpha.Extensions;

namespace VKAlpha
{
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public static IntPtr Handle;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModelLocator.MainViewModel;
        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            _Navigation.Get.Service = FrameMain.NavigationService;
            if (string.IsNullOrEmpty(MainViewModelLocator.Settings.token))
            {
                _Navigation.Get.Navigate("LoginView");
            }
            else
            {
                MainViewModelLocator.Vk.AccessToken.Token = MainViewModelLocator.Settings.token;
                MainViewModelLocator.Vk.AccessToken.UserId = MainViewModelLocator.Settings.userid;
                MainViewModelLocator.MainViewModel.SidebarVisible = true;
                _Navigation.Get.Navigate("AudiosListView", new ViewModels.AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
                MainViewModelLocator.MainViewModel.LoadPlaylists();
            }
            Handle = new WindowInteropHelper(this).Handle;
        }

        // Breaking mvvm pattern here, need more fancy moves without breaking mvvm pattern
        private void tbSearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = tbSearchQuery.Text;
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
                             let data = items.FullData
                             where data.Contains(query, StringComparison.OrdinalIgnoreCase)
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
                             let data = items.Name
                             where data.Contains(query, StringComparison.OrdinalIgnoreCase)
                             select items;
                page.FriendsList.ItemsSource = result;
            }
            else if (FrameMain.Content.GetType().Name == "PlaylistsView")
            {
                var vm = (FrameMain.Content as UserControl).DataContext as ViewModels.PlaylistsViewModel;
                if (query == "")
                {
                    page.PlaylistList.ItemsSource = vm.collection;
                    return;
                }
                var result = from items in vm.collection
                             let data = items.Title
                             where data.Contains(query, StringComparison.OrdinalIgnoreCase)
                             select items;
                page.PlaylistList.ItemsSource = result;
            }
        }

        private void TextBlock_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FrameMain.Content.GetType().Name == "AudiosListView")
            {
                var audiosView = (Views.AudiosListView)FrameMain.Content;
                var vm = (FrameMain.Content as UserControl).DataContext as ViewModels.AudiosListViewModel;
                if (MainViewModelLocator.BassPlayer.CurrentTrack.OwnerId != vm.collection[0].OwnerId && !MainViewModelLocator.MainViewModel.IsSearchActive) {
                    return;
                }
                var result = from items in vm.collection
                             let data = items.FullData
                             where data.Contains(MainViewModelLocator.BassPlayer.CurrentTrack.FullData, StringComparison.OrdinalIgnoreCase)
                             select items;
                audiosView.AudiosList.ScrollIntoView(result.ToList()[0]);
                //audiosView.AudiosList.ScrollIntoView(MainViewModelLocator.BassPlayer.CurrentTrack);
            }
        }
    }
}
