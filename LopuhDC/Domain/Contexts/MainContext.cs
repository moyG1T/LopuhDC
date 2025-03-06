using LopuhDC.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LopuhDC.Domain.Contexts
{
    public class MainContext
    {
        private readonly Stack<ViewModel> history = new Stack<ViewModel>();
        public ViewModel CurrentViewModel => history.Peek();

        public event Action ViewModelChanged;

        public void PushViewModel(ViewModel viewModel)
        {
            history.Push(viewModel);

            ViewModelChanged?.Invoke();
        }

        public void PopViewModel()
        {
            if (history.Count > 1)
            {
                var disposingViewModel = history.Pop();
                disposingViewModel.Dispose();

                ViewModelChanged?.Invoke();
            }
        }

        public void ClearAndPushViewModel(ViewModel viewModel)
        {
            if (history.Any())
            {
                foreach (var disposingViewModel in history)
                {
                    disposingViewModel.Dispose();
                }
                history.Clear(); 
            }

            history.Push(viewModel);

            ViewModelChanged?.Invoke();
        }
    }
}
