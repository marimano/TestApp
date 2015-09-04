using System.ComponentModel;

namespace TestAppMvvm.BaseImplementations
{
    /// <summary>
    /// Base class for all view models in the project
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;

            var args = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, args);
        }

        public void OnAllPropertyChanged()
        {
            OnPropertyChanged(null);
        }
    }
}
