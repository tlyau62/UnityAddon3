using System;
using Unity;
using TestObjectsBeanMethod;
using UnityAddon.Attributes;
using Generic;

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

            var a = container.Resolve<Test<string>>();
            var b = container.Resolve<ITest<string>>();
            var c = a == b;
        }
    }
}
