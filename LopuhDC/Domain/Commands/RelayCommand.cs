using System;
using System.Windows.Input;

namespace LopuhDC.Domain.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _action;
        private readonly Action<object> _paramedAction;

        public RelayCommand(Action action)
        {
            _action = action;
        }

        public RelayCommand(Action<object> paramedAction)
        {
            _paramedAction = paramedAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action?.Invoke();
            _paramedAction?.Invoke(parameter);
        }
    }
}
