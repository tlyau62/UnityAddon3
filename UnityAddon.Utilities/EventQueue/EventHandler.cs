using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.EventQueue
{
    public class EventHandler
    {
        public EventSource EventSource { get; set; }
    }

    public class EventHandler<T> : EventHandler
    {
        public Action<Event<T>> Handler { get; set; }
    }
}
