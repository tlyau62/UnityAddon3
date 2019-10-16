using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanBuildStrategies
{
    public class ResolveStackEntry
    {
        public Type ResolveType { get; set; }
        public string ResolveName { get; set; }
        public bool IsBaseResolve { get; set; } // true: the 1st call to container.resolve

        public ResolveStackEntry(Type resolveType, string resolveName, bool isBaseResolve = false)
        {
            ResolveType = resolveType;
            ResolveName = resolveName;
            IsBaseResolve = isBaseResolve;
        }
    }
}
