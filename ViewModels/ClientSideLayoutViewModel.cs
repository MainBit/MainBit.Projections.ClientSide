using MainBit.Projections.ClientSide.Providers.SortCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ViewModels
{
    public class ClientSideLayoutViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }

        public bool Applying { get; set; }
    }
}