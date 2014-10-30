using MainBit.Projections.ClientSide.FilterEditors.Forms;
using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Services;
using Orchard.Forms.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace MainBit.Projections.ClientSide.ClientSideEditors.Filters
{
    public class BooleanClientSideFilterEditor : ClientSideFilterEditor<BooleanClientSideFilter>
    {
        public override bool CanHandle(string filterEditorFormName, string clientSideType = null)
        {
            return filterEditorFormName == BooleanVariableFilterForm.FormName;
        }

        public override void OnFormBuilt(BuildingContext context, dynamic shapeHelper)
        {
            context.Shape._ClientSideRelatedField(ClientSideFilterFormHelper.CreateRelatedElementShape(shapeHelper, new { Value = "ClientSideFilters.Value:{0}" }));
        }

        protected override NameValueCollection ToQueryString(BooleanClientSideFilter filter)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (filter.Value.HasValue)
            {
                queryString[filter.Name] = filter.Value.Value ? "1" : "0";
            }
            return queryString;
        }
        protected override void FromQueryString(BooleanClientSideFilter filter, NameValueCollection queryString)
        {
            if (queryString[filter.Name] == "1")
            {
                filter.Value = true;
            }
            else if(queryString[filter.Name] == "0")
            {
                filter.Value = false;
            }
            else {
                filter.Value = null;
            }
        }
        protected override void BuildTokens(BooleanClientSideFilter filter, IClientSideProjectionTokensService tokenService)
        {
            if (filter.Value.HasValue)
            {
                tokenService.SetValue(filter.Name, filter.Value.Value ? "1" : "0");
            }
            else
            {
                tokenService.RemoveValue(filter.Name);
            }
        }

        protected override dynamic Display(BooleanClientSideFilter filter, dynamic shapeHelper)
        {
            return shapeHelper.Create("ClientSideFilters_Boolean");
        }
        protected override string ToJsonString(BooleanClientSideFilter filter)
        {
            var sb = new StringBuilder();
            sb.Append(filter.Name);
            sb.Append(":{type:\"simple\",value:\"");
            sb.Append(filter.Value.ToJsString());
            sb.Append("\"}");

            return sb.ToString();
        }
    }

    public static class BooleanExtensions
    {
        public static string ToJsString(this bool? value)
        {
            if (!value.HasValue) { return string.Empty; }
            return value.Value.ToJsString();
        }

        public static string ToJsString(this bool value)
        {
            return value ? "1" : "0";
        }
    }
}