using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using MainBit.Projections.ClientSide.Providers.SortCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models.SortCriteria
{
    public class ClientSideSortCriterion
    {
        public ClientSideSortCriterion()
        {
            Options = new List<ClientSideSortCriterionOption>();
        }
        public int? ApplyingPosition { get; set; } // Sort position selected by user. Example first sorted by title, then by price.
        public List<ClientSideSortCriterionOption> Options { get; set; }
        public ClientSideSortCriterionOption ApplyingOption { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }

        public IClientSideSortCriterionEditor Editor { get; set; }
    }

    public class ClientSideSortCriterionOption
    {
        public Enum Direction { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }
}