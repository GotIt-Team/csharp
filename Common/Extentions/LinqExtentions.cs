using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Common.Extentions
{
    public static class LinqExtentions
    {
        public static IQueryable<TSource> IncludeMultiple<TSource>(this IQueryable<TSource> query, string[] includes) where TSource : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));
            }

            return query;
        }
    }
}
