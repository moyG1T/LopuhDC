using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LopuhDC.Domain.Utilities
{
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public abstract void Dispose();
        public void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
