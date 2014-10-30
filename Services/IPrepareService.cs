using MainBit.Projections.ClientSide.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Projections.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Html;
using MainBit.Projections.ClientSide.Providers.SortCriteria;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IPrepareService : IDependency
    {
        void Prepare(PrepareContext prepareContext);
    }

    public class PrepareService : IPrepareService
    {
        private readonly IUrlService _urlService;
        private readonly IPresetService _presetService;
        private readonly IClientSideProjectionTokensService _tokenService;
        private readonly UrlHelper _urlHelper;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _wca;
        private readonly IClientSideSortService _clientSideSortService;
        private readonly IClientSideFilterService _clientSideFilterService;
        private readonly IClientSideLayoutService _clientSideLayoutService;

        public PrepareService(IUrlService presetQueryStringService,
            IClientSideProjectionTokensService tokenService,
            IPresetService presetService,
            UrlHelper urlHelper,
            IContentManager contentManager,
            IWorkContextAccessor wca,
            IClientSideSortService clientSideSortService,
            IClientSideFilterService clientSideFilterService,
            IClientSideLayoutService clientSideLayoutService
            )
        {
            _urlService = presetQueryStringService;
            _tokenService = tokenService;
            _presetService = presetService;
            _urlHelper = urlHelper;
            _contentManager = contentManager;
            _wca = wca;
            _clientSideSortService = clientSideSortService;
            _clientSideFilterService = clientSideFilterService;
            _clientSideLayoutService = clientSideLayoutService;
         }

        public void Prepare(PrepareContext context)
        {
            var presetQueryString = _urlService.MakeCleanQueryString(context.Part.Record.PresetQueryString);
            var fullQueryString = _urlService.MergeQueryString(context.Request.QueryString, presetQueryString);

            context.Part.Filters.ForEach(filter => filter.Editor.FromQueryString(filter, fullQueryString));
            context.Part.SortCriteria.ForEach(sort => _clientSideSortService.FromQueryString(sort, fullQueryString));
            context.Part.Layouts.ForEach(layout => _clientSideLayoutService.FromQueryString(layout, fullQueryString));

            var filterQueryString = _clientSideFilterService.GetFiltersQueryStirng(context.Part.Filters);
            var sortCriteriaQueryString = _clientSideSortService.GetSortCriteraQueryString(context.Part.SortCriteria);
            var layoutQueryString = _clientSideLayoutService.GetLayoutQueryString(context.Part.Layouts);

            var suitableContentId = _presetService.GetSuitableContentId(context.Part.Record.QueryPartRecord.Id, filterQueryString.ToString());
            if (suitableContentId == 0)
            {
                // I dont not know wthat should do
                // This means there is not suitable client side projection part and content item which must be showed
                // All of existing content with client side prijection part have more pre-installed filter options than user picks
                // In general need create one page with empty pre-installed filter options
            }

            var knowingKeys = new List<string>();
            context.Part.Filters.ForEach(f => knowingKeys.AddRange(f.Editor.GetNames(f)));
            knowingKeys.Add(ClientSideSortService.QueryStringParamName);
            knowingKeys.Add(ClientSideLayoutService.QueryStringParamName);

            if (suitableContentId > 0 && context.Part.Id != suitableContentId)
            {
                var suitablePart = _contentManager.Get<ClientSideProjectionPart>(suitableContentId);
                var suitablePresetQueryString = _urlService.MakeCleanQueryString(suitablePart.Record.PresetQueryString);
                var suitableContentUrl = _urlHelper.ItemDisplayUrl(suitablePart);
                var suitableUrl = _urlService.BuildUrl(suitableContentUrl, suitablePresetQueryString, filterQueryString, sortCriteriaQueryString, layoutQueryString, fullQueryString, knowingKeys);
                context.ResultUrl = suitableUrl;
            }
            else
            {
                var currentContentUrl = _urlHelper.ItemDisplayUrl(context.Part);
                var currentRightUrl = _urlService.BuildUrl(currentContentUrl, presetQueryString, filterQueryString, sortCriteriaQueryString, layoutQueryString, fullQueryString, knowingKeys);
                context.ResultUrl = currentRightUrl;
            }

            // if user has not select any sort criterion that query will not be ordered at all
            // can change behavior of IProjectionManager that it result will be depended on the order of my custom position
            // or may add default sort criterion to last position and set default ordering to it different from none
            // in general need find a way to apply sort criteria in my custom order
            var chanched = false;
            if (context.Part.SortCriteria.Any() && context.Part.SortCriteria.All(s => !s.ApplyingPosition.HasValue))
            {
                chanched = true;
                context.Part.SortCriteria[0].ApplyingOption = context.Part.SortCriteria[0].Options.FirstOrDefault();
                // context.Part.SortCriteria[0].ApplyingPosition = 0;
            }
            
            context.Part.Filters.ForEach(f => f.Editor.BuildTokens(f, _tokenService));
            context.Part.SortCriteria.ForEach(f => f.Editor.BuildTokens(f, _tokenService));

            if (chanched)
            {
                context.Part.SortCriteria[0].ApplyingOption = context.Part.SortCriteria[0].Options.FirstOrDefault();
                // context.Part.SortCriteria[0].ApplyingPosition = null;
            }
        }
    }

    public class PrepareContext
    {
        public ClientSideProjectionPart Part { get; set; }
        public HttpRequestBase Request { get; set; }
        public string ResultUrl { get; set; }

    }

}