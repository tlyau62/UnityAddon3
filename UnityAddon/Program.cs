using System;
using Unity;
using TestObjectsBeanMethod;

namespace UnityAddon
{
    // bean registry
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            var appContext = new ApplicationContext(container, "TestObjectsBeanMethod");

            var a = container.Resolve<Service>();
            var b = container.Resolve<Service2>();
            var c = IntStore.TestInt;
        }
    }
}
