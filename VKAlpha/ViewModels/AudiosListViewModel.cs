using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class AudiosListViewModel
    {
        private ObservableCollection<AudioModel> _collection = new ObservableCollection<AudioModel>();

        public ICommand PlayCommand { get; private set; }

        public ObservableCollection<AudioModel> collection { get => _collection; private set => _collection = value; }

        public AudiosListViewModel(ulong uid)
        {
            Init(uid);
            PlayCommand = new RelayCommand(Play);
        }

        public AudiosListViewModel(ulong uid, long albumId)
        {
            Init(uid, albumId);
            PlayCommand = new RelayCommand(Play);
        }

        public AudiosListViewModel(string query)
        {
            Init(query);
            PlayCommand = new RelayCommand(Play);
        }

        private async void Init(ulong uid, long albumId = 0)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return albumId == 0 ? await MainViewModelLocator.Vk.VkAudio.Get((long)uid) : await MainViewModelLocator.Vk.VkAudio.Get((long)uid, albumId);
            }).ContinueWith(tsk =>
            {
                if (tsk.Result.Size() == 0)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AccessDenied);
                    if (!Navigation.Get.Service.CanGoBack) { Navigation.Get.GoBackExtra(); return; } 
                    Navigation.Get.GoBack();
                    return;
                }
                collection.Clear();
                tsk.Result.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        collection.Add(AudioModel.VKModelToAudio(a));
                });
                MainViewModelLocator.PlaylistControl.NullCheckPlaylist(collection);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            if (MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = false;
        }

        private async void Init(string query)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.Search(query);
            }).ContinueWith(tsk =>
            {
                tsk.Result.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        collection.Add(AudioModel.VKModelToAudio(a));
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            if (!MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = true;
        }

        private void Play(object o)
        {
            MainViewModelLocator.PlaylistControl.CheckPlaylist(collection);
            MainViewModelLocator.BassPlayer.FindAndPlay(o.ToString());
        }
    }
}
