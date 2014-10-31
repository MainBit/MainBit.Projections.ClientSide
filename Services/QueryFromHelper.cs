using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide.Services
{
    public static class QueryFormHelper
    {
        public static string GetDisplayName(string descriptorName)
        {
            var indexOf = descriptorName.LastIndexOf(':');
            if (indexOf > 0)
            {
                return descriptorName.Substring(0, indexOf);
            }
            return descriptorName;
        }

        public static string GetName(string descriptorCategory, string descriptorType)
        {
            var segments = descriptorType.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return segments.Last().ToLower();
        }
    }
}