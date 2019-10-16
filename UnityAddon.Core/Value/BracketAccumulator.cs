using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Value
{
    /// <summary>
    /// Used by AbstrackBracketParser in parsing brackets.
    /// </summary>
    public class BracketAccumulator
    {
        private Stack<char> _stack = new Stack<char>();

        public string Consume()
        {
            string result = "";

            while (_stack.Count > 0 && _stack.Peek() != '{')
            {
                result = _stack.Pop() + result;
            }

            if (_stack.Count > 0)
            {
                _stack.Pop();
            }

            return result;
        }

        public void Push(params char[] els)
        {
            foreach (var el in els)
            {
                _stack.Push(el);
            }
        }

        public bool IsEmpty()
        {
            return _stack.Count == 0;
        }
    }
}
