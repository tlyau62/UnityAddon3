using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityAddon.Utilities.Pagination
{
    public static class Pagination
    {
        public static Page<T> ToPage<T>(this IQueryable<T> queryable, Pageable pageable)
        {
            var page = new Page<T>();
            var type = typeof(T);

            page.Content = queryable;

            if (pageable.Sort != null)
            {
                foreach (var order in pageable.Sort.Orders)
                {
                    if (order.Direction == Sort.Direction.ASC)
                    {
                        page.Content = page.Content is IOrderedEnumerable<T> ordered ?
                            ordered.ThenBy(t => GetPropertyValue(order.Property, t)) :
                            page.Content.OrderBy(t => GetPropertyValue(order.Property, t));
                    }
                    else if (order.Direction == Sort.Direction.DESC)
                    {
                        page.Content = page.Content is IOrderedEnumerable<T> ordered ?
                            ordered.ThenByDescending(t => GetPropertyValue(order.Property, t)) :
                            page.Content.OrderByDescending(t => GetPropertyValue(order.Property, t));
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            page.Content = page.Content.Skip(pageable.Page * pageable.Size).Take(pageable.Size).ToList();
            page.Pagination = pageable;

            return page;
        }

        private static object GetPropertyValue<T>(string path, T source)
        {
            var pp = path.Split('.');
            object instance = source;
            Type t = instance.GetType();

            foreach (var prop in pp)
            {
                PropertyInfo propInfo = t.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance);
                    t = propInfo.PropertyType;
                }
                else throw new ArgumentException("Properties path is not correct");
            }

            return instance;
        }

    }
}
