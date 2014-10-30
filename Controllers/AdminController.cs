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

namespace MainBit.Projections.ClientSide.Controllers
{
    [ValidateInput(false), Admin]
    public class AdminController : Controller
    {
        private readonly IAvaliableValuesService _avaliableValuesService;
        private readonly ISiteService _siteService;
        private readonly IQueryService _queryService;

        public AdminController(
            IAvaliableValuesService avaliableValuesService,
            IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IQueryService queryService)
        {
            _avaliableValuesService = avaliableValuesService;
            _siteService = siteService;
            _queryService = queryService;
            Services = services;

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
    }
}