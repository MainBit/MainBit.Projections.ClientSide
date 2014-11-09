using MainBit.Projections.ClientSide.Models;
using Orchard.Projections.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace MainBit.Projections.ClientSide.Storage.Providers
{
    public class InfosetStorageProvider : IStorageProvider
    {
        public int Priority
        {
            get { return -1; }
        }
        public string ProviderName
        {
            get { return "Infoset"; }
        }

        public bool CanHandle(string filterCategory, string filterType)
        {
            return true;
        }

        public IStorage BindStorage(ClientSideProjectionPart part, string filterCategory, string filterType)
        {
            var filterName = XmlConvert.EncodeLocalName(filterCategory + "__" + filterType);

            return new SimpleStorage(
                (name, valueType) => Get(part.Infoset.Element, filterName, name),
                (name, valueType, value) => Set(part.Infoset.Element, filterName, name, value));
        }

        private static string Get(XElement element, string filterName, string valueName)
        {
            var filterElement = element.Element(filterName);
            if (filterElement == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(valueName))
            {
                return filterElement.Value;
            }

            var valueAttribute = filterElement.Attribute(XmlConvert.EncodeLocalName(valueName));
            if (valueAttribute == null)
            {
                return null;
            }
            return valueAttribute.Value;
        }

        private void Set(XElement element, string filterName, string valueName, string value)
        {
            var filterElement = element.Element(filterName);
            if (filterElement == null)
            {
                filterElement = new XElement(filterName);
                element.Add(filterElement);
            }

            if (string.IsNullOrEmpty(valueName))
            {
                filterElement.Value = value;
            }
            else
            {
                filterElement.SetAttributeValue(XmlConvert.EncodeLocalName(valueName), value);
            }
        }
    }
}