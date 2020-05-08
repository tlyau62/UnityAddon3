using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.BeanDefinition.ServiceBean;
using Unity.Lifetime;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition>
    {
        IBeanDefinitionCollection AddComponent(Type type);

        //IBeanDefinitionCollection AddFromService(Func<IServiceProvider, IBeanDefinitionCollection> action);

        IBeanDefinitionCollection AddFromExisting(IBeanDefinitionCollection beanDefCollection);

        IBeanDefinitionCollection AddFromServiceCollection(Action<IServiceCollection> servicesCallback);

        IBeanDefinitionCollection AddFromServiceCollection(IServiceCollection services);

        IBeanDefinitionCollection AddFromUnityContainer(IUnityContainer unityContainer);
    }

    public class BeanDefinitionCollection : List<IBeanDefinition>, IBeanDefinitionCollection
    {
        //public IBeanDefinitionCollection AddFromService(Func<IServiceProvider, IBeanDefinitionCollection> action)
        //{
        //    Add(new FactoryBeanDefinition<IBeanDefinitionCollection>((sp, t, n) => action(sp)));

        //    return this;
        //}

        public IBeanDefinitionCollection AddComponent(Type type)
        {
            Add(new MemberComponentBeanDefinition(type));

            return this;
        }

        public IBeanDefinitionCollection AddFromServiceCollection(Action<IServiceCollection> servicesCallback)
        {
            var services = new ServiceCollection();

            servicesCallback(services);

            AddFromServiceCollection(services);

            return this;
        }

        public IBeanDefinitionCollection AddFromServiceCollection(IServiceCollection services)
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

        public IBeanDefinitionCollection AddFromExisting(IBeanDefinitionCollection beanDefCollection)
        {
            AddRange(beanDefCollection);

            return this;
        }

        public IBeanDefinitionCollection AddFromUnityContainer(IUnityContainer unityContainer)
        {
            foreach (var reg in unityContainer.Registrations)
            {
                if (reg.RegisteredType == typeof(IUnityContainer))
                {
                    continue;
                }

                var scope = ScopeType.None;

                if (reg.LifetimeManager is ContainerControlledLifetimeManager)
                {
                    scope = ScopeType.Singleton;
                }
                else if (reg.LifetimeManager is ContainerControlledTransientManager)
                {
                    scope = ScopeType.Transient;
                }
                else if (reg.LifetimeManager is HierarchicalLifetimeManager)
                {
                    scope = ScopeType.Scoped;
                }
                else
                {
                    throw new NotImplementedException();
                }

                Add(new FactoryBeanDefinition(reg.RegisteredType, (sp, t, n) => unityContainer.Resolve(t, reg.Name), reg.Name, scope));
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
