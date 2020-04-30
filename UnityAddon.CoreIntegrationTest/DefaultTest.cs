using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class DefaultTest
    {
        public DefaultTest(params string[] namespaces)
        {
            var f = new UnityAddonServiceProviderFactory();
            var defCol = f.CreateBuilder();

            defCol.AddFromComponentScanner(cs => cs.ScanAssembly(GetType().Assembly, namespaces.Length == 0 ? new[] { GetType().Namespace } : namespaces));

            var a = f.CreateServiceProvider(defCol);

            a.BuildUp(this);
        }
    }
}
