using LopuhDC.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LopuhDC.Domain.Commands
{
    public class PushCommand : ICommand
    {
        private readonly INavService _navService;

        public PushCommand(INavService navService)
        {
            _navService = navService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _navService.Push();
        }
    }
}
