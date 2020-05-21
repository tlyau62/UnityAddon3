using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using UnityAddon.Hangfire;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.HangfireTest.HangfireComponentScannerStrategy
{
    [Configuration]
    public class HfConfig
    {
        [Bean]
        public virtual Mock<IBackgroundJobClient> HfClientMock()
        {
            return new Mock<IBackgroundJobClient>();
        }

        [Bean]
        public virtual IBackgroundJobClient HfClient()
        {
            return HfClientMock().Object;
        }
    }

    [HangfireProxy]
    public interface IThumbnailTask
    {
        void CreateThumbnail();
    }

    [Component]
    [Primary]
    public class ThumbnailTask : IThumbnailTask
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void CreateThumbnail()
        {
            Logger.Log += "A";
        }
    }

    [Component]
    public class Logger
    {
        public string Log = "";
    }

    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonHangfireConfig), typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>))]
    public class HangfireComponentScannerStrategy : UnityAddonTest
    {
        public HangfireComponentScannerStrategy(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [HangfireProxy]
        public IThumbnailTask ThumbnailTaskClient { get; set; }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public Mock<IBackgroundJobClient> HfClientMock { get; set; }

        [Dependency]
        public IThumbnailTask ThumbnailTaskServer { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        [Fact]
        public void ScanHangfireComponentsOnClient()
        {
            DbContextTemplate.ExecuteTransaction<TestDbContext, object>(ctx =>
            {
                ThumbnailTaskClient.CreateThumbnail();

                return null;
            });

            HfClientMock.Verify(x => x.Create(
               It.Is<Job>(job => job.Method.Name == "CreateThumbnail"),
               It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void ScanHangfireComponentsOnServer()
        {
            ThumbnailTaskServer.CreateThumbnail();

            Assert.Equal("A", Logger.Log);
        }
    }
}
