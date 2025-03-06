using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Utilities;
using System;

namespace LopuhDC.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly MainContext _mainContext;

        public ViewModel CurrentViewModel => _mainContext.CurrentViewModel;

        public MainViewModel(MainContext mainContext)
        {
            _mainContext = mainContext;

            _mainContext.ViewModelChanged += OnViewModelChanged;
        }

        private void OnViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public override void Dispose()
        {
            _mainContext.ViewModelChanged -= OnViewModelChanged;

            GC.SuppressFinalize(this);
        }
    }
}
