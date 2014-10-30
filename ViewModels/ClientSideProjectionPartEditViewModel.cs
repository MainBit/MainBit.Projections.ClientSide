using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.ViewModels
{
    public class ClientSideProjectionPartEditViewModel
    {
        public string PresetQueryString { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Items { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Skip { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int MaxItems { get; set; }

        public string PagerSuffix { get; set; }

        public bool DisplayPager { get; set; }

        [Required(ErrorMessage = "You must select a Query")]
        public int QueryRecordId { get; set; }

        public IEnumerable<QueryRecordEntry> QueryRecordEntries { get; set; }
    }

    public class QueryRecordEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}