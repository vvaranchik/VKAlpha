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
        public ICommand GoBack { get; set; }

        public SettingsViewViewModel()
        {
            GoBack = new RelayCommand((o) => { _Navigation.GoBack(); });
        }
    }
}
