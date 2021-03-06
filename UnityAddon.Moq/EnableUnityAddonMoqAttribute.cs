﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Moq;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Define the scope (base namespaces) for component scanner to scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class EnableUnityAddonMoqAttribute : ConfigArgAttribute
    {
        public EnableUnityAddonMoqAttribute(bool partial = false)
        {
            Args.Add(new object[] { UnityAddonTestFixture.CONFIG_PREFIX + "UnityAddonMoqConfig", typeof(UnityAddonMoqConfig), typeof(Type) });
            Args.Add(new object[] { "UAMoqPartial", partial, typeof(bool) });
        }
    }
}
