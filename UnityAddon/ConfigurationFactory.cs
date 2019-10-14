using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Reflection;

namespace UnityAddon
{
    [Component]
    public class ConfigurationFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public BeanMethodInterceptor BeanMethodInterceptor { get; set; }

        public object CreateConfiguration(Type type, ConstructorInfo constructor)
        {
            return ProxyGenerator.CreateClassProxy(
                type,
                ParameterFill.FillAllParamaters(constructor),
                BeanMethodInterceptor);
        }
    }
}
