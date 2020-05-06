using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CoreTest.Aop.MethodAttributeInterceptor
{
    public interface IService
    {
        void ChainInterceptedServe();
        void CallMethodsInsideSameService();
        void CallMethodsOutsideService();
    }

    public interface ISetDep
    {
    }

    public interface ISetDep2
    {
    }

    public interface IExtraService
    {
        void ServeExtra();
    }

    [Component]
    public class Counter
    {
        public int Count { get; set; } = 0;
    }

    [Component]
    public class Service : IService, ISetDep, ISetDep2
    {
        [Dependency]
        public Counter Counter { get; set; }

        [Dependency]
        public IExtraService ExtraService { get; set; }

        [Inc(1)]
        [Mul(2)]
        public void ChainInterceptedServe()
        {
            Counter.Count++;
        }

        [Inc(1)]
        public void CallMethodsInsideSameService()
        {
            Counter.Count++;
            ChainInterceptedServe();
        }

        [Inc(1)]
        [Mul(2)]
        public void CallMethodsOutsideService()
        {
            Counter.Count++;
            ExtraService.ServeExtra();
        }
    }

    [Component]
    public class ExtraService : IExtraService
    {
        [Dependency]
        public Counter Counter { get; set; }

        [Mul(2)]
        [Inc(1)]
        public void ServeExtra()
        {
            Counter.Count++;
        }
    }
}
