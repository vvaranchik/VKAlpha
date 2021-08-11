using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace VKAlpha
{
    public partial class App : Application
    {
        public static IntPtr MainWindowHandle;
        public static SynchronizationContext UISync;

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
            MainWindowHandle = new WindowInteropHelper(Current.MainWindow as MainWindow).Handle;
            UISync = SynchronizationContext.Current;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Helpers.MainViewModelLocator.BassPlayer.Stop(true);
            base.OnExit(e);
        }

    }
}