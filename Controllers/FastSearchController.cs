using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainBit.Projections.ClientSide.Controllers
{
    public class FastSearchController : Controller
    {
        private readonly IPrepareService _prepareService;
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public FastSearchController(
            IPrepareService prepareService,
            IContentManager contentManager,
            IProjectionManager projectionManager
            )
        {
            _prepareService = prepareService;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
        }

        public JsonResult Index(int id)
        {
            var part = _contentManager.Get<ClientSideProjectionPart>(id);
            if (part == null) { Json(new { error = "content item not found" }); }

            var prepareContext = new PrepareContext
            {
                Part = part,
                Request = Request
            };
            _prepareService.Prepare(prepareContext);
            var count = _projectionManager.GetCount(part.Record.QueryPartRecord.Id);

            var data = new { count = count, url = prepareContext.ResultUrl };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}