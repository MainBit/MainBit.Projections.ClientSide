using MainBit.Projections.ClientSide.Models.SortCriteria;
using MainBit.Projections.ClientSide.Providers.SortCriteria;
using MainBit.Projections.ClientSide.Services;
using MainBit.Projections.ClientSide.ViewModels;
using Orchard;
using Orchard.Environment;
using Orchard.Forms.Services;
using Orchard.Projections.Providers.SortCriteria;
using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria
{
    public class DefaultClientSideSortCriterionEditor : ClientSideSortCriterionEditor<ClientSideSortCriterion>
    {
        private readonly Work<IResourceManager> _resourceManager;
        private readonly IClientSideProjectionTokensService _clientSideProjectionTokensService;

        public DefaultClientSideSortCriterionEditor(Work<IResourceManager> resourceManager,
            IClientSideProjectionTokensService clientSideProjectionTokensService)
        {
            _resourceManager = resourceManager;
            _clientSideProjectionTokensService = clientSideProjectionTokensService;
        }

        public override bool CanHandle(string sortCriterionFormName)
        {
            return sortCriterionFormName == VariableSortCriterionFormProvider.FormName;
        }

        public override void OnFormBuilt(BuildingContext context, dynamic shapeHelper)
        {
            context.Shape._ClientSideRelatedField(ClientSideFilterFormHelper.CreateRelatedElementShape(shapeHelper, new { Sort = "ClientSideFilters.Value:sort-{0}" }));

            _resourceManager.Value.Require("script", "jQuery");
            _resourceManager.Value.Include("script",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-sort-default.js",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-sort-default.js");
        }

        protected override void OnCreated(ClientSideSortCriterion sortCriterion, IDictionary<string, string> state)
        {
            sortCriterion.Options.Add(new ClientSideSortCriterionOption {
                Value = GetValue(sortCriterion.Name, SortDirection.Ascending),
                DisplayName = sortCriterion.DisplayName,
                Direction = SortDirection.Ascending
            });
            sortCriterion.Options.Add(new ClientSideSortCriterionOption {
                Value = GetValue(sortCriterion.Name, SortDirection.Descending),
                DisplayName = sortCriterion.DisplayName,
                Direction = SortDirection.Descending
            });

            var sort = (SortDirection)Enum.Parse(typeof(SortDirection), Convert.ToString(state["SortUndefined"]));
            switch (sort)
            {
                case SortDirection.None:
                case SortDirection.Ascending:
                    break;
                case SortDirection.Descending:
                    sortCriterion.Options.Reverse();
                    break;
                default:
                    break;
            }
        }

        private string GetValue(string name, SortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case SortDirection.None:
                    return string.Empty;
                case SortDirection.Ascending:
                    return name + "-asc";
                case SortDirection.Descending:
                    return name + "-desc";
                default:
                    return string.Empty;
            }
        }

        private string GetTokenValue(SortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case SortDirection.None:
                    return string.Empty;
                case SortDirection.Ascending:
                    return "Ascending";
                case SortDirection.Descending:
                    return "Descending";
                default:
                    return string.Empty;
            }
        }


        protected override dynamic Display(ClientSideSortCriterion sortCriterion, dynamic shapeHelper)
        {
            return shapeHelper.Create("ClientSideSortCriterion_Default");
        }


        protected override void BuildTokens(ClientSideSortCriterion sortCriterion, IClientSideProjectionTokensService tokenService)
        {
            if (sortCriterion.ApplyingOption != null)
            {
                tokenService.SetValue("sort-" + sortCriterion.Name, GetTokenValue((SortDirection)sortCriterion.ApplyingOption.Direction));
            }
            else
            {
                tokenService.RemoveValue("sort-" + sortCriterion.Name);
            }
        }

        public override IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.SortCriterion.SortCriterionDescriptor descriptor)
        {
            var dictionary = new Dictionary<string, string>();
            var name = QueryFromHelper.GetName(descriptor.Category, descriptor.Type);

            dictionary.Add("Description", QueryFromHelper.GetDisplayName(descriptor.Name.ToString()));
            dictionary.Add("Sort", "{" + string.Format(ClientSideProjectionTokensService.TokenName, "sort-" + name) + "}");
            dictionary.Add("SortUndefined", "None");
            dictionary.Add("ClientSideSwitcher", "true");
            dictionary.Add("ClientSideName", name);

            return dictionary;
        }

    }
}