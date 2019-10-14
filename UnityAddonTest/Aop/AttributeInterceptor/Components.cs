using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Attributes;

namespace UnityAddonTest.Aop.AttributeInterceptor
{
    [Component]
    public class Counter
    {
        public int Count { get; set; } = 0;
    }

    public interface IService
    {
        void ChainInterceptedServe();
        void CallMethodsInsideSameService();
        void CallMethodsOutsideService();
    }

    public interface ISetDep
    {
        Counter CounterAccess();
    }

    public interface ISetDep2
    {
        Counter CounterAccess2();
    }

    [Component]
    public class Service : IService, ISetDep, ISetDep2
    {
        [Dependency]
        public Counter Counter { get; set; }

        [Dependency]
        public IExtraService ExtraService { get; set; }

        [Inc]
        [Mul2]
        public void ChainInterceptedServe()
        {
            Counter.Count++;
        }

        [Inc]
        public void CallMethodsInsideSameService()
        {
            Counter.Count++;
            ChainInterceptedServe();
        }

        [Inc]
        [Mul2]
        public void CallMethodsOutsideService()
        {
            Counter.Count++;
            ExtraService.ServeExtra();
        }

        public Counter CounterAccess()
        {
            return Counter;
        }

        public Counter CounterAccess2()
        {
            return Counter;
        }
    }

    public interface IExtraService
    {
        void ServeExtra();
    }

    [Component]
    public class ExtraService : IExtraService
    {
        [Dependency]
        public Counter Counter { get; set; }

        [Mul2]
        [Inc]
        public void ServeExtra()
        {
            Counter.Count++;
        }
    }

    [Component]
    public class VirtualService
    {
        [Inc]
        public void Serve()
        {
        }
    }
}
