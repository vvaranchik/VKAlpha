using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class LoginViewViewModel : INotifyPropertyChanged
    {
        private string _login;
        private string _pass;
        private string captchaSid;
        private string captchaKey;

        public ICommand iLogIn { get; set; }

        public string Login { get => _login; set => this.MutateVerbose(ref _login, value, RaisePropertyChanged()); }

        public string Password { get => _pass; set => this.MutateVerbose(ref _pass, value, RaisePropertyChanged()); }

        public LoginViewViewModel()
        {
            iLogIn = new RelayCommand(TryLogin, CanLogin);
        }

        private async void TryLogin(object o)
        {
            _ = MainViewModelLocator.WindowDialogs.OpenDialog(new Dialogs.Loading().LoadingDial.DialogContent);
            try
            {
                var toka = await MainViewModelLocator.Vk.VkAuth.Login(Login, Password, captchaSid, captchaKey);
                MainViewModelLocator.Settings.token = toka.Token;
                MainViewModelLocator.Settings.userid = toka.UserId;
                MainViewModelLocator.Settings.Save();
                Login = "";
                Password = "";
                _Navigation.Navigate("AudiosListView", new AudiosListViewModel(toka.UserId));
                MainViewModelLocator.MainViewModel.SidebarVisible = true;
                MainViewModelLocator.WindowDialogs.CloseDialog();
            }
            catch (MonoVKLib.VK.Exceptions.VKCaptchaRequired e)
            {
                var captcha = new Dialogs.CaptchaDialog();
                captcha.CaptchaImg.Source = await e.CaptchaImg.GetImageSource();
                await MainViewModelLocator.WindowDialogs.OpenDialog(captcha.CaptchaDial.DialogContent);
                captchaSid = e.CaptchaSid;
                captchaKey = captcha.CaptchaKey.Text;
                captcha = null;
                TryLogin(null); 
                return;
            }
            catch (MonoVKLib.VK.Exceptions.VKInvalidClient)
            {
                MainViewModelLocator.WindowDialogs.CloseDialog();
                MainViewModelLocator.MainViewModel.MessageQueue.Enqueue(MainViewModelLocator.AppLang.AuthFailed);
            }
        }

        private bool CanLogin(object o)
        {
            if (!string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password))
                return true;
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
