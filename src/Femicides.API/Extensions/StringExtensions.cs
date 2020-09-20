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
    }
}