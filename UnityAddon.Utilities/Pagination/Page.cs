using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.Pagination
{
    /// <summary>
    /// aka Chunk
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T>
    {
        public IEnumerable<T> Content { get; set; }

        public Pageable Pagination { get; set; }
    }
}
