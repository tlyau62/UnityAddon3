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
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.Hangfire;
using UnityAddon.HangfireTest.Common;
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
    [Qualifier("client")]
    public interface IThumbnailTask
    {
        void CreateThumbnail();
    }

    public class HangfireComponentScannerStrategy
    {
        [Dependency("client")]
        public IThumbnailTask ThumbnailTaskClient { get; set; }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public Mock<IBackgroundJobClient> HfClientMock { get; set; }

        public HangfireComponentScannerStrategy()
        {
            new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace, "UnityAddon.HangfireTest.Common")
                .EnableUnityAddonEf()
                .EnableUnityAddonHangfire()
                .BuildUA()
                .BuildTestUA(this);
        }

        [Fact]
        public void ScanHangfireComponents()
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
    }
}
