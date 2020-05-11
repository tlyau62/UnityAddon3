using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using System.Reflection;
using Castle.DynamicProxy;
using UnityAddon.Core.Bean.DependencyInjection;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Register the constructor of any bean definition
    /// into the unity container.
    /// </summary>
    [Component]
    public class BeanFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        [Dependency]
        public ConstructorResolver CtorResolver { get; set; }

        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        [Dependency]
        public PropertyFill PropertyFill { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        public object Construct(Type type, IUnityAddonSP sp)
        {
            if (type.IsInterface)
            {
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type);
            }

            var ctor = CtorResolver.ChooseConstuctor(type, sp);
            var bean = ctor.Invoke(ParameterFill.FillAllParamaters(ctor, sp).ToArray());

            PropertyFill.FillAllProperties(type, bean, sp);

            return PostConstruct(type, bean);
        }

        public object ConstructClassProxy(Type type, IEnumerable<IInterceptor> interceptors, IUnityAddonSP sp)
        {
            var proxyBean = ProxyGenerator.CreateClassProxy(
                type,
                ParameterFill.FillAllParamaters(CtorResolver.ChooseConstuctor(type, sp), sp),
                interceptors.ToArray());

            PropertyFill.FillAllProperties(type, proxyBean, sp);

            return PostConstruct(type, proxyBean);
        }

        public object PostConstruct(Type type, object bean)
        {
            var postConstructors = MethodSelector.GetAllMethodsByAttribute<PostConstructAttribute>(type); // context.Existing.GetType()

            foreach (var pc in postConstructors)
            {
                if (pc.GetParameters().Length > 0 || pc.ReturnType != typeof(void))
                {
                    throw new InvalidOperationException("no-arg, void");
                }

                pc.Invoke(bean, new object[0]);
            }

            return bean;
        }
    }

}
