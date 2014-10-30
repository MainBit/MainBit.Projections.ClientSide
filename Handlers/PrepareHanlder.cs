using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment;
using Orchard.Projections.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Html;
using Orchard;

namespace MainBit.Projections.ClientSide.Handlers
{
    public class PrepareHanlder : ContentHandler
    {
        private readonly IPrepareService _prepareService;
        private readonly IWorkContextAccessor _wca;

        public PrepareHanlder(IWorkContextAccessor wca,
            IPrepareService prepareService
            )
        {
            _prepareService = prepareService;
            _wca = wca;

            OnGetDisplayShape<ClientSideProjectionPart>(NotGoodPlaceForThisFuntion);
        }


        private bool IsExecuted = false;
        private void NotGoodPlaceForThisFuntion(BuildDisplayContext context, ClientSideProjectionPart part)
        {
            if (context.DisplayType != "Detail" || part == null || IsExecuted) { return; }

            IsExecuted = true;
            var httpContext = _wca.GetContext().HttpContext;

            var prepareContext = new PrepareContext
            {
                Part = part,
                Request = httpContext.Request
            };

            _prepareService.Prepare(prepareContext);
            if (prepareContext.ResultUrl != httpContext.Request.RawUrl)
            {
                httpContext.Items["ClientSideProjectionRedirectUrl"] = prepareContext.ResultUrl;
            }
        }
    }
}