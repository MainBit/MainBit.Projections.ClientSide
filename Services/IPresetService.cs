using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IPresetService : IDependency
    {
        int GetSuitableContentId(int queryId, string filterQueryString);
    }

    public class PresetService : IPresetService
    {
        private readonly ISessionLocator _sessionLocator;
        public PresetService(ISessionLocator sessionLocator)
        {
            _sessionLocator = sessionLocator;
        }
        public int GetSuitableContentId(int queryId, string filterQueryString)
        {
            const string searchQuery = @"
                    SELECT Item.Id
                    FROM Orchard.ContentManagement.Records.ContentItemVersionRecord ItemVersion
                    JOIN ItemVersion.ContentItemRecord Item
                    JOIN Item.ClientSideProjectionPartRecord CsProjection
                    JOIN Item.QueryPartRecord Query
                    WHERE ItemVersion.Published = true
                    AND Query.Id = :Query_Id
                    AND (:FiltersQueryString like '%' + CsProjection.PresetQueryString + '%' OR CsProjection.PresetQueryString is null);
                    ORDER BY LENGTH(CsProjection.PresetQueryString) DESC
                    TAKE 1";

            var session = _sessionLocator.For(typeof(Orchard.ContentManagement.Records.ContentItemVersionRecord));
            var result = session.CreateQuery(searchQuery)
                .SetCacheable(false)
                .SetParameter("Query_Id", queryId)
                .SetParameter("FiltersQueryString", "&" + filterQueryString + "&")
                .List<int>()
                .FirstOrDefault();

            return result;
        }
    }
}