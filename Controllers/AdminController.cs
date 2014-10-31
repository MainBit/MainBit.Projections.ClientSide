using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Projections.Services;
using MainBit.Projections.ClientSide.Services;
using Orchard.Projections.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Forms.Services;
using MainBit.Projections.ClientSide.ViewModels;
using Orchard.UI.Navigation;
using Orchard;
using Orchard.Localization;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.Projections;
using System.Web.Routing;
using Orchard.UI.Notify;
using Orchard.Mvc;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Descriptors.Filter;

namespace MainBit.Projections.ClientSide.Controllers
{
    [ValidateInput(false), Admin]
    public class AdminController : Controller
    {
        private readonly IAvaliableValuesService _avaliableValuesService;
        private readonly ISiteService _siteService;
        private readonly IQueryService _queryService;
        private readonly IProjectionManager _projectionManager;

        public AdminController(
            IAvaliableValuesService avaliableValuesService,
            IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IQueryService queryService,
            IProjectionManager projectionManager)
        {
            _avaliableValuesService = avaliableValuesService;
            _siteService = siteService;
            _queryService = queryService;
            Services = services;
            _projectionManager = projectionManager;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index(AdminIndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to list queries")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            // default options
            if (options == null)
                options = new AdminIndexOptions();

            var queries = Services.ContentManager.Query<QueryPart>("Query");
            
            switch (options.Filter)
            {
                case QueriesFilter.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!String.IsNullOrWhiteSpace(options.Search))
            {
                queries = queries.Join<TitlePartRecord>().Where(r => r.Title.Contains(options.Search));
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(queries.Count());

            switch (options.Order)
            {
                case QueriesOrder.Name:
                    queries = queries.Join<TitlePartRecord>().OrderBy(u => u.Title);
                    break;
            }

            var results = queries
                .List()
                .Where(q => q.FilterGroups.SelectMany(g => g.Filters).Any(filter =>
                { 
                    var state = FormParametersHelper.FromString(filter.State);
                    return ClientSideFilterFormHelper.IsForClientSide(state);
                }))
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var model = new AdminIndexViewModel
            {
                Queries = results.Select(q => new QueryEntry
                {
                    Query = q.Record,
                    QueryId = q.Id,
                    Name = q.Name
                }).ToList(),
                Options = options,
                Pager = pagerShape
            };

            // maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            pagerShape.RouteData(routeData);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var viewModel = new AdminIndexViewModel { Queries = new List<QueryEntry>(), Options = new AdminIndexOptions() };
            UpdateModel(viewModel);

            var checkedItems = viewModel.Queries.Where(c => c.IsChecked);

            switch (viewModel.Options.BulkAction)
            {
                case QueriesBulkAction.None:
                    break;
                case QueriesBulkAction.UpdateAvaliableValues:
                    _avaliableValuesService.UpdateForQueries(checkedItems.Select(p => p.QueryId));
                    foreach (var checkedItem in checkedItems)
                    {
                        Services.Notifier.Information(T("Client side avaliable value were be updated for Query {0} ", checkedItem.QueryId ));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to manage queries")))
                return new HttpUnauthorizedResult();

            var query = _queryService.GetQuery(id);

            if (query == null)
            {
                return HttpNotFound();
            }

            _avaliableValuesService.UpdateForQuery(query);
            Services.Notifier.Information(T("Client side avaliable value were be updated for Query {0} ", query.Name));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageQueries, T("Not authorized to edit queries")))
                return new HttpUnauthorizedResult();

            var query = _queryService.GetQuery(id);
            var viewModel = new Orchard.Projections.ViewModels.AdminEditViewModel
            {
                Id = query.Id,
                Name = query.Name
            };

            #region Load Filters
            var filterGroupEntries = new List<Orchard.Projections.ViewModels.FilterGroupEntry>();
            var allFilters = _projectionManager.DescribeFilters().SelectMany(x => x.Descriptors).ToList();

            foreach (var group in query.FilterGroups)
            {
                var filterEntries = new List<Orchard.Projections.ViewModels.FilterEntry>();

                foreach (var filter in group.Filters.OrderBy(s => s.Position))
                {
                    var category = filter.Category;
                    var type = filter.Type;

                    var f = allFilters.FirstOrDefault(x => category == x.Category && type == x.Type);
                    if (f != null)
                    {
                        filterEntries.Add(
                            new Orchard.Projections.ViewModels.FilterEntry
                            {
                                Category = f.Category,
                                Type = f.Type,
                                FilterRecordId = filter.Id,
                                DisplayText = String.IsNullOrWhiteSpace(filter.Description) ? f.Display(new FilterContext { State = FormParametersHelper.ToDynamic(filter.State) }).Text : filter.Description
                            });
                    }
                }

                filterGroupEntries.Add(new Orchard.Projections.ViewModels.FilterGroupEntry { Id = group.Id, Filters = filterEntries });
            }

            viewModel.FilterGroups = filterGroupEntries;

            #endregion

            #region Load Sort criterias
            var sortCriterionEntries = new List<Orchard.Projections.ViewModels.SortCriterionEntry>();
            var allSortCriteria = _projectionManager.DescribeSortCriteria().SelectMany(x => x.Descriptors).ToList();

            foreach (var sortCriterion in query.SortCriteria.OrderBy(s => s.Position))
            {
                var category = sortCriterion.Category;
                var type = sortCriterion.Type;

                var f = allSortCriteria.FirstOrDefault(x => category == x.Category && type == x.Type);
                if (f != null)
                {
                    sortCriterionEntries.Add(
                        new Orchard.Projections.ViewModels.SortCriterionEntry
                        {
                            Category = f.Category,
                            Type = f.Type,
                            SortCriterionRecordId = sortCriterion.Id,
                            DisplayText = String.IsNullOrWhiteSpace(sortCriterion.Description) ? f.Display(new SortCriterionContext { State = FormParametersHelper.ToDynamic(sortCriterion.State) }).Text : sortCriterion.Description
                        });
                }
            }

            viewModel.SortCriteria = sortCriterionEntries;

            #endregion

            #region Load Layouts
            var layoutEntries = new List<Orchard.Projections.ViewModels.LayoutEntry>();
            var allLayouts = _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).ToList();

            foreach (var layout in query.Layouts)
            {
                var category = layout.Category;
                var type = layout.Type;

                var f = allLayouts.FirstOrDefault(x => category == x.Category && type == x.Type);
                if (f != null)
                {
                    layoutEntries.Add(
                        new Orchard.Projections.ViewModels.LayoutEntry
                        {
                            Category = f.Category,
                            Type = f.Type,
                            LayoutRecordId = layout.Id,
                            DisplayText = String.IsNullOrWhiteSpace(layout.Description) ? f.Display(new LayoutContext { State = FormParametersHelper.ToDynamic(layout.State) }).Text : layout.Description
                        });
                }
            }

            viewModel.Layouts = layoutEntries;

            #endregion

            return View(viewModel);
        }
    }
}