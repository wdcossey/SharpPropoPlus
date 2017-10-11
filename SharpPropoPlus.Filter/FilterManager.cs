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

        public IEnumerable<Lazy<IPropoPlusFilter, IFilterMetadata>> Filters => _filters;

        public FilterManager()
        {
            GlobalEventAggregator.Instance.AddListener<FilterChangedEventArgs>(FilterChangedListener);
            GlobalEventAggregator.Instance.AddListener<FilterStateEventArgs>(FilterStateListener);

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

            Filter = null;//GetDefaultFilter();
        }

        private void FilterStateListener(FilterStateEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            if (args.IsEnabled)
            {
                if (Filter == null)
                {
                    Filter = GetDefaultFilter();
                }
            }

            Notify();
        }

        private void FilterChangedListener(FilterChangedEventArgs args)
        {
            ChangeDecoder(args.Filter);
        }

        private IPropoPlusFilter GetDefaultFilter()
        {
            return Filters.First()?.Value;
        }

        public void ChangeDecoder(IPropoPlusFilter filter)
        {
            Filter = filter;
        }

        public void Notify()
        {
            var filter = Filters.FirstOrDefault(fd => fd.Value == Filter);

            GlobalEventAggregator.Instance.SendMessage(new FilterChangedEventArgs(filter?.Metadata?.Name,
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
            }
        }

        public void Dispose()
        {
            _catalog.Dispose();
            _container.Dispose();
        }
    }
}