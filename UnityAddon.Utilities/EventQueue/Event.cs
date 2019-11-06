using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Utilities.EventQueue
{
    public class Event<T>
    {
        public T Data { get; set; }
        public EventSource EventSource { get; set; }
    }
}
