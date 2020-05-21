﻿using System;
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
    public class PostInstantiate : IContextPostInstantiateSingleton
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void PostInitialize()
        {
            Logger.Log += "B";
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
            Assert.Equal("AB", Logger.Log);
        }
    }
}
