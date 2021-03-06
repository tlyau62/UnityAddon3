using Castle.DynamicProxy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using System;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.Hangfire;
using Xunit;

namespace UnityAddon.HangfireTest.HangfireProxyInterceptor
{
    public interface IThumbnailTask
    {
        void CreateThumbnail();
    }

    public class HangfireProxyInterceptor
    {
        [Fact]
        public void EnqueueTask()
        {
            var client = new Mock<IBackgroundJobClient>();
            var proxyGenerator = new ProxyGenerator();
            var hfInterceptor = new Hangfire.HangfireProxyInterceptor()
            {
                BackgroundJobClient = client.Object
            };
            var txCallbacks = new TransactionCallbacks();

            hfInterceptor.TransactionCallbacks = txCallbacks;

            var clientTask = (IThumbnailTask)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof(IThumbnailTask), hfInterceptor);

            clientTask.CreateThumbnail();

            client.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "CreateThumbnail"),
                It.IsAny<EnqueuedState>()));
        }


    }
}
