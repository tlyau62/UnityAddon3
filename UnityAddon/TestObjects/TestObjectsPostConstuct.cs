using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace TestObjectsPostConstuct
{
    public static class Store
    {
        public static string TestString = "";
    }

    [Component]
    public class Service
    {
        [PostConstruct]
        public void Init()
        {
            Store.TestString += "A";
        }
    }

    [Component]
    public class Service2
    {
        [PostConstruct]
        public void Init()
        {
            Store.TestString += "B";
        }
    }
}
