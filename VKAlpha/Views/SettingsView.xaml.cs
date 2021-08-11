using System.Windows.Controls;

namespace VKAlpha.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            Helpers.SettingsNav.Get.Service = SettingsFrame.NavigationService;
            Helpers.SettingsNav.Get.Navigate("InterfaceView");
        }
    }
}
