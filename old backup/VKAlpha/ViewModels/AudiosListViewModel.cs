using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class AudiosListViewModel
    {
        private ObservableCollection<MonoVKLib.VK.Models.VKAudioModel> _collection = new ObservableCollection<MonoVKLib.VK.Models.VKAudioModel>();

        public ICommand PlayCommand { get; private set; }

        public ObservableCollection<MonoVKLib.VK.Models.VKAudioModel> collection { get => _collection; private set => _collection = value; }

        public AudiosListViewModel(long uid)
        {
            Init(uid);
            PlayCommand = new RelayCommand(Play);
        }

        public AudiosListViewModel(string query)
        {
            Init(query);
            PlayCommand = new RelayCommand(Play);
        }

        private async void Init(long uid)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            /*

                foreach (var audio in tsk.Result.Items)
                    if (!string.IsNullOrEmpty(audio.Url))
                        collection.Add(audio);
            */
            await Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.Get(uid);
            }).ContinueWith(tsk =>
            {
                if (tsk.Result.Items == null || tsk.Result.Items.FirstOrDefault() == default)
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Access denied!");
                    _Navigation.GoBack();
                    return;
                }
                tsk.Result.Items.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        collection.Add(a);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private async void Init(string query)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            await Task.Run(async () =>
            {
                return await MainViewModelLocator.Vk.VkAudio.Seach(query);
            }).ContinueWith(tsk =>
            {
                tsk.Result.Items.ForEach((a) =>
                {
                    if (!string.IsNullOrEmpty(a.Url))
                        collection.Add(a);
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        private void Play(object o)
        {
            MainViewModelLocator.PlaylistControl.CheckPlaylist(collection);
            MainViewModelLocator.BassPlayer.FindAndPlay(o.ToString());
        }
    }
}
