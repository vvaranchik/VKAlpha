using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VKAlpha.Helpers
{
    public sealed class Navigation : BaseSingleton<Navigation>
    {
        private readonly MainWindow win = (App.Current.MainWindow as MainWindow);

        private int NavCount = 0;
        private NavigationService _navService;
        private object prevPage;

        bool clearStack { get; set; } = false;

        public NavigationService Service
        {
            get => _navService;
            set
            {
                if (_navService != null)
                {
                    _navService.Navigated -= _navService_Navigated;
                    _navService.Navigating -= _navService_Navigating;
                }

                _navService = value;
                _navService.Navigated += _navService_Navigated;
                _navService.Navigating += _navService_Navigating;
            }
        }

        private void _navService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (win.FrameMain.Content is Views.LoginView) // || (FrameMain.Content is Views.AudiosListView && MainViewModelLocator.PlaylistControl.VisiblePlaylist[0].OwnerId == MainViewModelLocator.Vk.AccessToken.UserId))
            {
                clearStack = true;
            }
        }

        private void _navService_Navigated(object o, NavigationEventArgs e)
        {
            NavCount++;
            if (clearStack)
            {
                ClearStack();
            }
            if (NavCount > 6)
            {
                ClearStack();
            }
            if (e.Content == null)
                return;
            if (e.ExtraData != null)
                (e.Content as UserControl).DataContext = e.ExtraData;
        }

        public void ClearStack()
        {
            clearStack = false;
            while (_navService.CanGoBack)
            {
                _navService.RemoveBackEntry();
            }
            NavCount = 0;
        }
        
        public void SubViewNavigate(string to, object extraData)
        {
            var type = Type.GetType($"VKAlpha.Views.SubViews.{to}", false);
            var page = Activator.CreateInstance(type);
            prevPage = _navService.Content;
            _navService.Navigate(page, extraData);
        }

        public void Navigate(string to, object extraData)
        {
            var type = Type.GetType($"VKAlpha.Views.{to}", false);
            var page = Activator.CreateInstance(type);
            prevPage = _navService.Content;
            _navService.Navigate(page, extraData);
        }

        public void Navigate(string to)
        {
            Navigate(to, null);
        }

        public void GoToSettings()
        {
            Type type = Type.GetType("VKAlpha.Views.SettingsView", false);
            prevPage = _navService.Content;
            var page = Activator.CreateInstance(type);
            instance._navService.Navigate(page, new ViewModels.SettingsViewViewModel());
        }

        public void GoBackExtra()
        {
            _navService.Navigate(prevPage);
            prevPage = null;
        }

        public void GoBack()
        {
            if (!_navService.CanGoBack)
                return;
            _navService.GoBack();
        }

        public void GoForward()
        {
            if (!_navService.CanGoForward)
                return;
            _navService.GoForward();
        }
    }

    public sealed class SettingsNav : BaseSingleton<SettingsNav>
    {
        private NavigationService _navService;

        public NavigationService Service
        {
            get { return _navService; }
            set
            {
                if (_navService != null)
                {
                    _navService.Navigated -= _navService_Navigated;
                    _navService.Navigating -= _navService_Navigating;
                }

                _navService = value;
                _navService.Navigated += _navService_Navigated;
                _navService.Navigating += _navService_Navigating;
            }
        }

        private void _navService_Navigating(object sender, NavigatingCancelEventArgs e) { }

        private void _navService_Navigated(object o, NavigationEventArgs e)
        {
            if (e.Content == null)
                return;
            if (e.ExtraData != null)
                (e.Content as UserControl).DataContext = e.ExtraData;
        }

        public void Navigate(string to, object extraData)
        {
            Type type = Type.GetType("VKAlpha.Views.SubViews." + to, false);
            if (_navService.Content != null )
            {
                if (_navService.Content.ToString() == $"VKAlpha.Views.SubViews.{to}")
                    return;
            }
            var page = Activator.CreateInstance(type);
            _navService.Navigate(page, extraData);
        }

        public void Navigate(string to)
        {
            Navigate(to, null);
        }
    }
}
