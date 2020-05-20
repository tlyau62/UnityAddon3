using Moq;
using System;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Moq;
using Xunit;
using static Moq.Mock;

namespace UnityAddon.MoqTest.Moq
{
    public interface IMessageRepository
    {
        string GetMessage();
    }

    public class EchoService
    {
        [Dependency]
        public IMessageRepository MessageRepository { get; set; }

        public string Echo()
        {
            return MessageRepository.GetMessage();
        }
    }

    [EnableUnityAddonMoq(false)]
    public class MoqTest : UnityAddonTest
    {
        [Mock]
        public Mock<IMessageRepository> MessageRepository { get; set; }

        [TestSubject]
        public EchoService EchoService { get; set; }

        [Fact]
        public void Test()
        {
            EchoService.MessageRepository = MessageRepository.Object;

            MessageRepository.Setup(m => m.GetMessage())
                .Returns("hi");
            var msg = EchoService.Echo();
            Assert.Equal("hi", msg);
        }
    }
}
