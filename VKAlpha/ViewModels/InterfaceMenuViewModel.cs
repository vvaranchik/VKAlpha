using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VKAlpha.Extensions;

namespace VKAlpha.ViewModels
{
    public class InterfaceMenuViewModel
    {
        public ICommand SetTheme { get; private set; }

        public InterfaceMenuViewModel()
        {
            SetTheme = new RelayCommand((o)=> { });
        }

    }
}
