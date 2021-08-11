using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class FriendsListViewModel
    {
        private ObservableCollection<MonoVKLib.VK.Models.VKUserProfile> _collection = new ObservableCollection<MonoVKLib.VK.Models.VKUserProfile>();

        public ICommand GetFriendAudios { get; private set; }

        public ICommand GetFriendsOf { get; private set; }

        public ObservableCollection<MonoVKLib.VK.Models.VKUserProfile> collection { get => _collection; private set => _collection = value; }

        public FriendsListViewModel(long uid)
        {
            Init(uid);
            GetFriendsOf = new RelayCommand(LoadFriendsOfFriend);
            GetFriendAudios = new RelayCommand(LoadFriendAudios);
        }

        private async void Init(long uid)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                List<string> uids = new List<string>();
                var list = await MainViewModelLocator.Vk.VkPeople.Get(uid, "photo,photo_100,photo_400_orig,first_name,last_name", null, 0, 0, MonoVKLib.VK.Methods.People.VKPeople.SortingOrder.ByRating, MainViewModelLocator.AppLang.language);
                foreach (var profile in list.Items)
                {
                    uids.Add(profile.Id.ToString());
                }
                var list2 = await MainViewModelLocator.Vk.VkPeople.GetBaseUserInfo(uids, null, "gen", MainViewModelLocator.AppLang.language);
                foreach (var a in list.Items)
                {
                    foreach (var p in list2.Items)
                    {
                        if (a.Id == p.Id)
                        {
                            a.PhotoMax = MainViewModelLocator.AppLang.strFormatFriends.Replace("[friends]", MainViewModelLocator.AppLang.Friends).Replace("[user]", p.FirstName);
                            break;
                        }
                    }
                }
                return list;
                //return await MainViewModelLocator.Vk.VkPeople.Get(uid, "photo,photo_100,photo_400_orig,first_name,last_name", null, 0, 0, MonoVKLib.VK.Methods.People.VKPeople.SortingOrder.ByRating);
            }).ContinueWith((tsk) =>
            {
                if (tsk.Result.Items == null || tsk.Result.Items.FirstOrDefault() == default)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AccessDenied);
                    _Navigation.GoBack();
                    return;
                }
                tsk.Result.Items.ForEach(a =>
                {
                    collection.Add(a);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void LoadFriendAudios(object o)
        {
            _Navigation.Navigate("AudiosListView", new AudiosListViewModel((long)o));
            MainViewModelLocator.MainViewModel.LoadPlaylists((long)o, 1);
        }

        private void LoadFriendsOfFriend(object o)
        {
            _Navigation.Navigate("FriendsListView", new FriendsListViewModel((long)o));
        }
    }
}
