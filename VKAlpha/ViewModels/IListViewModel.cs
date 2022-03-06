using System.Collections.ObjectModel;
using VKAlpha.Extensions;

namespace VKAlpha.ViewModels
{

    public interface IListViewModel : IViewModel
    {
        void HandleDataChange(string query);
    }

    public abstract class AbsListViewModel<T>: AbsViewModel, IListViewModel
    {
        private ObservableCollection<T> _collection = new ObservableCollection<T>();
        protected ObservableCollection<T> _backup;

        public ObservableCollection<T> Collection
        {
            get => _collection;
            set => this.MutateVerbose(ref _collection, value, RaisePropertyChanged(), nameof(Collection));
        }

        public bool Remove(T item)
        {
            if (Collection.Contains(item))
            {
                _backup.Remove(item);
                return Collection.Remove(item);
            }
            return false;
        }

        public virtual void HandleDataChange(string query) {}

    }
}
