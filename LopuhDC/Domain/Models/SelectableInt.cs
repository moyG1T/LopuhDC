using LopuhDC.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LopuhDC.Domain.Models
{
    public class SelectableInt : ObservableObject
    {
        private bool _isSelected;

        public int Index { get; set; }

        public bool Loaded { get; set; }

        public bool IsSelected
        {
            get => _isSelected; set
            {
                _isSelected = value; OnPropertyChanged();
            }
        }

        public SelectableInt(int index)
        {
            Index = index;
        }

        public SelectableInt(int index, bool selected)
        {
            Index = index;
            IsSelected = selected;
        }
    }
}
