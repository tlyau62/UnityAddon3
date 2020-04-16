using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Moq;
using Xunit;

namespace UnityAddon.MoqTest.PartialDependency
{
    public interface IMessageRepository
    {
        string GetMessage();
    }

    [Component]
    public class MessageService
    {
        [Dependency]
        public IMessageRepository MessageRepository { get; set; }
    }

    public class PartialDependencyTest : AbstractUnityAddonMoqTest
    {
        [Dependency]
        public MessageService MessageService { get; set; }

        public PartialDependencyTest() : base(true) { }

        [Fact]
        public void Test()
        {
            Assert.Null(MessageService.MessageRepository);
        }
    }
}
