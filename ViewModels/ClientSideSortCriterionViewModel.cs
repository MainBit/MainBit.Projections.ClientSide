using MainBit.Projections.ClientSide.Providers.SortCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ViewModels
{
    public class ClientSideSortCriterionViewModel
    {
        public ClientSideSortCriterionViewModel()
        {
            Entries = new List<ClientSideSortCriterionEntry>();
        }

        public int? ApplyingPosition { get; set; } // Sort position selected by user. Example first sorted by title, then by price.
        public List<ClientSideSortCriterionEntry> Entries { get; set; }
        public ClientSideSortCriterionEntry ApplyingEntry { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class ClientSideSortCriterionEntry
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public Enum Direction { get; set; }
        public string Url { get; set; } // url with sort only by this option
        public string AdditionalUrl { get; set; } // url with sort already applyed options and then by this option
    }
}