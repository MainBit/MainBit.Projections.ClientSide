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
    public class PrepareHalder : ContentHandler
    {
        private readonly IPrepareService _prepareService;
        private readonly IWorkContextAccessor _wca;

        public PrepareHalder(IWorkContextAccessor wca,
            IPrepareService prepareService
            )
        {
            _prepareService = prepareService;
            _wca = wca;

            OnGetDisplayShape<ProjectionPart>(OnGetDisplayShape);
            OnGetDisplayShape<ClientSideProjectionPart>(OnGetDisplayShape);
        }

        private void OnGetDisplayShape(BuildDisplayContext context, ProjectionPart part)
        {
            var otherPart = part.As<ClientSideProjectionPart>();
            CreateTokens_Redirect_NeedMoveTo(otherPart, part, context.DisplayType);
        }

        private void OnGetDisplayShape(BuildDisplayContext context, ClientSideProjectionPart part)
        {
            var otherPart = part.As<ProjectionPart>();
            CreateTokens_Redirect_NeedMoveTo(part, otherPart, context.DisplayType);
        }

        private bool IsExecuted = false;
        private void CreateTokens_Redirect_NeedMoveTo(ClientSideProjectionPart part, ProjectionPart projectionPart, string displayType)
        {
            if (displayType != "Detail" || part == null || projectionPart == null || IsExecuted) { return; }
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