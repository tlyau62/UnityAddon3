using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Util.ComponentScanning
{
    public class ComponentScannerOption
    {
        public IList<IComponentScannerStrategy> ScannerStrategies { get; set; } = new List<IComponentScannerStrategy>();

        public IList<Func<Type, bool>> IncludeFilters { get; set; } = new List<Func<Type, bool>>();

        public IList<Func<Type, bool>> ExcludeFilters { get; set; } = new List<Func<Type, bool>>();

        public ComponentScannerOption(bool useDefaultStrategy = true)
        {
            if (useDefaultStrategy)
            {
                ScannerStrategies.Add(new DefaultComponentScannerStrategy());

                ExcludeFilters.Add(type => type.GetCustomAttribute<IgnoreDuringScanAttribute>() != null);
            }
        }
    }
}
