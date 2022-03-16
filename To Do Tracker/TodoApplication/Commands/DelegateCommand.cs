using System;

namespace TodoApplication.Commands
{
    public class DelegateCommand : IDelegateCommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
            this.canExecute = AlwaysCanExecute;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object param)
        {
            return canExecute(param);
        }

        public void Execute(object param)
        {
            execute(param);
        }

        bool AlwaysCanExecute(object param)
        {
            return true;
        }
    }
}
