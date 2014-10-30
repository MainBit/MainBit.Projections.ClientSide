using Orchard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using MainBit.Projections.ClientSide.Models;
using MainBit.Projections.ClientSide.Models.Filters;
using MainBit.Projections.ClientSide.Models.SortCriteria;
using Orchard.Data;

namespace MainBit.Projections.ClientSide.Services
{
    public interface IUrlService : IDependency
    {
        NameValueCollection MakeCleanQueryString(string queryString);
        NameValueCollection MergeQueryString(NameValueCollection queryString, NameValueCollection presetQueryString);
        string BuildUrl(string contentUrl,
            NameValueCollection presetQueryString,
            NameValueCollection filterQueryString,
            NameValueCollection sortCriteriaQueryString,
            NameValueCollection layoutQueryString,
            NameValueCollection fullQueryString,
            List<string> knowingKeys,
            bool removeOthers = false);

        string RebuildUrl(string url, List<string> knowingKeys, NameValueCollection specifcValues, bool removeOthers = false);
    }


    public class UrlService : IUrlService
    {
        public NameValueCollection MakeCleanQueryString(string queryString)
        {
            return HttpUtility.ParseQueryString(
                string.IsNullOrWhiteSpace(queryString)
                ? string.Empty 
                : queryString.Trim(new char[] { '&', '?' }));
        }

        public NameValueCollection MergeQueryString(NameValueCollection queryString, NameValueCollection presetQueryString)
        {
            // when user change client side filter and send this request that parameter with search name exists in url
            if (queryString["search"] == "1")
            {
                return queryString;
            }
            else
            {
                var fullQueryString = HttpUtility.ParseQueryString(string.Empty);
                fullQueryString.Add(presetQueryString);
                fullQueryString.Add(queryString);
                return fullQueryString;
            }
        }

 
        public string BuildUrl(string contentUrl,
            NameValueCollection presetQueryString,
            NameValueCollection filterQueryString,
            NameValueCollection sortCriteriaQueryString,
            NameValueCollection layoutQueryString,
            NameValueCollection fullQueryString,
            List<string> knowingKeys,
            bool removeOthers = false
            )
        {
            var othersQueryString = HttpUtility.ParseQueryString(string.Empty); // such params like page or referal
            knowingKeys.Insert(0, "search");
            foreach (var key in fullQueryString.AllKeys)
            {
                if (!knowingKeys.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                {
                    othersQueryString.Add(key, fullQueryString[key]);
                }
            }

            var accurateQueryString = HttpUtility.ParseQueryString(string.Empty);
            if (fullQueryString["search"] == "1" && !String.Equals(filterQueryString.ToString(), presetQueryString.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                accurateQueryString.Add("search", "1");
                accurateQueryString.Add(filterQueryString);
            }
            accurateQueryString.Add(sortCriteriaQueryString);
            accurateQueryString.Add(layoutQueryString);
            if (!removeOthers)
            {
                accurateQueryString.Add(othersQueryString);
            }

            if (accurateQueryString.Count > 0)
            {
                contentUrl += (contentUrl.Contains("?") ? "&" : "?") + accurateQueryString.ToString();
            }
            return contentUrl;
        }

        public string RebuildUrl(string url, List<string> knowingKeys, NameValueCollection specifcValues, bool removeOthers = false)
        {
            knowingKeys.Insert(0, "search");

            var splitUrl = url.Split('?');
            var sQueryString = splitUrl.Length == 2 ? splitUrl[1] : string.Empty;
            var queryString = System.Web.HttpUtility.ParseQueryString(sQueryString);

            var qsParams = queryString.AllKeys
                .Select(key => new QueryStringParam {
                    Key = key,
                    Value = queryString[key],
                    ShouldIndex = knowingKeys.IndexOf(key)
                    })
                .ToList();

            var additionlIndex = 0;
            foreach (var specifcKey in specifcValues.AllKeys)
            {
                var qsParam = qsParams.FirstOrDefault(p => p.Key == specifcKey);
                if (string.IsNullOrEmpty(specifcValues[specifcKey]))
                {
                    if(qsParam != null) {
                        qsParams.Remove(qsParam);
                    }
                }
                else {
                    if (qsParam == null)
                    {
                        qsParam = new QueryStringParam
                        {
                            Key = specifcKey,
                            ShouldIndex = knowingKeys.IndexOf(specifcKey)
                        };
                        qsParams.Add(qsParam);
                    }

                    qsParam.Value = specifcValues[specifcKey];

                    if (qsParam.ShouldIndex == -1)
                    {
                        qsParam.ShouldIndex = knowingKeys.Count + additionlIndex;
                        additionlIndex++;
                    }
                }
            }

            if (removeOthers)
            {
                qsParams.RemoveAll(qsParam => qsParam.ShouldIndex == -1);
            }

            var newQueryString = string.Join("&", qsParams.OrderBy(p => p.ShouldIndex == -1 ? Int32.MaxValue : p.ShouldIndex).Select(p => p.Key + "=" + p.Value));
            return splitUrl[0] + (string.IsNullOrEmpty(newQueryString) ? string.Empty : ("?" + newQueryString));
        }
    }

    public class QueryStringParam
    {
        public int ShouldIndex { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}