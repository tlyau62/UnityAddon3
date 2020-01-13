using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.Pagination
{
    public class Pageable
    {
        public Pageable()
        {
        }

        public Pageable(int page, int size, string sort = "")
        {
            Page = page;
            Size = size;
            Sort = sort;
        }

        public int Page { get; set; }

        public int Size { get; set; }

        public string Sort { get; set; }
    }
}
