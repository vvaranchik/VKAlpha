using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace VKAlpha.Dialogs
{
    public class WindowDialogs
    {
        private DialogSession _session = null;
        private readonly MainWindow _wnd;

        public WindowDialogs() 
        {
            _wnd = (MainWindow)App.Current.MainWindow;
        }

        public async Task OpenDialog(object content)
        {
            if (_session != null)
            {
                if (_session.Content == content)
                    return;
                if (!_session.IsEnded)
                    _session.Close();
                _session = null;
            }
            await _wnd.RootDialog.ShowDialog(content, (object _, DialogOpenedEventArgs args) => _session = args.Session);
        }

        public void CloseDialog()
        {
            if (_session != null)
            {
                if (!_session.IsEnded)
                    _session.Close();
                _session = null;
            }
        }
    }
}
