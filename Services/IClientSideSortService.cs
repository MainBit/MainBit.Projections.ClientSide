using MainBit.Projections.ClientSide.ClientSideEditors.SortCriteria;
using MainBit.Projections.ClientSide.Models.SortCriteria;
using MainBit.Projections.ClientSide.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IClientSideSortService : IDependency
    {
        void FromQueryString(ClientSideSortCriterion sortCriterion, NameValueCollection queryString);
        NameValueCollection GetSortCriteraQueryString(List<ClientSideSortCriterion> sortCriteria);
        NameValueCollection GetSortCriteraQueryString(IEnumerable<ClientSideSortCriterionOption> option);
    }

    public class ClientSideSortService : IClientSideSortService
    {
        public const string QueryStringParamName = "sort";
        public const string QueryStringValuesSeparator = "_";

        public void FromQueryString(ClientSideSortCriterion sortCriterion, NameValueCollection queryString)
        {
            var param = queryString.GetValues(ClientSideSortService.QueryStringParamName);
            if (param == null) { return; }

            var currentSortValues = param.SelectMany(value => value.Split(
                new string[] { ClientSideSortService.QueryStringValuesSeparator }, StringSplitOptions.RemoveEmptyEntries)).ToList();
            if (currentSortValues == null) { return; }

            foreach (var option in sortCriterion.Options)
            {
                var applyingPosition = currentSortValues.IndexOf(option.Value);
                if (applyingPosition != -1)
                {
                    sortCriterion.ApplyingOption = option;
                    sortCriterion.ApplyingPosition = applyingPosition;
                }
            }
        }

        public NameValueCollection GetSortCriteraQueryString(List<ClientSideSortCriterion> sortCriteria) {
            return GetSortCriteraQueryString(sortCriteria.Where(s => s.ApplyingPosition.HasValue).OrderBy(s => s.ApplyingPosition).Select(s => s.ApplyingOption));
        }
        
        public NameValueCollection GetSortCriteraQueryString(IEnumerable<ClientSideSortCriterionOption> options)
        {
            var sortCriteriaQueryString = HttpUtility.ParseQueryString(string.Empty);

            var value = string.Join(
                ClientSideSortService.QueryStringValuesSeparator,
                options.Select(option => option.Value));

            if (!string.IsNullOrEmpty(value))
            {
                sortCriteriaQueryString[ClientSideSortService.QueryStringParamName] = value;
            }

            return sortCriteriaQueryString;
        }
    }
}