using System;
using System.Reflection;

namespace UnityAddon.Hangfire
{
    [Component]
    public class HangfireProxyBuilder
    {
        [Dependency]
        public HangfireProxyInterceptor HangfireProxyInterceptor { get; set; }

        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        // TODO to be called in component scan
        public void Scan(Assembly asm)
        {
            asm.GetTypes()
                .Where(t => t.IsInterface && t.GetCustomAttribute<HangfireProxyAttribute>() != null)
                .Select(t => new
                {
                    Type = t,
                    // call to this proxy enqueue a background job
                    // when the app is the Hangfire client while the Hangfire server is remote, the interface t may not have impl in this app
                    Bean = ProxyGenerator.CreateInterfaceProxyWithoutTarget(t, HangfireProxyInterceptor),
                    Name = t.GetCustomAttribute<HangfireProxyAttribute>().Name
                })
                .ToList()
                .ForEach(b =>
                {
                    if (b.Name != null)
                        // TODO ApplicationContext.RegisterInstance must be generic, not taking Type param... reflection unfriendly
                        UnityContainer.RegisterInstance(b.Type, b.Name, b.Bean);
                    else
                        UnityContainer.RegisterInstance(b.Type, b.Bean);
                });
        }
    }
}
