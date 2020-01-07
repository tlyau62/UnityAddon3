using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Reflection;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.Ef
{
    /// <summary>
    /// Extract the data source specified as [DataSourceAttribute] from a method / class type.
    /// Search from method, if not found then class, finally global, where global data source is a
    /// type of a subclass of a dbcontext.
    /// If mulitple dbcontext subclasses are found, the one marked
    /// with [Primary] will become the global datasource.
    /// </summary>
    [Component]
    public class DataSourceExtractor
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public Type GlobalDataSource { get => BeanDefinitionContainer.GetBeanDefinition(typeof(DbContext)).GetBeanType(); }

        public Type ExtractDataSource(MethodInfo method)
        {
            if (method.HasAttribute<DataSourceAttribute>())
            {
                return method.GetAttribute<DataSourceAttribute>().Entity;
            }
            else
            {
                return ExtractDataSource(method.DeclaringType);
            }
        }

        public Type ExtractDataSource(Type type)
        {
            if (type.HasAttribute<DataSourceAttribute>(true))
            {
                return type.GetAttribute<DataSourceAttribute>(true).Entity;
            }
            else
            {
                return GlobalDataSource;
            }
        }

    }
}
