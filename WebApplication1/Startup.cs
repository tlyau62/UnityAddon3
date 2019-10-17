using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unity;
using Unity.Microsoft.DependencyInjection;
using UnityAddon.Core;
using Unity.Lifetime;
using ServiceProvider = Unity.Microsoft.DependencyInjection.ServiceProvider;
using UnityAddon.Ef.Transaction;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static IServiceCollection Services { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;

            services.AddControllers().AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            //var factory = new ServiceProviderFactory(container);
            //var serviceProvider = factory.CreateServiceProvider(container);
            //container.RegisterInstance(serviceProvider, new ContainerControlledLifetimeManager());

            var serviceProvider = ServiceProvider.ConfigureServices(Services) as ServiceProvider;

            container.RegisterInstance<IServiceProvider>(serviceProvider);

            var appContext = new ApplicationContext(container, typeof(Program).Namespace);

            var z = appContext.IsRegistered(typeof(IDbContextTemplate<TestDbContext>));
        }
    }
}
