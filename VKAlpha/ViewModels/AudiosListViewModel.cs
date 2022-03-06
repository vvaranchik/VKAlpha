using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MonoVKLib.VK.Exceptions;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class AudiosListViewModel : AbsListViewModel<AudioModel>
    {
        string captchaSid = null, captchaKey = null;

        public ICommand PlayCommand { get; private set; }

        public ICommand GetRecommendationsCmd { get; private set; }

        private void CheckPlaying()
        {
            if(MainViewModelLocator.MainViewModel.CurrentTrack.OwnerId == Collection[0].OwnerId &&
                Collection.Count == MainViewModelLocator.PlaylistControl.PlayingPlaylist.Count)
            {
                var playlist = MainViewModelLocator.PlaylistControl.PlayingPlaylist;
                var currentTrack = playlist.FirstOrDefault(x => x.IsPlaying);
                if (AudioModel.IsAudioValid(currentTrack))
                {
                    currentTrack = Collection.First(x => x.Id == currentTrack.Id);
                    currentTrack.IsPlaying = true;
                }
            }
        }

        private void InitCommands()
        {
            PlayCommand = new RelayCommand(Play);
            GetRecommendationsCmd = new RelayCommand(GetRecommendations);
            _backup = Collection;
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

        public override void HandleDataChange(string query)
        {
            if (query == "")
            {
                Collection = _backup;
                MainViewModelLocator.PlaylistControl.CheckPlaylist(Collection);
                return;
            }
            var result = _backup.Where(x => x.FullData.Contains(query, StringComparison.InvariantCultureIgnoreCase));
            if (AudioModel.IsAudioValid(result.FirstOrDefault()))
            {
                Collection = new ObservableCollection<AudioModel>(result);
            }
            MainViewModelLocator.PlaylistControl.CheckPlaylist(Collection);
        }

        private void Init(ICollection<AudioModel> recommendations)
        {
            MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            foreach (var audio in recommendations)
            {
                if (!string.IsNullOrEmpty(audio.Url))
                    Collection.Add(audio);
            }
            MainViewModelLocator.WindowDialogs.CloseDialog();
            MainViewModelLocator.PlaylistControl.NullCheckPlaylist(Collection);
        }

        private async void Init(ulong uid, long albumId = 0)
        {
            MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                var result = albumId == 0 ? await MainViewModelLocator.Vk.VkAudio.Get((long)uid, captchaSid: captchaSid, captchaKey: captchaKey) : await MainViewModelLocator.Vk.VkAudio.Get((long)uid, albumId, captchaSid: captchaSid, captchaKey: captchaKey);
                captchaKey = captchaSid = null;
                return result;
            }).ContinueWith(async tsk =>
            {
                MainViewModelLocator.WindowDialogs.CloseDialog();
                var error = VKErrorProcessor.GetLastError();
                if (error.code != VKErrorProcessor.VKErrors.NO_ERROR)
                {
                    switch (error.code)
                    {
                        case VKErrorProcessor.VKErrors.Capthca:
                            var e = VKErrorProcessor.GetLastError().exception as VKCaptchaRequired;
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
                Collection.Clear();
                tsk.Result.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        Collection.Add(AudioModel.VKModelToAudio(a));
                });
                //if (uid == MainViewModelLocator.Vk.AccessToken.UserId && collection.Count > 100)
                //{
                //    var items = MainViewModelLocator.MainViewModel.SideBarItems[0].TreeContent;
                //    if (items[items.Count - 1].Name == "Get Recommendations") return;
                //    items.Add(new Controls.DrawerItem("Get Recommendations", GetRecommendationsCmd));
                //}
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            CheckPlaying();
            if (MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = false;
        }

        private async void Init(string query)
        {
            MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                var result = await MainViewModelLocator.Vk.VkAudio.Search(query, captchaSid: captchaSid, captchaKey: captchaKey);
                captchaKey = captchaSid = null;
                return result;
            }).ContinueWith(async (tsk) =>
            {
                MainViewModelLocator.WindowDialogs.CloseDialog();
                if (tsk.Result.IsEmpty())
                {
                    var error = VKErrorProcessor.GetLastError();
                    if (error.code != VKErrorProcessor.VKErrors.NO_ERROR)
                    {
                        switch (error.code)
                        {
                            case VKErrorProcessor.VKErrors.Capthca:
                                var e = VKErrorProcessor.GetLastError().exception as VKCaptchaRequired;
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
                        Collection.Add(AudioModel.VKModelToAudio(a));
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
            CheckPlaying();
            if (!MainViewModelLocator.MainViewModel.IsSearchActive) MainViewModelLocator.MainViewModel.IsSearchActive = true;
        }

        private async Task<Collection<AudioModel>> GetRandomTracks(uint n)
        {
            Collection<AudioModel> tracks = new Collection<AudioModel>();
            if (n > 0)
            {
                var audios = Collection[0].OwnerId == MainViewModelLocator.Settings.userid ? Collection : AudioModel.VKArrayToAudioCollection(await MainViewModelLocator.Vk.VkAudio.Get());
                for (var _ = 0; _ < n; _++)
                {
                    tracks.Add(audios.GetRandomElement());
                }
            }
            return tracks;
        }

        private async Task<Collection<AudioModel>> getRecommendations(Collection<AudioModel> prevCollection = null)
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
                                return tmp;
                            else if (VKErrorProcessor.GetLastError().code == VKErrorProcessor.VKErrors.InvalidClient)
                                return new Collection<AudioModel>();
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
            return tmp;
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
                return await getRecommendations();
            }).ContinueWith(new System.Action<Task<Collection<AudioModel>>>(async (tsk) =>
            {
                var @return = tsk.Result;
                MainViewModelLocator.WindowDialogs.CloseDialog();
                // need to move error processing to another method, probably to MainViewModelLocator.ProcessError(SafeReturn<> ret_val)
                if (@return == null || @return.Count == 0)
                {
                    var error = MonoSpotifyLib.Spotify.Exceptions.SpotifyErrorProcessor.GetLastError();
                    switch (error.code)
                    {
                        case MonoSpotifyLib.Spotify.Exceptions.Errors.InvalidMethod:
                            var e = VKErrorProcessor.GetLastError().exception as VKCaptchaRequired;
                            var captcha = new Dialogs.CaptchaDialog();
                            captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                            captchaSid = e.CaptchaSid;
                            await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                            captchaKey = captcha.CaptchaKey.Text;
                            GetRecommendations(@return);
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
                var recommendations = @return;
                if (recommendations.Count < 16)
                {
                    //MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("No recommendations found, try again later.");
                    GetRecommendations(recommendations); // lets retry
                    return;
                }
                recommendations.Shuffle();
                Navigation.Get.Navigate("AudiosListView", new AudiosListViewModel(recommendations));
            }), TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void Play(object o)
        {
            if (o != null)
            {
                MainViewModelLocator.MainViewModel.CurrentTrack.Cover = null;
                MainViewModelLocator.MainViewModel.CurrentTrack.IsPlaying = false;
                MainViewModelLocator.PlaylistControl.CheckPlaylist(Collection);
                MainViewModelLocator.MainViewModel.PlayTrack((long)o);
            }
        }
    }
}
