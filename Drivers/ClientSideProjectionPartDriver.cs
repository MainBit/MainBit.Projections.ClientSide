using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Html;
using MainBit.Projections.ClientSide.Services;
using MainBit.Projections.ClientSide.Models.SortCriteria;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Utility.Extensions;

namespace MainBit.Projections.ClientSide.Drivers
{
    public class ClientSideProjectionPartDriver : ContentPartDriver<ClientSideProjectionPart>
    {
        private const string TemplateName = "Parts/ClientSideProjectionPart";
        private readonly IProjectionManager _projectionManager;
        private readonly UrlHelper _urlHepler;
        private readonly IClientSideSortService _clientSideSortService;
        private readonly IClientSideLayoutService _clientSideLayoutService;
        private readonly IUrlService _urlService;
        private readonly IRepository<QueryPartRecord> _queryRepository;
        
        public ClientSideProjectionPartDriver(
            IOrchardServices services,
            IProjectionManager projectionManager,
            UrlHelper urlHepler,
            IClientSideSortService clientSideSortService,
            IClientSideLayoutService clientSideLayoutService,
            IUrlService urlService,
            IRepository<QueryPartRecord> queryRepository)
        {

            _projectionManager = projectionManager;
            _urlHepler = urlHepler;
            _clientSideSortService = clientSideSortService;
            _clientSideLayoutService = clientSideLayoutService;
            _urlService = urlService;
            _queryRepository = queryRepository;
            

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Services = services;
        }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public IOrchardServices Services { get; set; }

        protected override string Prefix { get { return "ClientSideProjectionPart"; } }
        

        protected override DriverResult Display(ClientSideProjectionPart part, string displayType, dynamic shapeHelper)
        {
            var httpContext = _urlHepler.RequestContext.HttpContext;
            var currentUrl = _urlHepler.RequestContext.HttpContext.Request.RawUrl;
            
            var knowingKeys = new List<string>();
            part.Filters.ForEach(filter => knowingKeys.AddRange(filter.Editor.GetNames(filter)));
            knowingKeys.Add(ClientSideSortService.QueryStringParamName);
            knowingKeys.Add(ClientSideLayoutService.QueryStringParamName);

            var result = Combined(
                ContentShape("Parts_ClientSideProjectionPart_Filters", shape =>
                {
                    var filterShapes = shapeHelper.List();
                    var filterJson = new List<string>();
                    //part.Filters.ForEach(filter => );
                    foreach (var filter in part.Filters)
                    {
                        filterShapes.Add(filter.Editor.BuildDisplay(filter, shapeHelper));
                        filterJson.Add(filter.Editor.ToJsonString(filter));
                    }
                    return shapeHelper.Parts_ClientSideProjectionPart_Filters(
                        List: filterShapes,
                        ResultUrl: _urlHepler.ItemDisplayUrl(part),
                        FastSearchUrl: _urlHepler.Action("Index", "FastSearch", new { area = "MainBit.Projections.ClientSide", Id = part.Id }),
                        FiltersJsonString: "{" + string.Join(",", filterJson.ToArray()) + "}",
                        Count: _projectionManager.GetCount(part.Record.QueryPartRecord.Id)
                    );
                }),

                ContentShape("Parts_ClientSideProjectionPart_SortCriteria", shape =>
                {
                    var applyingOptions = part.SortCriteria
                        .Where(s => s.ApplyingPosition.HasValue)
                        .OrderBy(s => s.ApplyingPosition)
                        .Select(s => s.ApplyingOption);

                    var sortShapes = shapeHelper.List();
                    var viewModels = new List<ClientSideSortCriterionViewModel>();

                    var isFirstSort = true;
                    foreach (var sortCriterion in part.SortCriteria)
                    {
                        var viewModel = new ClientSideSortCriterionViewModel
                        {
                            ApplyingPosition = sortCriterion.ApplyingPosition,
                            DisplayName = sortCriterion.DisplayName,
                            Name = sortCriterion.Name,
                        };

                        foreach(var option in sortCriterion.Options) {
                            var entry = new ClientSideSortCriterionEntry {
                                DisplayName = option.DisplayName,
                                Value = option.Value,
                                Direction = option.Direction
                            };

                            if(sortCriterion.ApplyingOption == option) {
                                viewModel.ApplyingEntry = entry;
                            }

                            var newApplyingOption = new List<ClientSideSortCriterionOption> { option };
                            var optionsQueryString = _clientSideSortService.GetSortCriteraQueryString(newApplyingOption);
                            if (isFirstSort)
                            {
                                foreach (var key in optionsQueryString.AllKeys)
                                {
                                    optionsQueryString[key] = null;
                                }
                                isFirstSort = false;
                            }
                            entry.Url = _urlService.RebuildUrl(currentUrl, knowingKeys, optionsQueryString, true);

                            optionsQueryString = _clientSideSortService.GetSortCriteraQueryString(applyingOptions.Concat(newApplyingOption));
                            entry.AdditionalUrl = _urlService.RebuildUrl(currentUrl, knowingKeys, optionsQueryString, true);

                            viewModel.Entries.Add(entry);
                        }
                        
                        var sortShape = sortCriterion.Editor.BuildDisplay(sortCriterion, shapeHelper);
                        sortShape.ViewModel(viewModel);
                        sortShapes.Add(sortShape);
                        viewModels.Add(viewModel);
                    }

                    return shapeHelper.Parts_ClientSideProjectionPart_SortCriteria(
                        List: sortShapes,
                        ViewModel: viewModels
                    );
                }),
                ContentShape("Parts_ClientSideProjectionPart_Layouts", shape =>
                {
                    var viewModel = new List<ClientSideLayoutViewModel>();

                    var isFirstSort = true;
                    foreach (var layout in part.Layouts)
                    {
                        var layoutViewModel = new ClientSideLayoutViewModel
                        {
                            Name = layout.Name,
                            DisplayName = layout.DisplayName,
                            Applying = layout.Applying
                        };

                        var layoutQueryString = _clientSideLayoutService.GetLayoutQueryString(layout, true);
                        if (isFirstSort)
                        {
                            foreach (var key in layoutQueryString.AllKeys)
                            {
                                layoutQueryString[key] = null;
                            }
                            isFirstSort = false;
                        }

                        layoutViewModel.Url = _urlService.RebuildUrl(currentUrl, knowingKeys, layoutQueryString, true);
                        viewModel.Add(layoutViewModel);
                    }

                    return shapeHelper.Parts_ClientSideProjectionPart_Layouts(
                        Layouts: viewModel
                    );
                })
            );

            var drivers = Services.WorkContext.Resolve<IEnumerable<IContentPartDriver>>();
            var projectionPartDriver = drivers.First(d => d.GetType().FullName ==  "Orchard.Projections.Drivers.ProjectionPartDriver");

            var projectionPart = new ProjectionPart {
                Record = new ProjectionPartRecord {
                    DisplayPager = part.Record.DisplayPager,
                    Items = part.Record.Items,
                    MaxItems = part.Record.MaxItems,
                    PagerSuffix = part.Record.PagerSuffix,
                    Skip = part.Record.Skip,
                    QueryPartRecord = part.Record.QueryPartRecord
                },
                ContentItem = part.ContentItem
            };
            var applyingLayout = part.Layouts.FirstOrDefault(p => p.Applying);
            if(applyingLayout == null) {
                applyingLayout = part.Layouts.FirstOrDefault();
            }
            if (applyingLayout != null)
            {
                projectionPart.Record.LayoutRecord = part.Record.QueryPartRecord.Layouts.FirstOrDefault(l => l.Id == applyingLayout.LayoutRecordId);
            }

            //var context = new Orchard.ContentManagement.Handlers.BuildDisplayContext(null, projectionPart, displayType, "", Services.New);
            //var projectionDriverResult = projectionPartDriver.BuildDisplay(context);
            var displayMethod = projectionPartDriver.GetType().GetMethod("Display", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var displayMethodResult = (DriverResult)displayMethod.Invoke(projectionPartDriver, new[] { projectionPart, displayType, shapeHelper });

            return Combined(result, displayMethodResult);
        }
        protected override DriverResult Editor(ClientSideProjectionPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_ClientSideProjection_Edit", () =>
            {
                var model = new ClientSideProjectionPartEditViewModel
                {
                    PresetQueryString = part.Record.PresetQueryString,
                    DisplayPager = part.Record.DisplayPager,
                    Items = part.Record.Items,
                    Skip = part.Record.Skip,
                    PagerSuffix = part.Record.PagerSuffix,
                    MaxItems = part.Record.MaxItems,
                    QueryRecordId = -1,
                };

                if (part.Record.QueryPartRecord != null)
                {
                    model.QueryRecordId = part.Record.QueryPartRecord.Id;
                }

                model.QueryRecordEntries = Services.ContentManager.Query<QueryPart, QueryPartRecord>().Join<TitlePartRecord>().OrderBy(x => x.Title).List()
                    .Select(query => new QueryRecordEntry
                    {
                        Id = query.Id,
                        Name = query.Name
                    });

                return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix);
            });
        }

        protected override DriverResult Editor(ClientSideProjectionPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new ClientSideProjectionPartEditViewModel();

            if (updater.TryUpdateModel(model, Prefix, null, null))
            {
                if (model.PresetQueryString != null)
                {
                    model.PresetQueryString = model.PresetQueryString.Trim(new char[] { ' ', '&', '?' });
                }
                if (string.IsNullOrEmpty(model.PresetQueryString))
                {
                    part.Record.PresetQueryString = null;
                }
                else
                {
                    part.Record.PresetQueryString = string.Format("&{0}&", model.PresetQueryString);
                }

                part.Record.DisplayPager = model.DisplayPager;
                part.Record.Items = model.Items;
                part.Record.Skip = model.Skip;
                part.Record.MaxItems = model.MaxItems;
                part.Record.PagerSuffix = (model.PagerSuffix ?? String.Empty).Trim();
                part.Record.QueryPartRecord = _queryRepository.Get(model.QueryRecordId);

                if (!String.IsNullOrWhiteSpace(part.Record.PagerSuffix)
                    && !String.Equals(part.Record.PagerSuffix.ToSafeName(), part.Record.PagerSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    updater.AddModelError("PagerSuffix", T("Suffix should not contain special characters."));
                }
            }

            return Editor(part, shapeHelper);
        }
    }
}