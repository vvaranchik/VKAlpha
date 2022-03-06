using System.Windows.Controls;

namespace VKAlpha.Views
{
    public partial class AudiosListView : UserControl
    {
        public AudiosListView()
        {
            InitializeComponent();
        }

        private void IgnoreRightMouseClick(object _, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
