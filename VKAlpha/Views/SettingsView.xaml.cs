﻿using System.Windows.Controls;

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
            Helpers.SettingsNav.Service = SettingsFrame.NavigationService;
            Helpers.SettingsNav.Navigate("InterfaceView");
        }
    }
}