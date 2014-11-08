using MainBit.Projections.ClientSide.ClientSideEditors.Filters;
using MainBit.Projections.ClientSide.Descriptors;
using MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers;
using MainBit.Projections.ClientSide.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Forms.Services;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IAvaliableValuesService : IDependency
    {
        void UpdateForQuery(QueryPart query);
        void UpdateForQueries(IEnumerable<QueryPart> query);
        void UpdateForQuery(int queryId);
        void UpdateForQueries(IEnumerable<int> queryIds);

        IEnumerable<TypeDescriptor<FilterDescriptor>> DescribeFilterValueRetrivers();
    }

    public class AvaliableValuesService : IAvaliableValuesService
    {
        private readonly IQueryService _queryService;
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        private readonly IEnumerable<IFilterValueRetriverProvider> _filterValueRetriverProviders;
        private readonly IClientSideFilterEditorSelector _clientSideFilterEditorSelector;

        public AvaliableValuesService(IQueryService queryService,
            IContentManager contentManager,
            IProjectionManager projectionManager,
            IEnumerable<IFilterValueRetriverProvider> filterValueRetriverProviders)
        {
            _queryService = queryService;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
            _filterValueRetriverProviders = filterValueRetriverProviders;
        }

        public void UpdateForQuery(QueryPart query)
        {
            var clientSideFilters = query.FilterGroups.SelectMany(g => g.Filters).Where(filter => {
                var state = FormParametersHelper.FromString(filter.State);
                return ClientSideFilterFormHelper.IsForClientSide(state);
            });

            var parts = _contentManager
                .Query<ClientSideProjectionPart, ClientSideProjectionPartRecord>()
                .Where(p => p.QueryPartRecord == query.Record)
                .List();
            var part = parts.FirstOrDefault();

            if (part == null) { return; }

            var contentItems = _projectionManager.GetContentItems(query.Id).ToList();
            var avaliableValueRetrievers = DescribeFilterValueRetrivers();
            foreach (var filter in part.Filters)
            {

                var valueRetriever = avaliableValueRetrievers
                    .Where(p => p.Category == filter.Category)
                    .SelectMany(p => p.Descriptors)
                    .FirstOrDefault(d => d.Type == filter.Type);

                if (valueRetriever == null)
                {
                    continue;
                }

                filter.Editor.ClearAvaliableValues(filter);
                foreach (var contentItem in contentItems)
                {
                    var values = valueRetriever.RetrieveValues(contentItem);
                    filter.Editor.UpdateAvaliableValues(filter, values);
                }
            }

            foreach(var item in parts) {
                item.Record.Data = part.Infoset.Data;
            }
        }

        public void UpdateForQueries(IEnumerable<QueryPart> queries)
        {
            foreach (var query in queries)
            {
                UpdateForQuery(query);
            }
        }
        public void UpdateForQuery(int queryId)
        {
            var query = _queryService.GetQuery(queryId);
            UpdateForQuery(query);
        }
        public void UpdateForQueries(IEnumerable<int> queryIds)
        {
            var queries = _contentManager.GetMany<QueryPart>(queryIds, VersionOptions.Published, QueryHints.Empty).ToList();
            queries.ForEach(q => UpdateForQuery(q));
        }



        public IEnumerable<TypeDescriptor<FilterDescriptor>> DescribeFilterValueRetrivers()
        {
            var context = new DescribeFilterContext();
            if (_filterValueRetrivers == null)
            {
                foreach(var provider in _filterValueRetriverProviders) {
                    provider.Describe(context);
                }
                _filterValueRetrivers = context.Describe();
            }
            return _filterValueRetrivers;
        }
        private IEnumerable<TypeDescriptor<FilterDescriptor>> _filterValueRetrivers = null;
    }
}