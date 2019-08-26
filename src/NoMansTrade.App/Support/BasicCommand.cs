using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoMansTrade.App.Support
{
    class BasicCommand : ICommand
    {
        private readonly Dispatcher mDispatcher;
        private readonly Action<object> mExecute;
        private readonly Func<object, bool> mCanExecute;

        public event EventHandler? CanExecuteChanged;

        public BasicCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            mDispatcher = Dispatcher.CurrentDispatcher;
            mExecute = execute;
            mCanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return mCanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            mExecute(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            mDispatcher.BeginInvoke(() => 
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }
    }
}
