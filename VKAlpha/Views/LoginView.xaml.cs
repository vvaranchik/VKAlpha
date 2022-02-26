using System.Windows.Controls;

namespace VKAlpha.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            Helpers.MainViewModelLocator.MainViewModel.SidebarVisible = false;
        }
    }
}
