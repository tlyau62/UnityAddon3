using Castle.DynamicProxy;
using Hangfire;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;

// TODO Hangfire should be moved to a new Addon plugin project, it depends on Addon.Ef
namespace UnityAddon.Ef.Hangfire
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class HangfireProxyAttribute : Attribute
    {
        // TODO name should be set in [Qualifier], but that attr is only on class, not interface
        public string Name { get; set; }
    }

    [Component]
    public class HangfireProxyInterceptor : IInterceptor
    {
        [Dependency]
        public ITransactionCallbacks TransactionCallbacks { get; set; }

        private MethodInfo _enqueue;

        public HangfireProxyInterceptor()
        {
            /*
             * _enqueue set to
             * BackgroundJob {
             *   static string Enqueue<T>(Expression<Action<T>>)
             * }
             */
            _enqueue = typeof(BackgroundJob).GetMethods()
                    .Where(m =>
                    {
                        if (m.Name != "Enqueue" || !m.IsGenericMethodDefinition)
                            return false;
                        var p = m.GetParameters();
                        if (p.Length != 1)
                            return false;
                        var pt = p[0].ParameterType;
                        if (!pt.IsGenericType)
                            return false;
                        var pg = pt.GetGenericTypeDefinition();
                        if (pg != typeof(Expression<>))
                            return false;
                        var pgg = pt.GetGenericArguments()[0];
                        return pgg.IsGenericType && pgg.GetGenericTypeDefinition() == typeof(Action<>);
                    })
                    .First();
        }

        public void Intercept(IInvocation invocation)
        {
            var intrfcType = invocation.Method.DeclaringType;
            var paramExpr = Expression.Parameter(intrfcType);
            var argExprs = invocation.Arguments
                .Select(a => Expression.Constant(a));
            var callExpr = Expression.Call(paramExpr, invocation.Method,
                argExprs);
            var lambdaType = typeof(Action<>).MakeGenericType(intrfcType);
            var lambda = Expression.Lambda(lambdaType, callExpr, paramExpr);

            var enqueue = _enqueue.MakeGenericMethod(intrfcType);

            // background job often depends on the data of the enclosing transation, so enqueue after commit
            TransactionCallbacks.OnCommit(() =>
            {
                enqueue.Invoke(null, new object[] { lambda });
            });
        }
    }

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
