﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Moq;

namespace UnityAddon.Moq
{
    public abstract class AbstractUnityAddonMoqTest : UnityAddonTest
    {
        public AbstractUnityAddonMoqTest() : this(false)
        {
        }

        public AbstractUnityAddonMoqTest(bool partial) : base(Config(partial))
        {
        }

        private static Action<IHostBuilder, UnityAddonTest> Config(bool partial)
        {
            return (builder, test) =>
            {
                builder.EnableUnityAddonMoq(test, partial);
            };
        }
    }
}
