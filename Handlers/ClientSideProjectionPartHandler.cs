using MainBit.Projections.ClientSide.ClientSideEditors.Filters;
using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Models.Layouts;
using MainBit.Projections.ClientSide.Models.SortCriteria;
using MainBit.Projections.ClientSide.Services;
using MainBit.Projections.ClientSide.Storage.Providers;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Forms.Services;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace MainBit.Projections.ClientSide.Handlers
{
    public class ClientSideProjectionPartHandler: ContentHandler {

        private readonly IClientSideFilterEditorSelector _clientSideFilterEditorSelector;
        private readonly IClientSideSortEditorSelector _clientSideSortCriterionEditorSelector;
        private readonly IStorageProviderSelector _storageProviderSelector;
        private readonly IProjectionManager _projectionManager;

        public ClientSideProjectionPartHandler(IRepository<ClientSideProjectionPartRecord> repository,
            IClientSideFilterEditorSelector clientSideFilterEditorSelector,
            IClientSideSortEditorSelector clientSideSortCriterionEditorSelector,
            IStorageProviderSelector storageProviderSelector,
            IProjectionManager projectionManager)
        {
            Filters.Add(StorageFilter.For(repository));


            _clientSideFilterEditorSelector = clientSideFilterEditorSelector;
            _clientSideSortCriterionEditorSelector = clientSideSortCriterionEditorSelector;
            _storageProviderSelector = storageProviderSelector;
            _projectionManager = projectionManager;


            OnLoading<ClientSideProjectionPart>(LazyLoadHandlers);
            OnLoaded<ClientSideProjectionPart>((context, part) =>
            {
                part.Infoset.Data = part.Record.Data;
            });
        }

        void LazyLoadHandlers(LoadContentContext context, ClientSideProjectionPart part)
        {
            #region Filters loader
            part.FiltersField.Loader(() =>
            {
                var clientSideFilters = new List<ClientSideFilter>();
                if (part.Record.QueryPartRecord == null) { return clientSideFilters; }

                var avaliableFilters = _projectionManager.DescribeFilters();
                foreach (var record in part.Record.QueryPartRecord.FilterGroups.SelectMany(g => g.Filters.OrderBy(f => f.Position)))
                {
                    var state = FormParametersHelper.FromString(record.State);
                    if (!ClientSideFilterFormHelper.IsForClientSide(state))
                    {
                        continue;
                    }

                    var descriptor = avaliableFilters
                        .Where(x => x.Category == record.Category)
                        .SelectMany(x => x.Descriptors)
                        .FirstOrDefault(x => x.Type == record.Type);
                    if (descriptor == null)
                    {
                        continue;
                    }

                    var clientSideType = ClientSideFilterFormHelper.GetForClientSideType(state);
                    var clientSideFilterEditor = _clientSideFilterEditorSelector.GetEditor(descriptor.Form, clientSideType);
                    if (clientSideFilterEditor == null)
                    {
                        continue;
                    }

                    var name = ClientSideFilterFormHelper.GetName(state);
                    var storageProvider = _storageProviderSelector.GetProvider(record.Category, record.Type);
                    var storage = storageProvider.BindStorage(part, record.Category, record.Type);
                    var filter = clientSideFilterEditor.Factory(storage, state, name, record.Description, descriptor.Category, descriptor.Type);

                    clientSideFilters.Add(filter);
                }
                return clientSideFilters;
            });

            #endregion

            #region Sort scriteria loader
            part.SortCriteriaField.Loader(() =>
            {
                var clientSideSortCriteria = new List<ClientSideSortCriterion>();
                if (part.Record.QueryPartRecord == null) { return clientSideSortCriteria; }

                var avaliableSortCriteria = _projectionManager.DescribeSortCriteria();
                foreach (var record in part.Record.QueryPartRecord.SortCriteria.OrderBy(s => s.Position))
                {
                    var state = FormParametersHelper.FromString(record.State);
                    if (!ClientSideFilterFormHelper.IsForClientSide(state))
                    {
                        continue;
                    }

                    var descriptor = avaliableSortCriteria
                        .Where(x => x.Category == record.Category)
                        .SelectMany(x => x.Descriptors)
                        .FirstOrDefault(x => x.Type == record.Type);
                    if (descriptor == null)
                    {
                        continue;
                    }

                    var clientSideSortCriterionEditor = _clientSideSortCriterionEditorSelector.GetEditor(descriptor.Form);
                    if (clientSideSortCriterionEditor == null)
                    {
                        continue;
                    }

                    var name = ClientSideFilterFormHelper.GetName(state);
                    var sortCriterion = clientSideSortCriterionEditor.Factory(state, name, record.Description, descriptor.Category, descriptor.Type);

                    clientSideSortCriteria.Add(sortCriterion);
                }
                return clientSideSortCriteria;
            });
            #endregion

            #region Layouts loader
            part.LayoutsField.Loader(() =>
            {
                var clientSideLayouts = new List<ClientSideLayout>();
                if (part.Record.QueryPartRecord == null) { return clientSideLayouts; }

                foreach (var record in part.Record.QueryPartRecord.Layouts)
                {
                    var state = FormParametersHelper.FromString(record.State);
                    if (!ClientSideFilterFormHelper.IsForClientSide(state))
                    {
                        continue;
                    }

                    var clientSideLayout = new ClientSideLayout {
                        Name = ClientSideFilterFormHelper.GetName(state),
                        DisplayName = record.Description,

                        Category = record.Category,
                        Type = record.Type,
                        LayoutRecordId = record.Id
                    };

                    clientSideLayouts.Add(clientSideLayout);
                }

                return clientSideLayouts;
            });

            #endregion
        }
    }
}