﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.BeanDefinition.ServiceBean;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition>
    {
        public IBeanDefinitionCollection AddComponent(Type type);
    }

    public class BeanDefinitionCollection : List<IBeanDefinition>, IBeanDefinitionCollection
    {
        public IBeanDefinitionCollection AddComponent(Type type)
        {
            Add(new MemberComponentBeanDefinition(type));

            return this;
        }

        public IBeanDefinitionCollection MergeServiceCollection(IServiceCollection services)
        {
            foreach (var d in services)
            {
                ServiceBeanDefinition beanDef = null;

                if (d.ImplementationInstance != null)
                {
                    beanDef = new ServiceInstanceBeanDefinition(d);
                }
                else if (d.ImplementationFactory != null)
                {
                    beanDef = new ServiceFactoryBeanDefinition(d);
                }
                else if (d.ImplementationType != null)
                {
                    beanDef = new ServiceTypeBeanDefinition(d);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                Add(beanDef);
            }

            return this;
        }
    }

    public static class BeanDefinitionCollectionExt
    {
        public static IBeanDefinitionCollection AddComponent<T>(this IBeanDefinitionCollection beanDefCollection)
        {
            return beanDefCollection.AddComponent(typeof(T));
        }
    }
}
