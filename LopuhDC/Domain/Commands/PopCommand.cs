using LopuhDC.Domain.Services;
using System;
using System.Windows.Input;

namespace LopuhDC.Domain.Commands
{
    public class PopCommand : ICommand
    {
        private readonly INavService _navService;

        public PopCommand(INavService navService)
        {
            _navService = navService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Pop();
        }
    }
}
