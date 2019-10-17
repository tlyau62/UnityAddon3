﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GuidBean
{
    interface IService { }

    [Component]
    class GeneralService
    {
        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }
    }

    [Component]
    [Guid("b1368cba-7614-4923-9426-8cd4456da29e")]
    class PrintService : IService
    {
    }

    [Component]
    [Guid("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
    class WriteService : IService
    {
    }

    [Trait("Bean", "GuidBean")]
    public class GuidBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnGuidBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var generalService = appContext.Resolve<GeneralService>();
            var printService = appContext.Resolve<IService>("b1368cba-7614-4923-9426-8cd4456da29e");
            var writeService = appContext.Resolve<IService>("4e55e61a-c57f-4b55-84dd-044d539dfbc7");

            Assert.Same(generalService.PrintService, printService);
            Assert.Same(generalService.WriteService, writeService);
        }
    }
}