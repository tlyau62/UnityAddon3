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
        private readonly IList<IComponentScannerStrategy> _scannerStrategies = new List<IComponentScannerStrategy>();

        public IEnumerable<IComponentScannerStrategy> ScannerStrategies => _scannerStrategies.OrderBy(stg => Ordered.GetOrder(stg.GetType())).ToArray();

        public ComponentScannerOption(bool useDefaultStrategy = true)
        {
            if (useDefaultStrategy)
            {
                AddComponentScannerStrategy<DefaultComponentScannerStrategy>();
                AddComponentScannerStrategy<ConfigurationScannerStrategy>();
            }
        }

        public void AddComponentScannerStrategy<T>() where T : IComponentScannerStrategy
        {
            _scannerStrategies.Add(Activator.CreateInstance<T>());
        }

        public void AddComponentScannerStrategy(IComponentScannerStrategy stg)
        {
            _scannerStrategies.Add(stg);
        }
    }
}
