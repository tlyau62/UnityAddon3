using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.DynamicProxy;
using Unity;

namespace UnityAddon.Core.EF.Transaction
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireDbContextAttribute : Attribute
    {
        public bool Transactional { get; set; } = false;
    }
}