using System;
using System.Linq;
using System.Windows.Interop;
using VKAlpha.Helpers;
using VKAlpha.Extensions;

namespace VKAlpha
{
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public static IntPtr Handle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            Navigation.Get.Service = FrameMain.NavigationService;
            if (string.IsNullOrEmpty(MainViewModelLocator.Settings.token))
            {
                Navigation.Get.Navigate("LoginView");
            }
            else
            {
                MainViewModelLocator.Vk.AccessToken.Token = MainViewModelLocator.Settings.token;
                MainViewModelLocator.Vk.AccessToken.UserId = (ulong)MainViewModelLocator.Settings.userid;
                MainViewModelLocator.MainViewModel.SidebarVisible = true;
                Navigation.Get.Navigate("AudiosListView", new ViewModels.AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
                MainViewModelLocator.MainViewModel.LoadPlaylists();
            }
            Handle = new WindowInteropHelper(this).Handle;
        }

        private void TextBlock_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FrameMain.Content.GetType().Name == "AudiosListView")
            {
                var audiosView = (Views.AudiosListView)FrameMain.Content;
                var list = audiosView.AudiosList.ItemsSource.Cast<AudioModel>();
                var playing = MainViewModelLocator.MainViewModel.CurrentPlayingItem;
                var first = list.FirstOrDefault();

                if (list.Count() == 0 ||
                    !AudioModel.IsAudioValid(first) || 
                    !AudioModel.IsAudioValid(playing) ||
                    list.Count() != MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count ||
                    first.OwnerId != playing.OwnerId &&
                    !MainViewModelLocator.MainViewModel.IsSearchActive)
                {
                    return;
                }
                //var result = list.FirstOrDefault(x => playing.FullData.Contains(x.FullData, StringComparison.InvariantCultureIgnoreCase));
                //if (result != null && result != default)
                //{
                    
                    audiosView.AudiosList.ScrollIntoView(playing);
                //}
            }
        }
    }
}
