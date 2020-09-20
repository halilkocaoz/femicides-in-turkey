using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Femicides.API.Extensions
{
    public static class StringExtensions
    {
        public static string ToStringQueries(this KeyValuePair<string, StringValues>[] queries)
        {
            if(queries == null || (queries.Count() == 1 && queries[0].Key.ToLower() == "page"))
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
        public static string NextPage(this String str, int totalPage, int selectedPage, string queries)
        {
            if (totalPage > 1 && totalPage > selectedPage)
            {
                str += queries;
                if(string.IsNullOrEmpty(queries))
                {
                    str += "?page=" + (selectedPage + 1).ToString();
                }
                else
                {
                    str += "page=" + (selectedPage + 1).ToString();
                }

                return str.Replace("&&", "&");
            }

            return null;
        }
        public static string PrevPage(this String str, int totalPage, int selectedPage, string queries)
        {
            if(totalPage > 1 && selectedPage > 1)
            {
                str += queries;
                if(string.IsNullOrEmpty(queries))
                {
                    str += "?page=" + (selectedPage - 1).ToString();
                }
                else
                {
                    str += "page=" + (selectedPage - 1).ToString();
                }
                return str.Replace("&&","&");
            }

            return null;
        }
    }
}