using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Models.Layouts;
using MainBit.Projections.ClientSide.Models.SortCriteria;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models
{
    public class ClientSideProjectionPart : ContentPart<ClientSideProjectionPartRecord>
    {
        public ClientSideProjectionPart()
        {
            Infoset = new Infoset();
        }

        public Infoset Infoset { get; set; }

        
        private readonly LazyField<List<ClientSideFilter>> _filtersField = new LazyField<List<ClientSideFilter>>();
        public LazyField<List<ClientSideFilter>> FiltersField { get { return _filtersField; } }
        public List<ClientSideFilter> Filters
        {
            get { return _filtersField.Value; }
            set { _filtersField.Value = value; }
        }


        private readonly LazyField<List<ClientSideSortCriterion>> _sortCriteriaField = new LazyField<List<ClientSideSortCriterion>>();
        public LazyField<List<ClientSideSortCriterion>> SortCriteriaField { get { return _sortCriteriaField; } }
        public List<ClientSideSortCriterion> SortCriteria
        {
            get { return _sortCriteriaField.Value; }
            set { _sortCriteriaField.Value = value; }
        }


        private readonly LazyField<List<ClientSideLayout>> _layoutField = new LazyField<List<ClientSideLayout>>();
        public LazyField<List<ClientSideLayout>> LayoutsField { get { return _layoutField; } }
        public List<ClientSideLayout> Layouts
        {
            get { return _layoutField.Value; }
            set { _layoutField.Value = value; }
        }
    }
}