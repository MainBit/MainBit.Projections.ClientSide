using MainBit.Projections.ClientSide.Models.Layouts;
using Orchard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IClientSideLayoutService : IDependency
    {
        void FromQueryString(ClientSideLayout layout, NameValueCollection queryString);
        NameValueCollection GetLayoutQueryString(List<ClientSideLayout> layouts);
        NameValueCollection GetLayoutQueryString(ClientSideLayout layout, bool forceInclude = false);
    }

    public class ClientSideLayoutService : IClientSideLayoutService
    {
        public const string QueryStringParamName = "layout";

        public void FromQueryString(ClientSideLayout layout, NameValueCollection queryString)
        {
            layout.Applying = queryString[QueryStringParamName] == layout.Name;
        }

        public NameValueCollection GetLayoutQueryString(List<ClientSideLayout> layouts)
        {
            var layoutQueryString = HttpUtility.ParseQueryString(string.Empty);

            var layout = layouts.FirstOrDefault(l => l.Applying);
            if (layout != null)
            {
                layoutQueryString[QueryStringParamName] = layout.Name;
            }

            return layoutQueryString;
        }

        public NameValueCollection GetLayoutQueryString(ClientSideLayout layout, bool forceInclude = false)
        {
            var layoutQueryString = HttpUtility.ParseQueryString(string.Empty);
            layoutQueryString[QueryStringParamName] = layout.Name;
            return layoutQueryString;
        }
    }
}