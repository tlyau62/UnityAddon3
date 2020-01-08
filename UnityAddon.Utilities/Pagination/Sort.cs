using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.Pagination
{
    public class Sort
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public static Sort Asc(string property)
        {
            var sort = new Sort();

            sort.ThenAsc(property);

            return sort;
        }

        public static Sort Desc(string property)
        {
            var sort = new Sort();

            sort.ThenDesc(property);

            return sort;
        }

        public Sort ThenAsc(string property)
        {
            Orders.Add(Order.By(property, Direction.ASC));

            return this;
        }

        public Sort ThenDesc(string property)
        {
            Orders.Add(Order.By(property, Direction.DESC));

            return this;
        }

        public class Order
        {
            public Direction Direction { get; set; }

            public string Property { get; set; }

            public static Order By(string property, Direction direction)
            {
                var order = new Order();

                order.Direction = direction;
                order.Property = property;

                return order;
            }

        }

        public enum Direction
        {
            ASC, DESC
        }
    }
}
