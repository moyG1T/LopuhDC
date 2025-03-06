using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LopuhDC.Domain.Services
{
    public interface INavService
    {
        void Push();
        void Pop();
        void PopAllAndPush();
    }
}
