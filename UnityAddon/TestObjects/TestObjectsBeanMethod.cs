using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace TestObjectsBeanMethod
{
    public class Service
    {
    }

    public class Service2
    {
    }

    public static class IntStore
    {
        public static int TestInt = 0;
    }

    [Configuration]
    public class Config
    {
        [Bean]
        public virtual Service CreateService()
        {
            IntStore.TestInt++;

            return new Service();
        }

        [Bean]
        public virtual Service2 CreateService2()
        {
            IntStore.TestInt++;

            CreateService();
            return new Service2();
        }
    }

}
