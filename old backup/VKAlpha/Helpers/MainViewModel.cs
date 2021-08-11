using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        public ICommand AddSong { get; private set; }

        public ICommand EditSong { get; private set; }

        public ICommand ShowLyrics { get; private set; }

        public ISnackbarMessageQueue MessageQueue { get => _msgQueue; private set => _msgQueue = value; }

        public string SearchQuery { get => searchQuery; set => this.MutateVerbose(ref searchQuery, value, RaisePropertyChanged()); }

        public DrawerItem[] SideBarItems { get; private set; }

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
            collection = null;
        }

        private void Add(object data)
        {
            var item = (MonoVKLib.VK.Models.VKAudioModel)data;
            MainViewModelLocator.Vk.VkAudio.Add(item.Id, item.OwnerId);
        }

        private async void Lyrics(object data)
        {
            var item = (MonoVKLib.VK.Models.VKAudioModel)data;
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
                    System.Diagnostics.Trace.WriteLine($"Error [{tsk.Result}]");
                    _msgQueue.Enqueue("Error!");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitCommands()
        {
            PlayCommand = new RelayCommand((o) => MainViewModelLocator.BassPlayer.PauseResume() );
            SkipPrev = new RelayCommand((o) => MainViewModelLocator.BassPlayer.Prev() );
            SkipNext = new RelayCommand((o) => MainViewModelLocator.BassPlayer.Next() );
            GoForward = new RelayCommand((o) => _Navigation.GoForward() );
            GoBackward = new RelayCommand((o) => _Navigation.GoBack() );
            CloseDialog = new RelayCommand((o) => MainViewModelLocator.WindowDialogs.CloseDialog());
            RmSong = new RelayCommand(RemoveSong);
            AddSong = new RelayCommand(Add);
            ShowLyrics = new RelayCommand(Lyrics);
            DoSearch = new RelayCommand(Search);
        }

        private void Search(object o)
        {
            if ((string)o == "")
                return;
            _Navigation.Navigate("AudiosListView", new ViewModels.AudiosListViewModel((string)o));
        }

        public MainViewModel()
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
            SideBarItems = new[]
                {
                new DrawerItem(MainViewModelLocator.AppLang.MyAudios, PackIconKind.MusicBoxOutline, new RelayCommand((o)=>
                {
                    _Navigation.Navigate("AudiosListView", new ViewModels.AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
                })),
                new DrawerItem(MainViewModelLocator.AppLang.Friends,  PackIconKind.AccountMultipleOutline,new RelayCommand((o)=>
                {
                    _Navigation.Navigate("FriendsListView", new ViewModels.FriendsListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
                })),
                new DrawerItem(MainViewModelLocator.AppLang.Settings, PackIconKind.SettingsOutline,new RelayCommand((o)=> 
                {
                    
                })),
                new DrawerItem(MainViewModelLocator.AppLang.LogOut, PackIconKind.AccountMinusOutline,new RelayCommand((o)=> 
                {
                    MainViewModelLocator.BassPlayer.Stop();
                    ClearUserData();
                    SidebarVisible = false;
                    _Navigation.Navigate("LoginView");
                    _Navigation.ClearStack();
                })),
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
