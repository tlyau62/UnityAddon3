using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.CoreTest.Util.Mocks.ComponentScanning;
using Xunit;

namespace UnityAddon.CoreTest.Util.Mocks.ComponentScanning
{
    public interface IService { }

    [Component]
    public class Service : IService { }

    [Configuration]
    public class Config { }
}

namespace UnityAddon.CoreTest.Util
{
    public class ComponentScannerTests
    {
        [Fact]
        public void ScanComponents()
        {
            IBeanDefinitionCollection defCol = new BeanDefinitionCollection();

            defCol.AddFromComponentScanner(cs => cs.ScanAssembly(GetType().Assembly, GetType().Namespace));

            Assert.NotNull(defCol.Where(def => def.Type == typeof(Service)).Single());
            Assert.NotNull(defCol.Where(def => def.Type == typeof(Config)).Single());
            Assert.Equal(2, defCol.Count());
        }
    }
}
