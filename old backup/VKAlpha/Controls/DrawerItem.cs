using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VKAlpha.Extensions;

namespace VKAlpha.Controls
{
    public class DrawerItem : INotifyPropertyChanged
    {
        private string _name;
        private object _content;
        private MaterialDesignThemes.Wpf.PackIconKind _icon;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);
        private ICommand _action;


        [Obsolete]
        public DrawerItem(string name, object content, MaterialDesignThemes.Wpf.PackIconKind icon)
        {
            _name = name;
            Content = content;
            Icon = icon;
        }

        public DrawerItem(string title, MaterialDesignThemes.Wpf.PackIconKind Icon)
        {
            this.Name = title;
            this.Icon = Icon;
        }

        public DrawerItem(string title, MaterialDesignThemes.Wpf.PackIconKind Icon, ICommand Action)
        {
            this.Name = title;
            this.Icon = Icon;
            this.Action = Action;
        }

        public string Name
        {
            get { return _name; }
            set { this.MutateVerbose(ref _name, value, RaisePropertyChanged()); }
        }

        public ICommand Action
        {
            get { return _action; }
            set { this.MutateVerbose(ref _action, value, RaisePropertyChanged()); }
        }

        public MaterialDesignThemes.Wpf.PackIconKind Icon
        {
            get { return _icon; }
            set { this.MutateVerbose(ref _icon, value, RaisePropertyChanged()); }
        }

        public object Content
        {
            get { return _content; }
            set { this.MutateVerbose(ref _content, value, RaisePropertyChanged()); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get { return _horizontalScrollBarVisibilityRequirement; }
            set { this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value, RaisePropertyChanged()); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get { return _verticalScrollBarVisibilityRequirement; }
            set { this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value, RaisePropertyChanged()); }
        }

        public Thickness MarginRequirement
        {
            get { return _marginRequirement; }
            set { this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
