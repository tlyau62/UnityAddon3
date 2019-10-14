using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Attributes;

namespace UnityAddonTest.Aop.AttributeInterceptor
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
}
