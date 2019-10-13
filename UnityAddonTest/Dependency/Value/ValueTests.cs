using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Dependency.Value
{
    public enum ServiceType
    {
        Print, Write
    }

    [Component]
    public class Service
    {
        [Value("{serviceType}")]
        public ServiceType Type { get; set; }
    }

    [Trait("Dependency", "Value")]
    public class ValueTests
    {
        [Fact]
        public void PropertyFill_ResolveValue_ValueInjected()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"serviceType", "Write"},
                })
                .Build());
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Equal(ServiceType.Write, appContext.Resolve<Service>().Type);
        }
    }
}
