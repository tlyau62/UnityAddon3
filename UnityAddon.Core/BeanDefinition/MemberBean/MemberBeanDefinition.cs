using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;
using System.Runtime.InteropServices;
using Unity;

namespace UnityAddon.Core.BeanDefinition.MemberBean
{
    /// <summary>
    /// A receipt for bean construction.
    /// </summary>
    public abstract class MemberBeanDefinition : IBeanDefinition
    {
        public MemberBeanDefinition(MemberInfo member)
        {
            Member = member;
        }

        public MemberInfo Member { get; }

        public abstract Type Type { get; }

        private LifetimeManager _scope;

        public LifetimeManager Scope
        {
            get
            {
                if (_scope != null)
                {
                    return _scope;
                }

                var scopeAttr = Member.GetAttribute<ScopeAttribute>();
                var scope = scopeAttr != null ? scopeAttr.Value : typeof(ContainerControlledLifetimeManager);

                return _scope = (LifetimeManager)Activator.CreateInstance(scope);
            }
        }

        public abstract string Name { get; }

        public virtual string[] Qualifiers
        {
            get
            {
                var qAttr = Member.GetAttribute<QualifierAttribute>();
                var gAttr = Member.GetAttribute<GuidAttribute>();
                var qualifiers = new List<string>();

                if (qAttr != null)
                {
                    qualifiers.AddRange(qAttr.Values);
                }

                if (gAttr != null)
                {
                    qualifiers.Add(gAttr.Value);
                }

                return qualifiers.ToArray();
            }
        }

        public string[] Profiles => Member.GetAttribute<ProfileAttribute>()?.Values ?? new string[0];

        public bool IsPrimary => Member.HasAttribute<PrimaryAttribute>();

        public abstract object Constructor(IServiceProvider serviceProvider, Type type, string name);

        public abstract string Namespace { get; }

        public abstract Type[] AutoWiredTypes { get; }

        public bool FromComponentScanning { get; set; } = false;

        public override string ToString()
        {
            return Type.FullName;
        }
    }
}
