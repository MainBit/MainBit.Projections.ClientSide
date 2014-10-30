using System;
using System.Collections.Generic;
using Orchard.Localization;
using Orchard.ContentManagement;
using System.Collections;

namespace MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers {
    public class DescribeFilterFor {
        private readonly string _category;

        public DescribeFilterFor(string category)
        {
            Types = new List<FilterDescriptor>();
            _category = category;
        }

        public List<FilterDescriptor> Types { get; private set; }

        public DescribeFilterFor Element(string type, Func<IContent, IEnumerable> retrieveValues)
        {
            Types.Add(new FilterDescriptor { Type = type, Category = _category, RetrieveValues = retrieveValues });
            return this;
        }
    }
}