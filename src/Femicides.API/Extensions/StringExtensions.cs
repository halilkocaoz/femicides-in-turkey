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
        public static string ToStringQueries(this KeyValuePair<string, StringValues>[] queries)
        {
            if(!queries.AreThereNecessaryQueries())
            {
                return null;
            }
            StringBuilder strQueries = new StringBuilder();
            var i = 0;
            foreach (var item in queries)
            {
                if(item.Key.ToLower() != "page")
                {
                    strQueries.Append(item.Key + "=" + item.Value.FirstOrDefault().Replace(" ", "%20"));
                }
                if (queries.Count() > i)
                {
                    strQueries.Append('&');
                }
                i++;
            }

            return "?" + strQueries;
        }
        public static string NextPage(this String url, int totalPage, int selectedPage, string queries)
        {
            if (totalPage > 1 && totalPage > selectedPage)
            {
                if(string.IsNullOrEmpty(queries))
                {
                    url += "?page=" + (selectedPage + 1).ToString();
                }
                else
                {
                    url += queries + "page=" + (selectedPage + 1).ToString();
                }
                return url.Replace("&&", "&");
            }
            return null;
        }
        public static string PrevPage(this String url, int totalPage, int selectedPage, string queries)
        {
            if(totalPage > 1 && selectedPage > 1)
            {
                if(string.IsNullOrEmpty(queries))
                {
                    url += "?page=" + (selectedPage - 1).ToString();
                }
                else
                {
                    url += queries + "page=" + (selectedPage - 1).ToString();
                }
                return url.Replace("&&","&");
            }
            return null;
        }
    }
}