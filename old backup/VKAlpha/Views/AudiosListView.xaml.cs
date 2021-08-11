using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VKAlpha.Extensions;

namespace VKAlpha.Views
{
    public partial class AudiosListView : UserControl
    {
        public AudiosListView()
        {
            InitializeComponent();
        }
        /*
        
        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AudiosList.ItemsSource != Helpers.MainViewModelLocator.PlaylistControl.VisiblePlaylist)
                AudiosList.ItemsSource = Helpers.MainViewModelLocator.PlaylistControl.VisiblePlaylist;
        }
        */
        private void tbSearchQuery_PreviewKeyUp(object sender, KeyEventArgs e)
        {
     //       if (e.Key != Key.Enter)
    //            return;
    //        if (tbSearchQuery.Text == "")
    //            return;
        //    if (AudiosList.ItemsSource != Helpers.MainViewModelLocator.PlaylistControl.VisiblePlaylist)
        //        AudiosList.ItemsSource = Helpers.MainViewModelLocator.PlaylistControl.VisiblePlaylist;
    //        DoSearch.Command.Execute(tbSearchQuery.Text);
        }
    }
}
