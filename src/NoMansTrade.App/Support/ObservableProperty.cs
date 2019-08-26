using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NoMansTrade.App.Support
{
    public class ObservableProperty<T> : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs sValueArgs = new PropertyChangedEventArgs(nameof(Value));
        private T mValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableProperty(T value)
        {
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

                this.PropertyChanged?.Invoke(this, sValueArgs);
            }
        }
    }
}
