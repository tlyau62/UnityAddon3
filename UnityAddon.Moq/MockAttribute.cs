using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Moq
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MockAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
