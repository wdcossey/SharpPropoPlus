using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Events;
using SharpPropoPlus.Filter.Contracts;
using SharpPropoPlus.Filter.EventArguments;

namespace SharpPropoPlus.Filter
{
    public class FilterManager : IFilterManager
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IPropoPlusFilter, IFilterMetadata>> _filters;
#pragma warning restore 649

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;
        private IPropoPlusFilter _filter;
        private bool _isEnabled = false;

        public IEnumerable<Lazy<IPropoPlusFilter, IFilterMetadata>> Filters => _filters;

        public FilterManager()
        {
            //An aggregate catalog that combines multiple catalogs
            _catalog = new AggregateCatalog();

            //Add all the parts found in all assemblies in
            //the same directory as the executing program
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _catalog.Catalogs.Add(new DirectoryCatalog(path));

            //Create the CompositionContainer with the parts in the catalog.
            _container = new CompositionContainer(_catalog);

            //Fill the imports of this object
            _container.ComposeParts(this);

            _filters = _filters.OrderBy(ob => ob.Metadata.Name);

            Filter = GetDefaultFilter();
        }

        private IPropoPlusFilter GetDefaultFilter()
        {
            return Filters.First()?.Value;
        }

        public void ChangeFilter(IPropoPlusFilter filter)
        {
            Filter = filter;
        }

        public bool IsEnabled
        {
            get => _isEnabled;

            private set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = value;

                Notify();
            }
        }

        public bool SetEnabled(bool enabled)
        {
            return IsEnabled = enabled;
        }

        public void Notify()
        {
            var filter = GetFilter();

            GlobalEventAggregator.Instance.SendMessage(new FilterChangedEventArgs(IsEnabled, filter?.Metadata?.Name,
                filter?.Metadata?.Description, filter?.Value));
        }

        public IPropoPlusFilter Filter
        {
            get => _filter;

            private set
            {
                if (value == null || _filter == value)
                {
                    return;
                }

                _filter = value;

                var lazyFilter = GetFilter(_filter);

                var message = new FilterChangedEventArgs(IsEnabled, lazyFilter.Metadata.Name, lazyFilter.Metadata.Description, lazyFilter.Value);
                GlobalEventAggregator.Instance.SendMessage(message);
            }
        }

        private Lazy<IPropoPlusFilter, IFilterMetadata> GetFilter(IPropoPlusFilter filter = null)
        {
            return Filters.FirstOrDefault(fd => fd.Value == (filter ?? Filter));
        }

        public IFilterMetadata GetFilterMetadata(IPropoPlusFilter decoder)
        {
            return GetFilter()?.Metadata;
        }

        public void Dispose()
        {
            _catalog.Dispose();
            _container.Dispose();
        }
    }
}