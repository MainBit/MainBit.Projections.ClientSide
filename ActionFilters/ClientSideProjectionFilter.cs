using MainBit.Projections.ClientSide.Services;
using Orchard.Mvc.Filters;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using Orchard.Projections.Models;
using Orchard.Mvc.Html;

namespace MainBit.Projections.ClientSide.Filters
{
    public class ClientSideProjectionFilter : FilterProvider, IActionFilter {

        private readonly ICurrentContentAccessor _currentContentAccessor;
        private readonly IUrlService _urlService;
        private readonly IPresetService _presetService;
        private readonly IClientSideProjectionTokensService _tokenService;
        private readonly UrlHelper _urlHelper;
        private readonly IContentManager _contentManager;

        public ClientSideProjectionFilter(ICurrentContentAccessor currentContentAccessor,
            IUrlService presetQueryStringService,
            IClientSideProjectionTokensService tokenService,
            IPresetService presetService,
            UrlHelper urlHelper,
            IContentManager contentManager
            )
        {
            _currentContentAccessor = currentContentAccessor;
            _urlService = presetQueryStringService;
            _tokenService = tokenService;
            _presetService = presetService;
            _urlHelper = urlHelper;
            _contentManager = contentManager;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Items["ClientSideProjectionRedirectUrl"] != null)
            {
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Items["ClientSideProjectionRedirectUrl"].ToString());
            }
        }
    }
}