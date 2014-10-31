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
    public class NumericClientSideFilterEditor : ClientSideFilterEditor<NumericClientSideFilter>
    {
        private readonly Work<IResourceManager> _resourceManager;
        public const string NameFrom = "from";
        public const string NameTo = "to";

        public NumericClientSideFilterEditor(Work<IResourceManager> resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public override bool CanHandle(string filterEditorFormName, string clientSideType = null)
        {
            return filterEditorFormName == NumericVariableFilterForm.FormName;
        }

        public override void OnFormBuilt(BuildingContext context, dynamic shapeHelper)
        {
            context.Shape._ClientSideRelatedField(ClientSideFilterFormHelper.CreateRelatedElementShape(shapeHelper, new {
                Min = "ClientSideFilters.Value:{0}" + NameFrom,
                Max = "ClientSideFilters.Value:{0}" + NameTo,
            }));

            _resourceManager.Value.Require("script", "jQuery");
            _resourceManager.Value.Include("script",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-numeric.js",
                "~/Modules/MainBit.Projections.ClientSide/Scripts/mainbit-projection-clientside-editor-numeric.js");
        }

        protected override IEnumerable<string> GetNames(NumericClientSideFilter filter)
        {
            return new List<string> { filter.GetNameFrom(), filter.GetNameTo() };
        }

        protected override NameValueCollection ToQueryString(NumericClientSideFilter filter)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (filter.From.HasValue)
            {
                queryString[filter.GetNameFrom()] = filter.From.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (filter.To.HasValue)
            {
                queryString[filter.GetNameTo()] = filter.To.Value.ToString(CultureInfo.InvariantCulture);
            }
            
            return queryString;
        }
        protected override void FromQueryString(NumericClientSideFilter filter, NameValueCollection queryString)
        {
            decimal outer;

            if (Decimal.TryParse(queryString[filter.GetNameFrom()], NumberStyles.None, CultureInfo.InvariantCulture, out outer))
            {
                filter.From = outer;
            }
            else {
                filter.From = null;
            }

            if (Decimal.TryParse(queryString[filter.GetNameTo()], NumberStyles.None, CultureInfo.InvariantCulture, out outer))
            {
                filter.To = outer;
            }
            else {
                filter.To = null;
            }
        }
        protected override void BuildTokens(NumericClientSideFilter filter, IClientSideProjectionTokensService tokenService)
        {
            if (filter.From.HasValue)
            {
                tokenService.SetValue(filter.GetNameFrom(), filter.From.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                tokenService.RemoveValue(filter.GetNameFrom());
            }

            if (filter.To.HasValue)
            {
                tokenService.SetValue(filter.GetNameTo(), filter.To.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                tokenService.RemoveValue(filter.GetNameTo());
            }
        }

        protected override dynamic Display(NumericClientSideFilter filter, dynamic shapeHelper)
        {
            return shapeHelper.Create("ClientSideFilters_Numeric");
        }
        protected override string ToJsonString(NumericClientSideFilter filter)
        {
            var sb = new StringBuilder();
            sb.Append(filter.GetNameFrom());
            sb.Append(":{type:\"numeric\",value:\"");
            sb.Append(filter.From.ToJsString());
            sb.Append("\"}");

            sb.Append(",");

            sb.Append(filter.GetNameTo());
            sb.Append(":{type:\"numeric\",value:\"");
            sb.Append(filter.To.ToJsString());
            sb.Append("\"}");

            return sb.ToString();
        }



        protected override void UpdateAvaliableValues(NumericClientSideFilter filter, IEnumerable values)
        {
            if (values == null) { return; }

            foreach (var value in values)
            {
                var t = value.GetType();

                // the T is nullable, convert using underlying type
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = Nullable.GetUnderlyingType(t);
                }

                var typeCode = Type.GetTypeCode(t);

                switch (typeCode)
                {
                    case TypeCode.Byte: // Orchard.Indexing.Handlers.InfosetFieldIndexingHandler different from Orchard.Projections.FilterEditors.NumericFilterEditor
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        var dValue = Convert.ToDecimal(value);
                        if (!filter.Max.HasValue || filter.Max.Value < dValue)
                        {
                            filter.Max = dValue;
                        }
                        if (!filter.Min.HasValue || filter.Min.Value > dValue)
                        {
                            filter.Min = dValue;
                        }
                        var scale = BitConverter.GetBytes(decimal.GetBits(dValue)[3])[2];
                        if (!filter.Scale.HasValue || filter.Scale.Value < dValue)
                        {
                            filter.Scale = scale;
                        }
                        break;
                }
            }
        }
        protected override void ClearAvaliableValues(NumericClientSideFilter filter)
        {
            filter.Max = null;
            filter.Min = null;
            filter.Scale = null;
        }

        public override IDictionary<string, string> BuildDefaultState(Orchard.Projections.Descriptors.Filter.FilterDescriptor descriptor)
        {
            var dictionary = new Dictionary<string, string>();
            var name = QueryFromHelper.GetName(descriptor.Category, descriptor.Type);

            dictionary.Add("Description", QueryFromHelper.GetDisplayName(descriptor.Name.ToString()));
            dictionary.Add("OperatorMin", "GreaterThanEquals");
            dictionary.Add("Min", string.Format(ClientSideProjectionTokensService.TokenName, name + NameFrom));
            dictionary.Add("OperatorMax", "LessThanEquals");
            dictionary.Add("Max", string.Format(ClientSideProjectionTokensService.TokenName, name + NameTo));
            dictionary.Add("ClientSideSwitcher", "true");
            dictionary.Add("ClientSideName", name);

            return dictionary;
        }
    }

    public static class NumericClientSideFilterExtensions
    {
        public static string GetNameFrom(this NumericClientSideFilter filter)
        {
            return filter.Name + NumericClientSideFilterEditor.NameFrom;
        }

        public static string GetNameTo(this NumericClientSideFilter filter)
        {
            return filter.Name + NumericClientSideFilterEditor.NameTo;
        }
    }

    public static class DecimalExtensions
    {
        public static string ToJsString(this decimal? value)
        {
            if (!value.HasValue) { return string.Empty; }
            return value.Value.ToJsString();
        }

        public static string ToJsString(this decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}