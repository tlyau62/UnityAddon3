using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace UnityAddon.Serilog
{
    public class SerilogServiceCollectionTrap : IHostBuilder
    {
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        private IServiceCollection _serviceCollection;

        private HostBuilderContext _hostBuilderContext;

        public SerilogServiceCollectionTrap(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            _hostBuilderContext = hostBuilderContext;
            _serviceCollection = serviceCollection;
        }

        public IHost Build()
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            configureDelegate(_hostBuilderContext, _serviceCollection);

            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            throw new NotImplementedException();
        }
    }
}
