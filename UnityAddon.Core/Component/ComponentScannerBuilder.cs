using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Component
{
    public class ComponentScannerBuilder
    {
        private IList<Type> _scannerStrategies = new List<Type>();

        public ComponentScannerBuilder()
        {
            AddComponentScannerStrategy<DefaultComponentScannerStrategy>();
            AddComponentScannerStrategy<ConfigurationScannerStrategy>();
        }

        public void AddComponentScannerStrategy<T>() where T : IComponentScannerStrategy
        {
            _scannerStrategies.Add(typeof(T));
        }

        public ComponentScanner Build(IServiceProvider sp)
        {
            return new ComponentScanner(
                _scannerStrategies.OrderBy(stg => Ordered.GetOrder(stg))
                    .Select(stg => (IComponentScannerStrategy)sp.BuildUp(Activator.CreateInstance(stg))));
        }
    }
}
