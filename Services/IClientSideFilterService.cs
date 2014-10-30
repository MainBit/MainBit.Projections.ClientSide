using MainBit.Projections.ClientSide.Models.Filters;
using Orchard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IClientSideFilterService : IDependency
    {
        NameValueCollection GetFiltersQueryStirng(List<ClientSideFilter> filters);
    }

    public class ClientSideFilterService : IClientSideFilterService
    {
        public NameValueCollection GetFiltersQueryStirng(List<ClientSideFilter> filters)
        {
            var filterQueryString = HttpUtility.ParseQueryString(string.Empty);

            foreach (var filter in filters)
            {
                filterQueryString.Add(filter.Editor.ToQueryString(filter));
            }

            return filterQueryString;
        }
    }
}