using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.Projections.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models
{
    public class ClientSideProjectionPartRecord : ContentPartRecord
    {
        public virtual string PresetQueryString { get; set; }
        
        [StringLengthMax]
        public virtual string Data { get; set; }

        /// <summary>
        /// Maximum number of items to retrieve from db
        /// </summary>
        public virtual int Items { get; set; }

        /// <summary>
        /// Number of items to skip
        /// </summary>
        public virtual int Skip { get; set; }

        /// <summary>
        /// The maximum number of items which can be requested at once. 
        /// </summary>
        public virtual int MaxItems { get; set; }

        /// <summary>
        /// Suffix to use when multiple pagers are available on the same page
        /// </summary>
        [StringLength(255)]
        public virtual string PagerSuffix { get; set; }

        /// <summary>
        /// True to render a pager
        /// </summary>
        public virtual bool DisplayPager { get; set; }

        /// <summary>
        /// The query to execute
        /// </summary>
        [Aggregate]
        public virtual QueryPartRecord QueryPartRecord { get; set; }
    }
}