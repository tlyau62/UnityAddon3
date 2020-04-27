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

namespace UnityAddon.Core.BeanDefinition
{
    /// <summary>
    /// A receipt for bean construction.
    /// </summary>
    public abstract class MemberBeanDefinition : IBeanDefinition
    {
        private MemberInfo _member;

        public MemberBeanDefinition(MemberInfo member)
        {
            _member = member;
        }

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

                var scopeAttr = _member.GetAttribute<ScopeAttribute>();
                var scope = scopeAttr != null ? scopeAttr.Value : typeof(ContainerControlledLifetimeManager);

                return _scope = (LifetimeManager)Activator.CreateInstance(scope);
            }
        }

        public abstract string Name { get; }

        public string[] Qualifiers
        {
            get
            {
                var qAttr = _member.GetAttribute<QualifierAttribute>();
                var gAttr = _member.GetAttribute<GuidAttribute>();
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

        public string[] Profiles => _member.GetAttribute<ProfileAttribute>()?.Values ?? new string[0];

        public bool IsPrimary => _member.HasAttribute<PrimaryAttribute>();

        public override string ToString()
        {
            return Type.FullName;
        }

        public abstract object Constructor(IUnityContainer container, Type type, string name);

        public string Namespace { get; set; }

        public Type[] AutoWiredTypes { get; set; }

        public bool FromComponentScanning { get; set; }
    }
}
