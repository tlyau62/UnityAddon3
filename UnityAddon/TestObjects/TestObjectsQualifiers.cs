using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace TestObjects
{
    public interface IService { }

    [Component]
    [Qualifier("a")]
    public class Service : IService
    {
    }

    [Component]
    [Qualifier("b")]
    public class Service2 : IService
    {
    }

    [Component]
    public class UserService
    {
        [Dependency("Service")]
        public IService Service1 { get; set; }

        [Dependency("Service2")]
        public IService Service2 { get; set; }

        [Dependency("a")]
        public IService Service3 { get; set; }

        [Dependency("b")]
        public IService Service4 { get; set; }
    }
}
