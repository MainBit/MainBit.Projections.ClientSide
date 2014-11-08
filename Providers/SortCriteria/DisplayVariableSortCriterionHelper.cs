using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Providers.SortCriteria
{
    public class DisplayVariableSortCriterionHelper
    {
        public static LocalizedString DisplaySortCriterion(SortCriterionContext context, string name, Localizer T)
        {
            var sSort = Convert.ToString(context.State.Sort);
            string sFirstDirection = string.Empty;
            if (IsToken(sSort))
            {
                sFirstDirection = sSort;
            }
            else
            {
                var sort = (SortDirection)Enum.Parse(typeof(SortDirection), sSort);
                switch (sort)
                {
                    case SortDirection.None:
                        break;
                    case SortDirection.Ascending:
                        sFirstDirection = "ascending";
                        break;
                    case SortDirection.Descending:
                        sFirstDirection = "descending";
                        break;
                    default:
                        break;
                }
            }

            var sortUndefined = (SortDirection)Enum.Parse(typeof(SortDirection), Convert.ToString(context.State.SortUndefined));
            var display = "Ordered by field {0} with {0} direction.";

            if (IsToken(sSort))
            {

                switch (sortUndefined)
                {
                    case SortDirection.None:
                        break;
                    case SortDirection.Ascending:
                        display += " Order by ascending if first order direction is undefined";
                        break;
                    case SortDirection.Descending:
                        display += " Order by descending if first order direction is undefined";
                        break;
                    default:
                        break;
                }
            }

            return T(display, name, sFirstDirection);
        }

        public static SortDirection GetSortDirection(SortCriterionContext context, ITokenizer tokenizer)
        {
            var sSort = Convert.ToString(context.State.Sort);
            sSort = tokenizer.Replace(sSort, new Dictionary<string, object>());
            SortDirection sort;
            if (Enum.IsDefined(typeof(SortDirection), sSort))
            {
                sort = (SortDirection)Enum.Parse(typeof(SortDirection), sSort);
            }
            else
            {
                sort = (SortDirection)Enum.Parse(typeof(SortDirection), Convert.ToString(context.State.SortUndefined));
            }
            return sort;
        }

        private static bool IsToken(string value)
        {
            return value.StartsWith("{") && value.EndsWith("}");
        }
    }
}