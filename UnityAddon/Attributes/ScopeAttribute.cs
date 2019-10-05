using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;

namespace UnityAddon.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ScopeAttribute : Attribute
    {
        private static Dictionary<ScopeType, Type> scopeMap = new Dictionary<ScopeType, Type>()
        {
            {ScopeType.Transient, typeof(TransientLifetimeManager)},
            {ScopeType.ContainerControlled, typeof(ContainerControlledLifetimeManager)}
        };
        public Type Value { get; set; }

        public ScopeAttribute(ScopeType scopeType)
        {
            Value = scopeMap[scopeType];
        }
    }

    public enum ScopeType
    {
        Transient,
        ContainerControlled
    }
}
