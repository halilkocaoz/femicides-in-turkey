using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Femicides.API.Extensions
{
    public static class StringExtensions
    {
        public static bool AreThereNecessaryQueries(this KeyValuePair<string, StringValues>[] queries)
        {
            switch (queries.Count())
            {
                case 0:
                case 1 when queries[0].Key.ToLower() == "page":
                    return false;
                default:
                    return true;
            }
        }
        public static string ToStringWithOutPageParam(this KeyValuePair<string, StringValues>[] queries)
        {
            if(!queries.AreThereNecessaryQueries())
            {
                return null;
            }
            StringBuilder strQueries = new StringBuilder();
            for (int i = 0; i < queries.Length; i++)
            {
                if(queries[i].Key == "page")
                {
                    continue;
                }
                strQueries.Append(queries[i].Key + "=" + queries[i].Value.FirstOrDefault().Replace(" ", "%20") + "&");
            }
            return "?" + strQueries.Remove(strQueries.Length - 1, 1);
        }
        public static string RequestedUrlPaginationWithParams(this String requestedHostPath, int totalPage, int selectedPage, string queries, int jump)
        {
            if (totalPage > 1 && (totalPage > selectedPage && jump == 1) || (selectedPage > 1 && jump == -1))
            {
                if(string.IsNullOrEmpty(queries))
                {
                    requestedHostPath += "?page=" + (selectedPage + jump).ToString();
                }
                else
                {
                    requestedHostPath += queries + "&page=" + (selectedPage + jump).ToString();
                }
                return requestedHostPath;
            }
            return null;
        }
    }
}