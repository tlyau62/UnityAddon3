using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Test
{
    public interface IService { }

    [Component]
    public class Service : IService
    {
        // [Dependency]
        // public IService2 Service2 { get; set; }
    }

    public interface IService2 { }

    [Configuration]
    public class Service2 : IService2
    {
        [Dependency]
        public IService Service { get; set; }
    }
}
