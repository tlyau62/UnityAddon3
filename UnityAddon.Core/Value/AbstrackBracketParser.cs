using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Value
{
    public abstract class AbstrackBracketParser
    {
        private BracketAccumulator _accumulator = new BracketAccumulator();

        public virtual string Parse(string inputs)
        {
            string result = "";

            foreach (var c in inputs)
            {
                if (c == '}')
                {
                    string temp = _accumulator.Consume();

                    temp = Process(temp);

                    if (_accumulator.IsEmpty())
                    {
                        result += temp;
                    }
                    else
                    {
                        _accumulator.Push(temp.ToCharArray());
                    }
                }
                else if (c == '{' || !_accumulator.IsEmpty())
                {
                    _accumulator.Push(c);
                }
                else
                {
                    result += c;
                }
            }

            return result;
        }

        protected abstract string Process(string intermediateResult);
    }


}
