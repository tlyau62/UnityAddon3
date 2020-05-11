using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ConfigArgAttribute : Attribute
    {
        private Type _type;

        public string Key { get; set; }

        public object Value { get; set; }

        public Type Type => _type ?? Value.GetType();

        public ConfigArgAttribute(string key, Type value) : this(key, value, typeof(Type))
        {
        }

        public ConfigArgAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public ConfigArgAttribute(string key, object value, Type type) : this(key, value)
        {
            _type = type;
        }
    }
}
