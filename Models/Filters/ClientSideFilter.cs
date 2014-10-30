using MainBit.Projections.ClientSide.ClientSideEditors.Filters;
using MainBit.Projections.ClientSide.Descriptors.FilterValueRetrievers;
using MainBit.Projections.ClientSide.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models.Filters
{
    public abstract class ClientSideFilter
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IStorage Storage { get; set; }


        public string Category { get; set; }
        public string Type { get; set; }


        public IClientSideFilterEditor Editor { get; set; }
    }
}