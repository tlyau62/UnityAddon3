using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

namespace UnityAddon.Utilities.Pagination
{
    public static class Pagination
    {
        public static Page<T> ToPage<T>(this IQueryable<T> queryable, Pageable pageable)
        {
            var page = new Page<T>();

            page.Content = string.IsNullOrEmpty(pageable.Sort) ? queryable : queryable.OrderBy(pageable.Sort);
            page.Content = page.Content.Skip(pageable.Page * pageable.Size).Take(pageable.Size).ToList();
            page.Pagination = pageable;

            return page;
        }
    }
}
