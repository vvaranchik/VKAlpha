using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class PlaylistsViewModel
    {
        private ObservableCollection<MonoVKLib.VK.Models.VKPlaylistModel> _collection = new ObservableCollection<MonoVKLib.VK.Models.VKPlaylistModel>();

        public ICommand LoadPlaylist { get; private set; }

        public ObservableCollection<MonoVKLib.VK.Models.VKPlaylistModel> collection { get => _collection; private set => _collection = value; }

        public PlaylistsViewModel(ulong uid)
        {
            Init(uid);
            LoadPlaylist = new RelayCommand((o) => LoadUserPlaylist(uid, (long)o));
        }

        private async void Init(ulong uid)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.GetPlaylists((long)uid);
            }).ContinueWith(tsk =>
            {
                if (tsk.Result.IsEmpty())
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AccessDenied);
                    if (!Navigation.Get.Service.CanGoBack) { Navigation.Get.GoBackExtra(); return; }
                    Navigation.Get.GoBack();
                    return;
                }
                var logo = (App.Current.FindResource("ALPHA") as System.Windows.Controls.Image).Source.ToString();
                tsk.Result.ForEach((a) =>
                {
                    if (a.Count > 0)
                    {
                        if(a.Thumbs == null || a.Thumbs.Count == 0)
                        {
                            a.Photo = logo;
                        }
                        else if (a.Thumbs.Count <= 4 && a.Thumbs.Count > 0)
                        {
                            a.Photo = a.Thumbs[0].photo_300;
                        }
                    }
                    
                    collection.Add(a);
                });
                
                if (uid == MainViewModelLocator.Vk.AccessToken.UserId)
                {
                    collection.Add(new MonoVKLib.VK.Models.VKPlaylistModel { Title = "Create new playlist", Photo = logo, Id = -2 });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void LoadUserPlaylist(object userId, object playlistId)
        {
            if((long)playlistId == -2)
            {
                // TODO: 
                // Open dialog with fields:
                // Title
                // Description (optional)
                // Selectable image for playlist cover (optional)
                // Next dialog with list of main audios that can be selected for adding in this playlist
                // [Submit] [Cancel]
                MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("This feature not available now...");
                return;
            }
            Navigation.Get.Navigate("AudiosListView", new AudiosListViewModel(ulong.Parse(userId.ToString()), (long)playlistId));
        }
    }
}
