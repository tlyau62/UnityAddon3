using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [Component]
    public class Logger
    {
        public string Log { get; set; } = "";
    }

    public interface IService
    {
        void Inc<T>(T t);
    }

    [Component]
    public class Service : IService
    {
        [Dependency]
        public Logger Logger { get; set; }

        [Prefix("test")]
        public void Inc<T>(T t)
        {
            Logger.Log += t.ToString();
        }
    }
}
