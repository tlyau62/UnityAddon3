using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityAddon.Core.Aop
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AopAttributeAttribute : Attribute
    {
        public Type[] AopAttributes { get; }

        public AopAttributeAttribute(params Type[] aopAttributes)
        {
            if (!aopAttributes.All(attr => typeof(Attribute).IsAssignableFrom(attr)))
            {
                throw new ArgumentException("The given arguments contain type of 'Attribute'");
            }

            AopAttributes = aopAttributes;
        }
    }
}
