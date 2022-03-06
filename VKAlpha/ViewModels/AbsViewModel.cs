using System.ComponentModel;

namespace VKAlpha.ViewModels
{

    public interface IViewModel: INotifyPropertyChanged
    {
        System.Action<PropertyChangedEventArgs> RaisePropertyChanged();
    }

    public abstract class AbsViewModel : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => RaisePropertyChanged().Invoke(new PropertyChangedEventArgs(name));

        public System.Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
