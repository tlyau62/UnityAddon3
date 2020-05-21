using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Moq;
using UnityAddon.MoqTest.Moq;
using UnityAddon.Test;
using Xunit;

namespace UnityAddon.MoqTest.PartialDependency
{
    public interface IMessageRepository
    {
        string GetMessage();
    }

    public class MessageService
    {
        [Dependency]
        public IMessageRepository MessageRepository { get; set; }
    }

    [EnableUnityAddonMoq(true)]
    public class PartialDependencyTest : UnityAddonTest
    {
        public PartialDependencyTest(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [TestSubject]
        public MessageService MessageService { get; set; }

        [Fact]
        public void Test()
        {
            Assert.Null(MessageService.MessageRepository);
        }
    }
}
