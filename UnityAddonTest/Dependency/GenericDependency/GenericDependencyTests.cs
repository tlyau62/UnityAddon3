using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Dependency.GenericDependency
{
    public abstract class Context { }

    public class AppContext : Context { }

    public interface IContextHelper<T> where T : Context { }

    [Component]
    public class ContextHelper<T> : IContextHelper<T> where T : Context { }

    public interface IContextFactory<T> where T : Context
    {
        IContextHelper<T> ContextHelper { get; set; }
    }

    [Component]
    public class ContextFactory<T> : IContextFactory<T> where T : Context
    {
        [Dependency]
        public IContextHelper<T> ContextHelper { get; set; }
    }

    [Trait("Dependency", "GenericDependency")]
    public class GenericBeanTests
    {
        [Fact]
        public void BuildStrategy_ResolveGenericDependency_BeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var contextHelper = appContext.Resolve<IContextHelper<AppContext>>();
            var contextFactory = appContext.Resolve<IContextFactory<AppContext>>();

            Assert.Same(contextHelper, contextFactory.ContextHelper);
        }
    }
}
