using System;
using TestObjectsPostConstuct;
using Unity;

namespace UnityAddon
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            var appContext = new ApplicationContext(container, "TestObjectsPostConstuct");

            //var a = container.Resolve<Service2>();
            //var b = a.Service;

            //var a = container.Resolve<UserService>();

            //var b = a.Service1;
            //var c = a.Service2;
            //var d = a.Service3;
            //var e = a.Service4;

            var b = container.Resolve<Service2>();
            var a = container.Resolve<Service>();

            var c = Store.TestString;
        }
    }
}
