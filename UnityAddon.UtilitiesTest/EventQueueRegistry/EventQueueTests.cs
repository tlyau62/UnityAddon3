using System;
using System.Collections.Generic;
using UnityAddon.Utilities.EventQueue;
using Xunit;

namespace UnityAddon.UtilitiesTest.EventQueueRegistry
{
    [Trait("Utitilies", "EventQueue")]
    public class EventQueueTests
    {
        [Fact]
        public void EventQueue_RegisterEmptyEvent_EventTriggered()
        {
            var evtQ = new EventQueue();
            var isEvtTrigger = false;

            evtQ.RegisterEvent<object>("test", p => isEvtTrigger = true);

            Assert.False(isEvtTrigger);

            evtQ.TriggerEvents<object>("test", null);

            Assert.True(isEvtTrigger);
        }

        [Fact]
        public void EventQueue_RegisterPrimitiveEvent_EventTriggered()
        {
            var evtQ = new EventQueue();
            var isEvtTrigger = false;

            evtQ.RegisterEvent<string>("test", p =>
            {
                Assert.Equal("string_data", p.Data);

                isEvtTrigger = true;
            });

            Assert.False(isEvtTrigger);

            evtQ.TriggerEvents("test", "string_data");

            Assert.True(isEvtTrigger);
        }

        [Fact]
        public void EventQueue_RegisterObjectEvent_EventTriggered()
        {
            var evtQ = new EventQueue();
            var isEvtTrigger = false;

            evtQ.RegisterEvent<string[]>("test", p =>
            {
                Assert.Equal(new string[] { "val1", "val2" }, p.Data);

                isEvtTrigger = true;
            });

            Assert.False(isEvtTrigger);

            evtQ.TriggerEvents("test", new string[] { "val1", "val2" });

            Assert.True(isEvtTrigger);
        }

        [Fact]
        public void EventQueue_RegisterTupleEvent_EventTriggered()
        {
            var evtQ = new EventQueue();
            var isEvtTrigger = false;

            evtQ.RegisterEvent<ValueTuple<string, int, double>>("test", p =>
            {
                Assert.Equal("val1", p.Data.Item1);
                Assert.Equal(1, p.Data.Item2);
                Assert.Equal(3.5, p.Data.Item3);

                isEvtTrigger = true;
            });

            Assert.False(isEvtTrigger);

            evtQ.TriggerEvents("test", ("val1", 1, 3.5));

            Assert.True(isEvtTrigger);
        }

        [Fact]
        public void EventQueue_RegisterExceptionEvent_EventExceptionLogged()
        {
            string logger = "";
            var evtQ = new EventQueue(log => logger += log);

            evtQ.RegisterEvent<object>("test", p =>
            {
                throw new InvalidOperationException("expected");
            });

            var ex = Assert.Throws<InvalidOperationException>(() => evtQ.TriggerEvents<object>("test", null));

            Assert.Equal("expected", ex.Message);
            Assert.StartsWith($"Event exception 'test' is thrown.", logger);
        }
    }
}
