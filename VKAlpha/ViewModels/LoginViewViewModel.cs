using System;
using System.ComponentModel;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;
using MonoVKLib.VK.Exceptions;

namespace VKAlpha.ViewModels
{
    public class LoginViewViewModel : INotifyPropertyChanged
    {
        private string _login;
        private string _pass;
        private string captchaSid;
        private string captchaKey;

        public ICommand iLogIn { get; }

        public string Login { get => _login; set => this.MutateVerbose(ref _login, value, RaisePropertyChanged()); }

        public string Password { get => _pass; set => this.MutateVerbose(ref _pass, value, RaisePropertyChanged()); }

        private bool CanLogin(object o) => !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);

        public LoginViewViewModel()
        {
            iLogIn = new RelayCommand(TryLogin, CanLogin);
        }

        private async void TryLogin(object _ = null)
        {
            MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            var result = await MainViewModelLocator.Vk.VkAuth.Login(Login, Password, captchaSid, captchaKey);
            if (!result)
            {
                switch (VKErrorProcessor.GetLastError().code)
                {
                    case VKErrorProcessor.VKErrors.Capthca:
                        var e = (VKErrorProcessor.GetLastError().exception as VKCaptchaRequired);
                        var captcha = new Dialogs.CaptchaDialog();
                        captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                        await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                        captchaSid = e.CaptchaSid;
                        captchaKey = captcha.CaptchaKey.Text;
                        TryLogin();
                        break;
                    case VKErrorProcessor.VKErrors.InvalidClient:
                        MainViewModelLocator.WindowDialogs.CloseDialog();
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AuthFailed);
                        Password = "";
                        break;
                }
                return;
            }
            MainViewModelLocator.Settings.token = MainViewModelLocator.Vk.AccessToken.Token;
            MainViewModelLocator.Settings.userid = (long)MainViewModelLocator.Vk.AccessToken.UserId;
            MainViewModelLocator.Settings.Save();
            Login = "";
            Password = "";
            Navigation.Get.Navigate("AudiosListView", new AudiosListViewModel(MainViewModelLocator.Vk.AccessToken.UserId));
            MainViewModelLocator.MainViewModel.LoadPlaylists();
            MainViewModelLocator.MainViewModel.SidebarVisible = true;
            //MainViewModelLocator.WindowDialogs.CloseDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
