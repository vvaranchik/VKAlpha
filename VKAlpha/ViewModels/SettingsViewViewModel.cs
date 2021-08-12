using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;
using VKAlpha.Helpers;

namespace VKAlpha.ViewModels
{
    public class SettingsViewViewModel
    {
        public ICommand GoBack { get; private set; }

        public ICommand InterfaceMenu { get; private set; }

        public ICommand HotkeysMenu { get; private set; }

        public ICommand UpdateMenu { get; private set; }

        public ICommand AboutMenu { get; private set; }

        public SettingsViewViewModel()
        {
            GoBack = new RelayCommand((o) => { Navigation.Get.GoBackExtra(); MainViewModelLocator.MainViewModel.SidebarVisible = true; });
            InterfaceMenu = new RelayCommand((o) => SettingsNav.Get.Navigate("InterfaceView", new InterfaceMenuViewModel()));
            //HotkeysMenu = new RelayCommand((o) => SettingsNav.Navigate("HotkeysMenu"));
            //UpdateMenu = new RelayCommand((o) => SettingsNav.Navigate("UpdateMenu"));
            //AboutMenu = new RelayCommand((o) => SettingsNav.Navigate("AboutMenu"));
        }
    }
}
