using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LopuhDC.Domain.Services
{
    public class MainNavService : INavService
    {
        private readonly MainContext _mainContext;
        private readonly Func<ViewModel> _func;

        public MainNavService(MainContext mainContext, Func<ViewModel> func = null)
        {
            _mainContext = mainContext;
            _func = func;
        }

        public void Pop()
        {
            _mainContext.PopViewModel();
        }

        public void PopAllAndPush()
        {
            _mainContext.ClearAndPushViewModel(_func());
        }

        public void Push()
        {
            _mainContext.PushViewModel(_func());
        }
    }
}
