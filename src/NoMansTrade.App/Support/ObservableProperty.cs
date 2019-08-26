using System.ComponentModel;
using System.Windows.Threading;

namespace NoMansTrade.App.Support
{
    public class ObservableProperty<T> : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs sValueArgs = new PropertyChangedEventArgs(nameof(Value));
        private readonly Dispatcher mDispatcher;
        private T mValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableProperty(T value)
        {
            mDispatcher = Dispatcher.CurrentDispatcher;

            mValue = value;
        }

        public T Value
        {
            get => mValue;
            set
            {
                var valueIsNull = value == null;
                var equalNulity = (valueIsNull) == (mValue == null);

                if (equalNulity && (valueIsNull || mValue!.Equals(value)))
                {
                    return;
                }

                mValue = value;

                mDispatcher.BeginInvoke(
                    () => this.PropertyChanged?.Invoke(this, sValueArgs));
            }
        }
    }
}
