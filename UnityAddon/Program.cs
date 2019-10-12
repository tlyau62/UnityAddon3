using System;
using Unity;
using UnityAddon.Attributes;
using Generic;
using UnityAddon.Value;

namespace Generic
{
    public interface ITest<T> { }

    [Component]
    public class Test<T> : ITest<T> { }
}

namespace UnityAddon
{
    // bean registry
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            var appContext = new ApplicationContext(container, "Generic");

            //var config = appContext.Resolve<ValueProvider>().Parse(typeof(string), "section0.key0");
        }
    }
}
