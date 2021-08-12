using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VKAlpha.Controls;
using VKAlpha.Extensions;

namespace VKAlpha.Helpers
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool isSidebarVisible = false;

        private string searchQuery = "";

        private ISnackbarMessageQueue _msgQueue;

        public ICommand PlayCommand { get; private set; }

        public ICommand PlayPause { get; private set; }

        public ICommand SkipPrev { get; private set; }

        public ICommand SkipNext { get; private set; }

        public ICommand GetFriendAudios { get; private set; }

        public ICommand GetFriendsOf { get; private set; }

        public ICommand CloseDialog { get; private set; }

        public ICommand DoSearch { get; private set; }

        public ICommand GoForward { get; private set; }

        public ICommand GoBackward { get; private set; }

        public ICommand RmSong { get; private set; }

        public ICommand DownloadSong { get; set; }

        public ICommand AddSong { get; private set; }

        public ICommand EditSong { get; private set; }

        public ICommand ShowLyrics { get; private set; }

        public ICommand SubmitEdit { get; private set; }

        public ICommand SearchByArtist { get; private set; }

        public ICommand CRestartNow { get; private set; }

        public ICommand Upload { get; private set; }

        public ICommand OpenSelector { get; private set; }

        public ISnackbarMessageQueue MessageQueue { get => _msgQueue; private set => _msgQueue = value; }

        public string SearchQuery { get => searchQuery; set => this.MutateVerbose(ref searchQuery, value, RaisePropertyChanged()); }

        public bool IsSearchActive { get; set; } = false;

        public DrawerCategory[] SideBarItems { get; private set; }

        public bool SidebarVisible { get => isSidebarVisible; set => this.MutateVerbose(ref isSidebarVisible, value, RaisePropertyChanged()); }

        private void ClearUserData()
        {
            MainViewModelLocator.Settings.token = null;
            MainViewModelLocator.Settings.userid = -1;
            MainViewModelLocator.Settings.Save();
        }

        private void RemoveSong(object data)
        {
            var collection = (((App.Current.MainWindow as MainWindow).FrameMain.Content as UserControl).DataContext as ViewModels.AudiosListViewModel).collection;
            var item = collection.Where(selector => selector.FullData == (string)data).FirstOrDefault();
            MainViewModelLocator.Vk.VkAudio.RemoveSong(item.Id, item.OwnerId);
            collection.Remove(item);
            MainViewModelLocator.PlaylistControl.PlayingPlaylist.Remove(item);
            _msgQueue.Enqueue($"{item.FullData} removed from playlist");
        }

        private void Add(object data)
        {
            var item = (AudioModel)data;
            MainViewModelLocator.Vk.VkAudio.Add(item.Id, item.OwnerId);
        }

        private async void Lyrics(object data)
        {
            var item = (AudioModel)data;
            await Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.GetLyrics(item.LyricsId);
            }).ContinueWith(tsk =>
            {
                if (tsk.Result != null)
                {
                    var dialog = new Dialogs.Lyrics();
                    dialog.tbLyrics.Text = tsk.Result;
                    _ = MainViewModelLocator.WindowDialogs.OpenDialog(dialog.LyricsDial.DialogContent);
                }
                else
                {
                    Trace.WriteLine($"Error [{tsk.Result}]");
                    _msgQueue.Enqueue("Error!");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitCommands()
        {
            PlayCommand = new RelayCommand((o) => MainViewModelLocator.BassPlayer.PauseResume());
            SkipPrev = new RelayCommand((o) => MainViewModelLocator.BassPlayer.Prev());
            SkipNext = new RelayCommand((o) => MainViewModelLocator.BassPlayer.Next());
            GoForward = new RelayCommand((o) => Navigation.Get.GoForward());
            GoBackward = new RelayCommand((o) => Navigation.Get.GoBack());
            CloseDialog = new RelayCommand((o) => MainViewModelLocator.WindowDialogs.CloseDialog());
            DownloadSong = new RelayCommand((o) => TrackDownloader.AddToQueue((AudioModel)o));
            Upload = new RelayCommand((o) => UploadAudio(o));
            OpenSelector = new RelayCommand((o) => UploadSelector(o));
            CRestartNow = new RelayCommand((o) => { });
            RmSong = new RelayCommand(RemoveSong);
            AddSong = new RelayCommand(Add);
            ShowLyrics = new RelayCommand(Lyrics);
            SearchByArtist = new RelayCommand(Search);
            DoSearch = new RelayCommand(Search);
            EditSong = new RelayCommand(Edit);
        }

        private async void UploadSelector(object _)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "MP3 Files (*.mp3)|*.mp3",
                DefaultExt = ".mp3",
            };
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result == true)
            {
                MainViewModelLocator.WindowDialogs.CloseDialog();
                if (await MainViewModelLocator.Vk.VkAudio.UploadAudio(dialog.FileName) == -1)
                    MessageQueue.Enqueue("Error occured while uploading file to VK.com");
                else MessageQueue.Enqueue("Upload Successful!");
            }
        }

        private async void UploadAudio(object _)
        {
            var dialog = new Dialogs.UploadDialog();
            await MainViewModelLocator.WindowDialogs.OpenDialog(dialog.UploadDial.DialogContent);
        }

        private async void Edit(object item)
        {
            var audio = (AudioModel)item;
            var dialog = new Dialogs.EditSong();
            dialog.artistEdit.Text = audio.Artist; dialog.titleEdit.Text = audio.Title;
            if (audio.LyricsId != 0) dialog.lyricsEdit.Text = await MainViewModelLocator.Vk.VkAudio.GetLyrics(audio.LyricsId);
            dialog.SumbitEdit.Command = new RelayCommand((o) =>
            {
                MainViewModelLocator.Vk.VkAudio.Edit(audio.OwnerId, audio.Id, dialog.artistEdit.Text, dialog.titleEdit.Text, dialog.lyricsEdit.Text, (int)audio.GenreId);
                CloseDialog.Execute(o);
            });
            await MainViewModelLocator.WindowDialogs.OpenDialog(dialog.EditSongDial.DialogContent);
        }

        private void Search(object o)
        {
            if ((string)o == "")
                return;
            IsSearchActive = true;
            Navigation.Get.Navigate("AudiosListView", new ViewModels.AudiosListViewModel((string)o));
        }

        public void ClearPlaylistsList(int category)
        {
            SideBarItems[category].TreeContent.Clear();
        }

        public void LoadPlaylists(ulong userId = 0ul, int category = 0)
        {
            if (userId == 0ul) userId = MainViewModelLocator.Vk.AccessToken.UserId;
            ClearPlaylistsList(category);
            Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.GetPlaylists((long)userId);
            }).ContinueWith((tsk) =>
            {
                var result = tsk.Result;
                if (result.Size() > 0)
                {
                    SideBarItems[category].TreeContent.Add(new DrawerItem(MainViewModelLocator.AppLang.Playlists, new RelayCommand((o) => { Navigation.Get.Navigate("PlaylistsView", new ViewModels.PlaylistsViewModel(userId)); })));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public MainViewModel()
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SideBarItems = new[]
            {
                new DrawerCategory(MainViewModelLocator.AppLang.MyAudios, PackIconKind.MusicBoxOutline, new RelayCommand((o)=>
                {
                    if(IsSearchActive) IsSearchActive = false;
                    Navigation.Get.Navigate("AudiosListView", new ViewModels.AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
                })),
                new DrawerCategory(MainViewModelLocator.AppLang.Friends, PackIconKind.AccountMultipleOutline, new RelayCommand((o)=> Navigation.Get.Navigate("FriendsListView", new ViewModels.FriendsListViewModel(MainViewModelLocator.Vk.AccessToken.UserId)) )),
                new DrawerCategory(MainViewModelLocator.AppLang.Settings, PackIconKind.SettingsOutline,  new RelayCommand((o)=>
                {
                    SidebarVisible = false;
                    Navigation.Get.GoToSettings();
                })),
                new DrawerCategory(MainViewModelLocator.AppLang.LogOut, PackIconKind.AccountMinusOutline, new RelayCommand((o)=>
                {
                    MainViewModelLocator.BassPlayer.Stop();
                    ClearUserData();
                    SidebarVisible = false;
                    Navigation.Get.Navigate("LoginView");
                    Navigation.Get.ClearStack();
                }))
            };

            InitCommands();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
