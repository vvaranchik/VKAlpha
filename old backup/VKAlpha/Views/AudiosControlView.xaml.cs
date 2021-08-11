using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VKAlpha.Views
{
    public partial class AudiosControlView : UserControl
    {
        public AudiosControlView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AudiosControlViewViewModel();
        }
        
        

        private void scrollToItem(object sender, RoutedEventArgs e)
        {
            //if ((btnArtist.Content as TextBlock).Text != "Artist")
            {
                //if (GridContent.Children[0] == Helpers.Locator.AudiosListView)
                {
                //    if (Helpers.Locator.BassPlayer.CurrentTrack.OwnerId == Helpers.Locator.PlaylistControl.VisiblePlaylist[0].OwnerId)
                //        Helpers.Locator.AudiosListView.AudiosList.ScrollIntoView(Helpers.Locator.BassPlayer.CurrentTrack);
                }
            }
        }
    }
}