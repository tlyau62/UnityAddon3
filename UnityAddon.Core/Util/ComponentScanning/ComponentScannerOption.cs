using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Util.ComponentScanning
{
    public class ComponentScannerOption
    {
        public IList<IComponentScannerStrategy> ScannerStrategies { get; private set; } = new List<IComponentScannerStrategy>();

        public IList<Func<Type, bool>> IncludeFilters { get; private set; } = new List<Func<Type, bool>>();

        public IList<Func<Type, bool>> ExcludeFilters { get; private set; } = new List<Func<Type, bool>>();

        public ComponentScannerOption(bool useDefaultStrategy = true)
        {
            if (useDefaultStrategy)
            {
                ScannerStrategies.Add(new DefaultComponentScannerStrategy());
                ScannerStrategies.Add(new ConfigurationScannerStrategy());
            }
        }
    }
}
