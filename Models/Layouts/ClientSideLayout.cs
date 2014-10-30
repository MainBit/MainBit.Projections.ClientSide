using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using MainBit.Projections.ClientSide.Providers.SortCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models.Layouts
{
    public class ClientSideLayout
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }
        public int LayoutRecordId { get; set; }

        public bool Applying { get; set; }
    }
}