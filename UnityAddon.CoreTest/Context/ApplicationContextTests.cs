using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Context
{
    [Component]
    public class Logger
    {
        public string Log { get; set; } = "";
    }

    [Component]
    public class Service
    {
        [Dependency]
        public Logger Logger { get; set; }

        [PostConstruct]
        public void Setup()
        {
            Logger.Log += "A";
        }
    }

    [Component]
    [Order(1)]
    public class PostInstantiateA : IAppCtxFinishPhase
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Process()
        {
            Logger.Log += "B";
        }
    }

    [Component]
    [Order(2)]
    public class PostInstantiateB : IAppCtxFinishPhase
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Process()
        {
            Logger.Log += "C";
        }
    }

    [Component]
    [Order(4)]
    public class PostInstantiateC : IAppCtxFinishPhase
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Process()
        {
            Logger.Log += "D";
        }
    }

    [Component]
    [Order(3)]
    public class PostInstantiateD : IAppCtxFinishPhase
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Process()
        {
            Logger.Log += "E";
        }
    }

    [ComponentScan]
    public class ApplicationContextTests : UnityAddonTest
    {
        public ApplicationContextTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public Logger Logger { get; set; }

        [Fact]
        public void PostInstantiateSingleton()
        {
            Assert.Equal("ABCED", Logger.Log);
        }
    }
}
