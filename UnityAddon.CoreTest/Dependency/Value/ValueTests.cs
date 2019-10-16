using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Value;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.Value
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
            var appContext = new ApplicationContext(container, GetType().Namespace);

            appContext.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"serviceType", "Write"},
                })
                .Build());

            Assert.Equal(ServiceType.Write, appContext.Resolve<Service>().Type);
        }
    }
}
