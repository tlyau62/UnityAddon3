using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class UnityAddonDefaultTest
    {
        public UnityAddonDefaultTest()
        {
            var f = new UnityAddonServiceProviderFactory();
            var defCol = f.CreateBuilder();

            defCol.AddFromComponentScanner(cs => cs.ScanAssembly(GetType().Assembly, GetType().Namespace));

            var a = f.CreateServiceProvider(defCol);

            a.BuildUp(this);
        }
    }
}
