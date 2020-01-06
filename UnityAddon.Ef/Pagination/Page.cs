using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef.Pagination
{
    public class Page<T>
    {
        public IEnumerable<T> Content { get; set; }

        public Pageable Pagination { get; set; }
    }
}
