using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Core
{
    /// <summary>
    /// Create a configuration bean.
    /// </summary>
    [Component]
    public class ConfigurationFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        public object CreateConfiguration(Type type, ConstructorInfo constructor, IUnityContainer container)
        {
            return ProxyGenerator.CreateClassProxy(
                type,
                ParameterFill.FillAllParamaters(constructor, container),
                container.BuildUp(new BeanMethodInterceptor(container)));
        }
    }
}
