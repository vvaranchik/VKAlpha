using MaterialDesignThemes.Wpf;
using System.Windows.Input;
using VKAlpha.Controls;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class AudiosControlViewViewModel
    {
        /*
        private readonly BASS.BassAudioPlayer _player;

        public DrawerItem[] SideBarItems { get; private set; }

        private void Play(object o)
        {
            _player?.FindAndPlay(o.ToString());
        }

        private void Switch(object o)
        {
            _player?.PauseResume();
        }

        private void Skip(object o)
        {
            if ((string)o == "Next") _player?.Next();
            else if ((string)o == "Prev") _player?.Prev();
        }

        private void GetAudios(object o)
        {
        //    Navigation.UserId = (long)o;
        //    NewNavigation.Navigate("AudiosListView", true);
        }

        private async void LoadFriendsOf(object o)
        {
      //      _ = Locator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            try
            {
      //          await Locator.PlaylistControl.LoadFriends((long)o);
                //NewNavigation.UpdateColorZone("FriendsListView");
            }
            catch (System.ArgumentNullException)
            {
                //((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel).MessageQueue.Enqueue("No access!");
            }
      //      Locator.WindowDialogs.CloseDialog();
        }

        public BASS.BassAudioPlayer GetPlayer => _player;

        public AudiosControlViewViewModel()
        {

            //    PlayCommand = new RelayCommand(Play);
            // /   PlayPause = new RelayCommand(Switch);
            //    SkipTrack = new RelayCommand(Skip);
            //   GetFriendAudios = new RelayCommand(GetAudios);
            //  GetFriendsOf = new RelayCommand(LoadFriendsOf);
      //      SideBarItems = new[]
      //            {
      //          new DrawerItem(Locator.AppLang.MyAudios, PackIconKind.MusicBoxOutline),
     //           new DrawerItem(Locator.AppLang.Friends,  PackIconKind.AccountMultipleOutline),
     //           new DrawerItem(Locator.AppLang.Settings, PackIconKind.SettingsOutline),
     //           new DrawerItem(Locator.AppLang.LogOut, PackIconKind.AccountMinusOutline),
     //       };
    //        _player = Locator.BassPlayer;
        }
        
        public async void ChangeGridContent(GridContent content, long uid)
        {
            if (content == GridContent.EMPTY)
                return;
            _ = Locator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            if (content == GridContent.AudiosList)
            {
                try
                {
                    await Locator.PlaylistControl.LoadPlaylist(uid);
                }
                catch (MonoVKLib.VK.Exceptions.VKAccessDenied)
                {
                    ((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel).MessageQueue.Enqueue("No access!");
                }
                //Locator.AudiosControlView.GridContent.Children.Clear();
                //Locator.AudiosControlView.GridContent.Children.Add(Locator.AudiosListView);
            }
            else if (content == GridContent.FriendsList)
            {
                await Locator.PlaylistControl.LoadFriends(uid);
                //Locator.AudiosControlView.GridContent.Children.Clear();
                //Locator.AudiosControlView.GridContent.Children.Add(Locator.FriendsListView);
            }
            Locator.WindowDialogs.CloseDialog();
        }
        */
    }
}
