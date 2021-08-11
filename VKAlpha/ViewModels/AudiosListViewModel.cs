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

        public AudiosListViewModel(long uid)
        {
            Init(uid);
            PlayCommand = new RelayCommand(Play);
        }

        public AudiosListViewModel(long uid, long albumId)
        {
            Init(uid, albumId);
            PlayCommand = new RelayCommand(Play);
        }

        public AudiosListViewModel(string query)
        {
            Init(query);
            PlayCommand = new RelayCommand(Play);
        }

        private async void Init(long uid, long albumId = 0)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return albumId == 0 ? await MainViewModelLocator.Vk.VkAudio.Get(uid) : await MainViewModelLocator.Vk.VkAudio.Get(uid, albumId);
            }).ContinueWith(tsk =>
            {
                if (tsk.Result.TotalCount == 0)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AccessDenied);
                    if (!_Navigation.Get.Service.CanGoBack) System.Diagnostics.Trace.WriteLine("No way to go back! Will crash now!"); // this will never happen
                    _Navigation.Get.GoBack();
                    return;
                }
                collection.Clear();
                tsk.Result.Items.ForEach((a) =>
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
                tsk.Result.Items.ForEach((a) =>
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
