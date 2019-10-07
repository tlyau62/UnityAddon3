using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Dependency.Bean.GenericBean
{
    public interface IService
    {
        IRepo Repo { get; set; }
        IRepo2 Repo2 { get; set; }
    }
    public interface IRepo
    {
        IContextFactory<Context> ContextFactory { get; set; }
    }
    public interface IRepo2
    {
        IContextFactory<Context> ContextFactory { get; set; }
    }

    [Component]
    internal class Service : IService
    {
        [Dependency]
        public IRepo Repo { get; set; }

        [Dependency]
        public IRepo2 Repo2 { get; set; }
    }

    [Component]
    internal class Repo : IRepo
    {
        [Dependency]
        public IContextFactory<Context> ContextFactory { get; set; }
    }

    [Component]
    internal class Repo2 : IRepo2
    {
        [Dependency]
        public IContextFactory<Context> ContextFactory { get; set; }
    }

    public class Context { }

    public interface IContextFactory<T> where T : Context { }

    [Component]
    public class ContextFactory<T> : IContextFactory<T> where T : Context { }

    [Trait("Bean", "GenericBean")]
    public class GenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnGenericTypeBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var service = container.Resolve<IService>();
            var repo = container.Resolve<IRepo>();
            var repo2 = container.Resolve<IRepo2>();
            var factory = container.Resolve<IContextFactory<Context>>();

            Assert.Same(service.Repo, repo);
            Assert.Same(service.Repo2, repo2);
            Assert.Same(repo.ContextFactory, factory);
            Assert.Same(repo2.ContextFactory, factory);
        }
    }
}
