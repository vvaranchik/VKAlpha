using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VKAlpha.Extensions;

namespace VKAlpha.Controls
{
    public sealed class DrawerItem : INotifyPropertyChanged 
    {
        private string _name;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);
        private ICommand _action;

        public string Name
        {
            get => _name;
            set => this.MutateVerbose(ref _name, value, RaisePropertyChanged());
        }

        public ICommand Action
        {
            get => _action;
            set => this.MutateVerbose(ref _action, value, RaisePropertyChanged());
        }

        public DrawerItem(string title, ICommand Action)
        {
            this.Name = title;
            this.Action = Action;
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }

    public class DrawerCategory : INotifyPropertyChanged
    {
        private string _name;
        private MaterialDesignThemes.Wpf.PackIconKind _icon;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);
        private ICommand _action;
        private ObservableCollection<DrawerItem> _treeContent;

        public DrawerCategory(string title, MaterialDesignThemes.Wpf.PackIconKind Icon, ICommand Action, params DrawerItem[] items)
        {
            this.Name = title;
            this.Icon = Icon;
            this.Action = Action;
            this.TreeContent = new ObservableCollection<DrawerItem>(items);
        }

        public DrawerCategory(string title, MaterialDesignThemes.Wpf.PackIconKind Icon, ICommand Action)
        {
            this.Name = title;
            this.Icon = Icon;
            this.Action = Action;
            this.TreeContent = new ObservableCollection<DrawerItem>();
        }

        public string Name
        {
            get => _name; 
            set => this.MutateVerbose(ref _name, value, RaisePropertyChanged()); 
        }

        public ICommand Action
        {
            get => _action; 
            set => this.MutateVerbose(ref _action, value, RaisePropertyChanged()); 
        }

        public MaterialDesignThemes.Wpf.PackIconKind Icon
        {
            get => _icon;
            set => this.MutateVerbose(ref _icon, value, RaisePropertyChanged()); 
        }

        public ObservableCollection<DrawerItem> TreeContent
        {
            get => _treeContent;
            set => this.MutateVerbose(ref _treeContent, value, RaisePropertyChanged());
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
