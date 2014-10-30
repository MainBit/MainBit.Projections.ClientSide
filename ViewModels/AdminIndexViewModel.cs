using Orchard.Projections.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ViewModels
{
    public class AdminIndexViewModel
    {
        public IList<QueryEntry> Queries { get; set; }

        public AdminIndexOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }

    public class QueryEntry
    {
        public QueryPartRecord Query { get; set; }
        public bool IsChecked { get; set; }
        public int QueryId { get; set; }
        public string Name { get; set; }
    }

    public class AdminIndexOptions
    {
        public string Search { get; set; }
        public QueriesOrder Order { get; set; }
        public QueriesFilter Filter { get; set; }
        public QueriesBulkAction BulkAction { get; set; }
    }

    public enum QueriesOrder
    {
        Name,
        Creation
    }

    public enum QueriesFilter
    {
        All
    }

    public enum QueriesBulkAction
    {
        None,
        UpdateAvaliableValues
    }
}