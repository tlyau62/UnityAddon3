using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Reflection;
using Xunit;

namespace UnityAddon.CoreTest.Reflection
{
    [AttributeUsage(AttributeTargets.All)]
    public class GeneralParentAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All)]
    public class GeneralAttribute : GeneralParentAttribute { }

    [AttributeUsage(AttributeTargets.All)]
    public class General2Attribute : GeneralParentAttribute { }

    [AttributeUsage(AttributeTargets.All)]
    public class NonGeneralAttribute : Attribute { }

    [General]
    [General2]
    public class AttributeTestClass { }

    public class AttributeExtTests
    {
        [Fact]
        public void Type_HasAttribute()
        {
            Assert.True(typeof(AttributeTestClass).HasAttribute<GeneralAttribute>());
            Assert.True(typeof(AttributeTestClass).HasAttribute<GeneralParentAttribute>(true));

            Assert.False(typeof(AttributeTestClass).HasAttribute<NonGeneralAttribute>());
            Assert.False(typeof(AttributeTestClass).HasAttribute<GeneralParentAttribute>(false));
        }

        [Fact]
        public void Type_GetAttribute()
        {
            Assert.IsType<GeneralAttribute>(typeof(AttributeTestClass).GetAttribute<GeneralAttribute>());
            Assert.IsType<GeneralAttribute>(typeof(AttributeTestClass).GetAttribute<GeneralParentAttribute>(true));

            Assert.Null(typeof(AttributeTestClass).GetAttribute<NonGeneralAttribute>());
            Assert.Null(typeof(AttributeTestClass).GetAttribute<GeneralParentAttribute>(false));
        }

        [Fact]
        public void Type_GetAllAttributes()
        {
            Assert.Single(typeof(AttributeTestClass).GetAllAttributes<GeneralAttribute>());
            Assert.Equal(2, typeof(AttributeTestClass).GetAllAttributes<GeneralParentAttribute>(true).Count());

            Assert.Empty(typeof(AttributeTestClass).GetAllAttributes<NonGeneralAttribute>());
            Assert.Empty(typeof(AttributeTestClass).GetAllAttributes<GeneralParentAttribute>(false));
        }
    }
}
