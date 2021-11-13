using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MonoVKLib.VK.Exceptions;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class AudiosListViewModel
    {
        string captchaSid = null, captchaKey = null;

        private ObservableCollection<AudioModel> _collection = new ObservableCollection<AudioModel>();

        public ICommand PlayCommand { get; private set; }

        public ICommand GetRecommendationsCmd { get; private set; }

        public ObservableCollection<AudioModel> collection { get => _collection; private set => _collection = value; }

        private void InitCommands()
        {

            PlayCommand = new RelayCommand(Play);
            GetRecommendationsCmd = new RelayCommand(GetRecommendations);
        }

        public AudiosListViewModel(ulong uid)
        {
            Init(uid);
            InitCommands();
        }

        public AudiosListViewModel(ulong uid, long albumId)
        {
            Init(uid, albumId);
            InitCommands();
        }

        public AudiosListViewModel(string query)
        {
            Init(query);
            InitCommands();
        }

        public AudiosListViewModel(ICollection<AudioModel> recommendations)
        {
            Init(recommendations);
            InitCommands();
        }

        private void Init(ICollection<AudioModel> recommendations)
        {
            foreach(var audio in recommendations)
            {
                if (!string.IsNullOrEmpty(audio.Url))
                    collection.Add(audio);
            }
            //collection = new ObservableCollection<AudioModel>(recommendations);
            MainViewModelLocator.PlaylistControl.NullCheckPlaylist(collection);
        }

        private async void Init(ulong uid, long albumId = 0)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                var result = albumId == 0 ? await MainViewModelLocator.Vk.VkAudio.Get((long)uid, captchaSid: captchaSid, captchaKey: captchaKey) : await MainViewModelLocator.Vk.VkAudio.Get((long)uid, albumId, captchaSid: captchaSid, captchaKey: captchaKey);
                captchaKey = captchaSid = null;
                return result;
            }).ContinueWith(async tsk =>
            {
                var error = VKErrorProcessor.GetLastError();
                if (error.code != VKErrorProcessor.VKErrors.NO_ERROR)
                {
                    switch (error.code)
                    {
                        case VKErrorProcessor.VKErrors.Capthca:
                            var e = (VKErrorProcessor.GetLastError().exception as VKCaptchaRequired);
                            var captcha = new Dialogs.CaptchaDialog();
                            captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                            await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                            captchaKey = captcha.CaptchaKey.Text;
                            captchaSid = e.CaptchaSid;
                            Init(uid, albumId);
                            return;
                        case VKErrorProcessor.VKErrors.InvalidClient:
                            MainViewModelLocator.MainViewModel.Logout();
                            MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Token outdated. New auth requested.");
                            return;
                        default:
                            System.Windows.MessageBox.Show($"Error appered: code {error.code}| msg {error.exception.Message}");
                            return;
                    }
                }

                if (tsk.Result.IsEmpty())
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
                if (collection.Count > 100)
                {
                    if((long)uid == MainViewModelLocator.Settings.userid)
                    {
                        var items = MainViewModelLocator.MainViewModel.SideBarItems[0].TreeContent;
                        if (items[items.Count - 1].Name == "Get Recommendations") return;
                        items.Add(new Controls.DrawerItem("Get Recommendations", GetRecommendationsCmd));
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            if (MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = false;
        }

        private async void Init(string query)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                var result = await MainViewModelLocator.Vk.VkAudio.Search(query, captchaSid: captchaSid, captchaKey: captchaKey);
                captchaKey = captchaSid = null;
                return result;
            }).ContinueWith(async(tsk) =>
            {
                if (tsk.Result.IsEmpty())
                {
                    var error = VKErrorProcessor.GetLastError();
                    if (error.code != VKErrorProcessor.VKErrors.NO_ERROR)
                    {
                        switch (error.code)
                        {
                            case VKErrorProcessor.VKErrors.Capthca:
                                var e = (VKErrorProcessor.GetLastError().exception as VKCaptchaRequired);
                                var captcha = new Dialogs.CaptchaDialog();
                                captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                                await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                                captchaKey = captcha.CaptchaKey.Text;
                                captchaSid = e.CaptchaSid;
                                Init(query);
                                return;
                            case VKErrorProcessor.VKErrors.InvalidClient:
                                MainViewModelLocator.MainViewModel.Logout();
                                MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Token outdated. New auth requested.");
                                return;
                            default:
                                System.Windows.MessageBox.Show($"Error appered: code {error.code}| msg {error.exception.Message}");
                                return;
                        }
                    }
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Nothing found");
                    if (!Navigation.Get.Service.CanGoBack) { Navigation.Get.GoBackExtra(); return; }
                    Navigation.Get.GoBack();
                    return;
                }
                tsk.Result.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        collection.Add(AudioModel.VKModelToAudio(a));
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            if (!MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = true;
        }

        private async Task<Collection<AudioModel>> GetRandomTracks(int n)
        {
            Collection<AudioModel> tracks = new Collection<AudioModel>();
            if (n > 0)
            {
                var audios = collection[0].OwnerId == MainViewModelLocator.Settings.userid ? collection : AudioModel.VKArrayToAudioCollection(await MainViewModelLocator.Vk.VkAudio.Get());
                for (var i = 0; i < n; i++)
                {
                    tracks.Add(audios.GetRandomElement());
                }
            }
            return tracks;
        }

        private async Task<MonoSpotifyLib.Spotify.RETURN<Collection<AudioModel>>> getRecommendations(Collection<AudioModel> prevCollection = null)
        {
            var tmp = prevCollection ?? new Collection<AudioModel>();
            var audios = await GetRandomTracks(10); // seems accurate enough
            foreach (var vkAudio in audios)
            {
                var recommendations = await MainViewModelLocator.SpotifyHelper.Recommendations.GetBasedOnTrack(vkAudio.Artist, vkAudio.Title, 5);
                if (recommendations == null) { continue; }
                foreach (var spotifyAudio in recommendations.Tracks)
                {
                    var vkSearchResult = await MainViewModelLocator.Vk.VkAudio.Search($"{spotifyAudio.Artists[0].Name} - {spotifyAudio.Name}", captchaSid: captchaSid, captchaKey: captchaKey);
                    captchaKey = captchaSid = null;
                    if (vkSearchResult == null || vkSearchResult.IsEmpty())
                    {
                        if (VKErrorProcessor.GetLastError().code != VKErrorProcessor.VKErrors.NO_ERROR)
                        {
                            if (VKErrorProcessor.GetLastError().code == VKErrorProcessor.VKErrors.Capthca)
                                return new MonoSpotifyLib.Spotify.RETURN<Collection<AudioModel>>(MonoSpotifyLib.Spotify.Exceptions.Errors.InvalidMethod, tmp);
                            else if (VKErrorProcessor.GetLastError().code == VKErrorProcessor.VKErrors.InvalidClient)
                                return new MonoSpotifyLib.Spotify.RETURN<Collection<AudioModel>>(MonoSpotifyLib.Spotify.Exceptions.Errors.InvalidClient, null);
                        }
                        continue;
                    }
                    foreach (MonoVKLib.VK.Models.VKAudioModel vkSearchAudio in vkSearchResult)
                    {
                        if (!vkSearchAudio.Artist.Contains(spotifyAudio.Artists[0].Name, System.StringComparison.OrdinalIgnoreCase)) continue;
                        tmp.Add(AudioModel.VKModelToAudio(vkSearchAudio));
                        break;
                    }
                }
                await Task.Delay(1550);
            }
            return new MonoSpotifyLib.Spotify.RETURN<Collection<AudioModel>>(MonoSpotifyLib.Spotify.Exceptions.Errors.NoError, tmp);
        }

        /// <summary>
        /// TODO: 
        /// Add setting to choose recommendation type:
        /// 1 - N random tracks from playlist (done)
        /// 2 - First N tracks from playlist
        /// 3 - N tracks from start of the playlist + N from middle + N from end
        /// </summary>
        /// <param name="o"></param>
        private async void GetRecommendations(object o = null)
        {
            MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return await getRecommendations((Collection<AudioModel>)o);
            }).ContinueWith(new System.Action<Task<MonoSpotifyLib.Spotify.RETURN<Collection<AudioModel>>>>(async (tsk) =>
            {
                var @return = tsk.Result;
                // need to move error processing to another method, probably to MainViewModelLocator.ProcessError(SafeReturn<> ret_val)
                if (@return.GetError() != MonoSpotifyLib.Spotify.Exceptions.Errors.NoError)
                {
                    var error = @return.GetError();
                    switch (error)
                    {
                        case MonoSpotifyLib.Spotify.Exceptions.Errors.InvalidMethod:
                            var e = (VKErrorProcessor.GetLastError().exception as VKCaptchaRequired);
                            var captcha = new Dialogs.CaptchaDialog();
                            captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                            captchaSid = e.CaptchaSid;
                            await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                            captchaKey = captcha.CaptchaKey.Text;
                            GetRecommendations(@return.GetResult());
                            return;
                        case MonoSpotifyLib.Spotify.Exceptions.Errors.InvalidClient:
                            MainViewModelLocator.MainViewModel.Logout();
                            MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Token outdated. New auth requested.");
                            return;
                        default:
                            System.Windows.MessageBox.Show($"Error appered: code {error}");
                            return;
                    }
                }
                if (@return.GetResult().Count < 16)
                {
                    //MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("No recommendations found, try again later.");
                    GetRecommendations(tsk.Result); // lets retry
                    return;
                }
                @return.GetResult().Shuffle();
                Navigation.Get.Navigate("AudiosListView", new AudiosListViewModel(@return.GetResult()));
            }), TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void Play(object o)
        {
            MainViewModelLocator.PlaylistControl.CheckPlaylist(collection);
            MainViewModelLocator.BassPlayer.FindAndPlay(o.ToString());
        }
    }
}
