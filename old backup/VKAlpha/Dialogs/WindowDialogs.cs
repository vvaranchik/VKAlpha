using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace VKAlpha.Dialogs
{
    public class WindowDialogs
    {
        private DialogSession _session = null;

        public WindowDialogs() { }

        public async Task OpenDialog(object content)
        {
            if (_session != null)
            {
                if (!_session.IsEnded)
                    _session.Close();
                _session = null;
            }
            await ((MainWindow)App.Current.MainWindow).RootDialog.ShowDialog(content, (object o, DialogOpenedEventArgs args) => _session = args.Session);
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
