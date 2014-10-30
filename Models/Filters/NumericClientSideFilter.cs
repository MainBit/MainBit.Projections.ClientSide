using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Models.Filters
{
    public class NumericClientSideFilter : ClientSideFilter
    {
        public decimal? Min
        {
            get { return Storage.Get<decimal?>("min"); }
            set { Storage.Set<decimal?>("min", value); }
        }
        public decimal? Max
        {
            get { return Storage.Get<decimal?>("max"); }
            set { Storage.Set<decimal?>("max", value); }
        }
        public decimal? Scale
        {
            get { return Storage.Get<decimal?>("scale"); }
            set { Storage.Set<decimal?>("scale", value); }
        }

        public decimal? From;
        public decimal? To;
    }
}