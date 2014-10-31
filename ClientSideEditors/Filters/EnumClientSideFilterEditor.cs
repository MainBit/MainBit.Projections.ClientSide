using MainBit.Projections.ClientSide.FilterEditors.Forms;
using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Services;
using Orchard.Environment;
using Orchard.Forms.Services;
using Orchard.UI.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Filters
{
    public class EnumClientSideFilterEditor : ClientSideFilterEditor<EnumClientSideFilter>
    {
        private readonly Work<IResourceManager> _resourceManager;
        public const string QueryStringSeparator = "-";

        public EnumClientSideFilterEditor(Work<IResourceManager> resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public override bool CanHandle(string filterEditorFormName, string clientSideType = null)
        {
            return filterEditorFormName == StringVariableFilterForm.FormName;
        }

        public override void OnFormBuilt(BuildingContext context, dynamic shapeHelper)
        {
            context.Shape._ClientSideRelatedField(ClientSideFilterFormHelper.CreateRelatedElementShape(shapeHelper, new {
                Value = "ClientSideFilters.Value:{0}"
            }));

            _resourceManager.Value.Require("script", "jQuery");
            _resourceManager.Value.Include("script",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-enum.js",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-enum.js");
        }

        protected override NameValueCollection ToQueryString(EnumClientSideFilter filter)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            var selectedItems = filter.Items.Where(i => i.Selected);

            if (selectedItems.Any())
            {
                queryString[filter.Name] = string.Join(QueryStringSeparator, selectedItems.Select(p => p.Id));
            }
            
            return queryString;
        }
        protected override void FromQueryString(EnumClientSideFilter filter, NameValueCollection queryString)
        {
            var values = queryString.GetValues(filter.Name);
            if (values != null)
            {
                var clearValues = values.SelectMany(v => v.Split(new string[] { QueryStringSeparator }, StringSplitOptions.RemoveEmptyEntries));
                foreach (var item in filter.Items)
                {
                    item.Selected = clearValues.Contains(item.Id);
                }
            }
            else
            {
                foreach (var item in filter.Items)
                {
                    item.Selected = false;
                }
            }
        }
        protected override void BuildTokens(EnumClientSideFilter filter, IClientSideProjectionTokensService tokenService)
        {
            var jsonString = string.Join(StringVariableFilterForm.Separator, filter.Items.Where(i => i.Selected).Select(i => i.DisplayValue));
            if (!string.IsNullOrEmpty(jsonString))
            {
                tokenService.SetValue(filter.Name, jsonString);
            }
            else
            {
                tokenService.RemoveValue(filter.Name);
            }
        }

        protected override dynamic Display(EnumClientSideFilter filter, dynamic shapeHelper)
        {
            return shapeHelper.Create("ClientSideFilters_Enum");
        }
        protected override string ToJsonString(EnumClientSideFilter filter)
        {
            var sb = new StringBuilder();
            sb.Append(filter.Name);
            sb.Append(":{type:\"simple\",value:\"");
            sb.Append(string.Join(EnumClientSideFilterEditor.QueryStringSeparator, filter.Items.Where(i => i.Selected).Select(i => i.Id)));
            sb.Append("\"}");
            return sb.ToString();
        }


        protected override void UpdateAvaliableValues(EnumClientSideFilter filter, IEnumerable values)
        {
            if (values == null) { return; }

            foreach (var value in values)
            {
                var sValue = value.ToString();
                if (!filter.Items.Any(i => string.Equals(i.DisplayValue, sValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var items = filter.Items.ToList();
                    items.Add(new EnumClientSideFilterItemEntry
                    {
                        DisplayValue = sValue,
                        Id = (items.Count + 1).ToString(),
                        Selected = false
                    });
                    filter.Items = items.ToArray();
                }
            }
        }
        protected override void ClearAvaliableValues(EnumClientSideFilter filter)
        {
            filter.Items = null;
        }

        public override IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Filter.FilterDescriptor descriptor)
        {
            var dictionary = new Dictionary<string, string>();
            var name = QueryFromHelper.GetName(descriptor.Category, descriptor.Type);

            dictionary.Add("Description", QueryFromHelper.GetDisplayName(descriptor.Name.ToString()));
            dictionary.Add("Operator", "ContainsAny");
            dictionary.Add("Value", "{" + string.Format(ClientSideProjectionTokensService.TokenName, name) + "}");
            dictionary.Add("OperatorUndefined", "Any");
            dictionary.Add("ClientSideSwitcher", "true");
            dictionary.Add("ClientSideName", name);

            return dictionary;
        }
    }

}