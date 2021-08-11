using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VKAlpha.Helpers
{
    public sealed class _Navigation
    {
        private static readonly MainWindow win = (App.Current.MainWindow as MainWindow);

        private static int NavCount = 0;

        private static volatile _Navigation instance;
        private static object syncRoot = new object();
        private NavigationService _navService;
        private static object prevPage;

        public bool clearStack { get; set; } = false;

        private _Navigation() {}

        private static _Navigation Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new _Navigation();
                        }
                    }
                }
                return instance;
            }
        }

        public static NavigationService Service
        {
            get { return Instance._navService; }
            set
            {
                if (Instance._navService != null)
                {
                    Instance._navService.Navigated -= Instance._navService_Navigated;
                    Instance._navService.Navigating -= Instance._navService_Navigating;
                }

                Instance._navService = value;
                Instance._navService.Navigated += Instance._navService_Navigated;
                Instance._navService.Navigating += Instance._navService_Navigating;
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
                clearStack = false;
                ClearStack();
            }
            // 
            if (NavCount > 6)
            {
                while (Service.CanGoBack) Service.RemoveBackEntry();
                NavCount = 0;
            }
            if (e.Content == null)
                return;
            if (e.ExtraData != null)
                (e.Content as UserControl).DataContext = e.ExtraData;
        }

        public static void ClearStack()
        {
            while (Instance._navService.CanGoBack)
            {
                Instance._navService.RemoveBackEntry();
            }
            NavCount = 0;
        }

        public static void Navigate(string to, object extraData)
        {
            var type = Type.GetType("VKAlpha.Views." + to, false);
            var page = Activator.CreateInstance(type);
            Instance._navService.Navigate(page, extraData);
        }

        public static void Navigate(string to)
        {
            Navigate(to, null);
        }

        public static void GoToSettings()
        {
            Type type = Type.GetType("VKAlpha.Views.SettingsView", false);
            prevPage = Instance._navService.Content;
            var page = Activator.CreateInstance(type);
            Instance._navService.Navigate(page, new ViewModels.SettingsViewViewModel());
        }

        public static void GoBackExtra()
        {
            Instance._navService.Navigate(prevPage);
            prevPage = null;
            return;
        }

        public static void GoBack()
        {
            if (!Instance._navService.CanGoBack)
                return;
            Instance._navService.GoBack();
        }

        public static void GoForward()
        {
            if (!Instance._navService.CanGoForward)
                return;
            Instance._navService.GoForward();
        }
    }

    public sealed class SettingsNav 
    {
        private static volatile SettingsNav instance;
        private static object syncRoot = new object();

        private NavigationService _navService;

        private SettingsNav() { }

        private static SettingsNav Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SettingsNav();
                        }
                    }
                }
                return instance;
            }
        }

        public static NavigationService Service
        {
            get { return Instance._navService; }
            set
            {
                if (Instance._navService != null)
                {
                    Instance._navService.Navigated -= Instance._navService_Navigated;
                    Instance._navService.Navigating -= Instance._navService_Navigating;
                }

                Instance._navService = value;
                Instance._navService.Navigated += Instance._navService_Navigated;
                Instance._navService.Navigating += Instance._navService_Navigating;
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

        public static void Navigate(string to, object extraData)
        {
            Type type = Type.GetType("VKAlpha.Views.SubViews." + to, false);
            var page = Activator.CreateInstance(type);
            Instance._navService.Navigate(page, extraData);
        }

        public static void Navigate(string to)
        {
            Navigate(to, null);
        }
    }
}
