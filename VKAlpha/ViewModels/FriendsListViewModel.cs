using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class FriendsListViewModel : AbsListViewModel<MonoVKLib.VK.Models.VKUserProfile>
    {
        public ICommand GetFriendAudios { get; private set; }

        public ICommand GetFriendsOf { get; private set; }

        public FriendsListViewModel(ulong uid)
        {
            Init(uid);
            _backup = Collection;
            GetFriendsOf = new RelayCommand(LoadFriendsOfFriend);
            GetFriendAudios = new RelayCommand(LoadFriendAudios);
        }

        public override void HandleDataChange(string query)
        {
            if (query == "")
            {
                Collection = _backup;
                return;
            }
            var result = _backup.Where(x => x.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase));
            if (result != null && result.FirstOrDefault() != default)
            {
                Collection = new ObservableCollection<MonoVKLib.VK.Models.VKUserProfile>(result);
            }
        }

        private async void Init(ulong uid)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                List<string> uids = new List<string>();
                var list = await MainViewModelLocator.Vk.VkPeople.Get(uid, "photo,photo_100,photo_400_orig,first_name,last_name", null, 0, 0, MonoVKLib.VK.Methods.People.SortingOrder.ByRating, MainViewModelLocator.AppLang.language);
                if (list.IsEmpty())
                {
                    return list;
                }
                list.ForEach((profile) => uids.Add(profile.Id.ToString()));
                foreach (MonoVKLib.VK.Models.VKUserProfile profile in list)
                {
                    uids.Add(profile.Id.ToString());
                }
                var list2 = await MainViewModelLocator.Vk.VkPeople.GetBaseUserInfo(uids, null, "gen", MainViewModelLocator.AppLang.language);
                foreach (MonoVKLib.VK.Models.VKUserProfile a in list)
                {
                    foreach (MonoVKLib.VK.Models.VKUserProfile p in list2)
                    {
                        if (a.Id == p.Id)
                        {
                            a.PhotoMax = MainViewModelLocator.AppLang.strFormatFriends.Replace("[friends]", MainViewModelLocator.AppLang.Friends).Replace("[user]", p.FirstName);
                            break;
                        }
                    }
                }
                return list;
            }).ContinueWith((tsk) =>
            {
                if (tsk.Result.IsEmpty())
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AccessDenied);
                    if (!Navigation.Get.Service.CanGoBack) { Navigation.Get.GoBackExtra(); return; }
                    Navigation.Get.GoBack();
                    return;
                }
                tsk.Result.ForEach(a =>
                {
                    Collection.Add(a);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void LoadFriendAudios(object o)
        {
            Navigation.Get.Navigate("AudiosListView", new AudiosListViewModel(ulong.Parse(o.ToString())));
            if (MainViewModelLocator.MainViewModel.IsShuffled) MainViewModelLocator.PlaylistControl.RequstShuffle = true;
            MainViewModelLocator.MainViewModel.LoadPlaylists(ulong.Parse(o.ToString()), 1);
        }

        private void LoadFriendsOfFriend(object o)
        {
            Navigation.Get.Navigate("FriendsListView", new FriendsListViewModel(ulong.Parse(o.ToString())));
        }
    }
}
