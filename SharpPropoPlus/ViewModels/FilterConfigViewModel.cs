using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class FilterConfigViewModel : BaseViewModel, IFilterConfigViewModel
    {
        private ReadOnlyObservableCollection<Lazy<IPropoPlusFilter, IFilterMetadata>> _filterCollection;
        private Lazy<IPropoPlusFilter, IFilterMetadata> _selectedFilter;

        public FilterConfigViewModel()
        {
            FilterCollection =
                new ReadOnlyObservableCollection<Lazy<IPropoPlusFilter, IFilterMetadata>>(
                    new ObservableCollection<Lazy<IPropoPlusFilter, IFilterMetadata>>(Application.Instance
                        .FilterManager.Filters.ToList()));

            SelectedFilter =
                FilterCollection.FirstOrDefault(fd => fd.Value == Application.Instance.FilterManager.Filter);
        }

        public ReadOnlyObservableCollection<Lazy<IPropoPlusFilter, IFilterMetadata>> FilterCollection
        {
            get => _filterCollection;
            private set
            {
                _filterCollection = value;
                OnPropertyChanged();
            }
        }

        public Lazy<IPropoPlusFilter, IFilterMetadata> SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter == value)
                {
                    return;
                }

                _selectedFilter = value;

                //Application.Instance.FilterManager.ChangeFilter(value);

                OnPropertyChanged();
            }
        }
    }
}