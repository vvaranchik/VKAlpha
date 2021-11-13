using System.Windows.Controls;

namespace VKAlpha.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            Helpers.SettingsNav.Get.Service = SettingsFrame.NavigationService;
            Helpers.SettingsNav.Get.Navigate("InterfaceView", new ViewModels.InterfaceMenuViewModel());
        }
    }
}
