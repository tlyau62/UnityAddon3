using Castle.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.Test
{
    public abstract class UnityAddonTest : IClassFixture<UnityAddonTestFixture>
    {
        protected IHostBuilder HostBuilder { get; private set; }

        protected IHost Host { get; private set; }

        protected UnityAddonTestFixture TestFixture { get; }

        public UnityAddonTest(UnityAddonTestFixture testFixture) : this(testFixture, false)
        {
        }

        public UnityAddonTest(UnityAddonTestFixture testFixture, bool isDefered)
        {
            TestFixture = testFixture;

            testFixture.UnityAddonTest = this;

            if (!isDefered)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            TestFixture.Init();
            TestFixture.Build();
            TestFixture.BuildUp();
            HostBuilder = TestFixture.TestHostBuilder;
            Host = TestFixture.TestHost;
        }
    }
}
