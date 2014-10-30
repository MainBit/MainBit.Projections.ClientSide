using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.ComponentModel;
using System.Text;
using System.Web.Script.Serialization;

namespace MainBit.Projections.ClientSide.Services
{
    public class ClientSideFilterFormHelper
    {
        public const string Switcher = "ClientSideSwitcher";
        public const string Name = "ClientSideName";
        public const string Type = "CliendSideType";

        public static bool IsForClientSide(IDictionary<string, string> filterState)
        {
            return filterState.ContainsKey(Switcher) ? Convert.ToBoolean(filterState[Switcher]) : false;
        }

        public static string GetForClientSideType(IDictionary<string, string> filterState)
        {
            return filterState.ContainsKey(Type) ? filterState[Type] : null;
        }

        public static string GetName(IDictionary<string, string> filterState)
        {
            return filterState.ContainsKey(Name) ? filterState[Name] : null;
        }

        public static dynamic CreateRelatedElementShape(dynamic shapeFactory, object nameValues)
        {
            var relatedElShape = shapeFactory.FieldSet(Classes: new[] { "client-side-related-elements" });
            relatedElShape.Attributes.Add("style", "display:none");

            var jsonElements = new JavaScriptSerializer().Serialize(nameValues);
            relatedElShape.Attributes.Add("data-elements", jsonElements);

            return relatedElShape;
        }
    }
}