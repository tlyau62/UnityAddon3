using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.Pagination
{
    public class PageRequest
    {
        public static Pageable Of(int page, int size, Sort sort = null)
        {
            return new Pageable(page, size, sort);
        }
    }
}
