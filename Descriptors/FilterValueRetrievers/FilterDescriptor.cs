using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;
using System.Collections;

namespace MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers
{
    public class FilterDescriptor
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public Func<IContent, IEnumerable> RetrieveValues { get; set; }
    }
}