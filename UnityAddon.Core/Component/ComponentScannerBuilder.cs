using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Component
{
    public class ComponentScannerBuilder
    {
        private IList<IComponentScannerStrategy> _scannerStrategies = new List<IComponentScannerStrategy>();

        public ComponentScannerBuilder()
        {
            AddComponentScannerStrategy(new DefaultComponentScannerStrategy());
        }

        public void AddComponentScannerStrategy(IComponentScannerStrategy componentScannerStrategy)
        {
            _scannerStrategies.Add(componentScannerStrategy);
        }

        public ComponentScanner Build()
        {
            return new ComponentScanner(_scannerStrategies.OrderBy(stg => Ordered.GetOrder(stg.GetType())));
        }
    }
}
