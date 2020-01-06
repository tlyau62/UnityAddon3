using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef.Pagination
{
    public class PageRequest
    {
        public static Pageable Of(int page, int size, Sort sort)
        {
            return new Pageable(page, size, sort);
        }
    }
}
