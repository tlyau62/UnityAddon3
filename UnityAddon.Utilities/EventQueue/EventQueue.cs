using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityAddon.Utilities.EventQueue
{
    public interface IEventQueue
    {
        void RegisterEvent<T>(string evtName, Action<Event<T>> action,
            [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);

        void TriggerEvents<T>(string evtName, T data,
            [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
    }

    public class EventQueue : IEventQueue
    {
        private IDictionary<string, ConcurrentQueue<EventHandler>> evtDict = new ConcurrentDictionary<string, ConcurrentQueue<EventHandler>>();

        private Action<string> _logger;

        public EventQueue() { }

        public EventQueue(Action<string> logger)
        {
            ConfigureLogger(logger);
        }

        public void ConfigureLogger(Action<string> logger)
        {
            _logger = logger;
        }

        public void RegisterEvent<T>(string evtName, Action<Event<T>> action,
            [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!evtDict.ContainsKey(evtName))
            {
                evtDict[evtName] = new ConcurrentQueue<EventHandler>();
            }

            var evtHandler = new EventHandler<T>
            {
                Handler = action,
                EventSource = new EventSource(filePath, lineNumber, memberName)
            };

            evtDict[evtName].Enqueue(evtHandler);
        }

        public void TriggerEvents<T>(string evtName, T data, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            EventHandler<T> evtHandlerGeneric = null;
            var evt = new Event<T>
            {
                Data = data,
                EventSource = new EventSource(filePath, lineNumber, memberName)
            };

            try
            {
                foreach (var evtHandler in evtDict[evtName])
                {
                    evtHandlerGeneric = (EventHandler<T>)evtHandler;

                    evtHandlerGeneric.Handler(evt);
                }
            }
            catch (Exception)
            {
                _logger?.Invoke(GetDebugMsg(evtName, evtHandlerGeneric));

                throw;
            }
        }

        private string GetDebugMsg(string evtName, EventHandler targetEvtHandler)
        {
            var msg = "";
            var executedEvtHandlers = new List<EventHandler>();
            var targetEvtQueue = evtDict[evtName];

            foreach (var hander in targetEvtQueue)
            {
                if (hander == targetEvtHandler)
                {
                    break;
                }

                executedEvtHandlers.Add(hander);
            }

            msg += $"Event exception '{evtName}' is thrown.\r\n";
            msg += $"Event handler is registered at {targetEvtHandler.EventSource}.";

            if (executedEvtHandlers.Count() > 0)
            {
                msg += "\r\nExecuted event handlers:\r\n";
                msg += string.Join("\r\n", executedEvtHandlers.Select(eventHandler => "- " + eventHandler.EventSource));
            }

            return msg;
        }
    }
}