using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Filter.Contracts;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class FilterConfigViewModel : BaseViewModel, IFilterConfigViewModel
    {
        private ReadOnlyObservableCollection<Lazy<IPropoPlusFilter, IFilterMetadata>> _filterCollection;
        private Lazy<IPropoPlusFilter, IFilterMetadata> _selectedFilter;
        private bool _isEnabled;
        private ObservableCollection<IChannelData> _filteredChannelData;
        private ObservableCollection<IChannelData> _rawChannelData;

        public FilterConfigViewModel()
        {
            GlobalEventAggregator.Instance.AddListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);

            IsEnabled = Application.Instance.FilterManager.IsEnabled;

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

                Application.Instance.FilterManager.ChangeFilter(_selectedFilter.Value);

                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = Application.Instance.FilterManager.SetEnabled(value);

                OnPropertyChanged();
            }
        }

        private void ChannelsUpdateListener(ChannelsUpdateEventArgs args)
        {
            if (args == null)
                return;

            var rawChannelData = new int[16];
            Array.Copy(args.RawChannels, rawChannelData, Math.Min(args.RawCount, rawChannelData.Length));

            RawChannelData = new ObservableCollection<IChannelData>(rawChannelData.Select(s => new ChannelDataViewModel("", s)));

            var filteredChannelData = new int[16];
            Array.Copy(args.FilterChannels, filteredChannelData, Math.Min(filteredChannelData.Length, args.RawCount));

            FilteredChannelData = new ObservableCollection<IChannelData>(filteredChannelData.Select(s => new ChannelDataViewModel("", s)));
        }

        public ObservableCollection<IChannelData> RawChannelData
        {
            get => _rawChannelData;
            set
            {
                if (_rawChannelData == value)
                    return;

                _rawChannelData = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<IChannelData> FilteredChannelData
        {
            get => _filteredChannelData;
            set
            {
                if (_filteredChannelData == value)
                    return;

                _filteredChannelData = value;

                OnPropertyChanged();
            }
        }

        public override void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);

            base.Dispose();
        }
    }
}