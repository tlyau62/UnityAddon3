using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Core.Reflection
{
    /// <summary>
    /// Choose the 1st constructor with InjectionConstructorAttribute, or
    /// the constructor with the max number of parameters
    /// </summary>
    public static class DefaultConstructor
    {
        public static ConstructorInfo Select(Type type)
        {
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var defaultCtor = ctors[0];

            for (var i = 1; i < ctors.Length && !defaultCtor.HasAttribute<InjectionConstructorAttribute>(); i++)
            {
                var ctor = ctors[i];

                if (ctor.GetParameters().Length > defaultCtor.GetParameters().Length)
                {
                    defaultCtor = ctor;
                }
            }

            return defaultCtor;
        }
    }
}
