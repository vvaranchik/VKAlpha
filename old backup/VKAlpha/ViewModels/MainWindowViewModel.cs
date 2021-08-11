using System;
using System.ComponentModel;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using VKAlpha.Controls;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /*
        private async void GoTo(object o)
        {
            
            if (!byte.TryParse((string)o, out byte index))
                return;
            switch (index)
            {
                case 3: ClearUserData(); _Navigation.Navigate("LoginView"); break;
                case 0:
                    _ = Locator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
                    await Locator.PlaylistControl.LoadPlaylist(Locator.Vk.AccessToken.UserId);
                    NewNavigation.Navigate("AudiosListView");
                    NewNavigation.UpdateColorZone("AudiosListView");
                    Locator.WindowDialogs.CloseDialog();
                    break;
                case 1:
                    _ = Locator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
                    await Locator.PlaylistControl.LoadFriends(Locator.Vk.AccessToken.UserId);
                    NewNavigation.Navigate("FriendsListView", true);
                    Locator.WindowDialogs.CloseDialog();
                    break;
            }
            
            if (index == CurrentView.LogOut.EnumConvert())
            {
                if (!string.IsNullOrEmpty(Locator.Settings.token))
                {
                    ClearUserData();
                }
            }
            
            if (index == 0 || index == 1)
            {
                (Locator.AudiosControlView.DataContext as AudiosControlViewViewModel).
                    ChangeGridContent(index == 0 ? GridContent.AudiosList : index == 1 ? GridContent.FriendsList : GridContent.EMPTY, Locator.Vk.AccessToken.UserId);
            }
            Navigation.Update(true);
            
    }
    */
        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
