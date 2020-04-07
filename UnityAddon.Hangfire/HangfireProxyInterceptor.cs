using Castle.DynamicProxy;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace UnityAddon.Hangfire
{
    [Component]
    public class HangfireProxyInterceptor : IInterceptor
    {
        [OptionalDependency]
        public ITransactionCallbacks TransactionCallbacks { get; set; }

        private MethodInfo _enqueue;

        public HangfireProxyInterceptor()
        {
            /*
             * _enqueue set to
             * BackgroundJob {
             *   static string Enqueue<T>(Expression<Action<T>>)
             * }
             * TODO use IBackgroundJobClient for unit test
             * https://docs.hangfire.io/en/latest/background-methods/writing-unit-tests.html
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
            Action callback = () => enqueue.Invoke(null, new object[] { lambda });

            if (TransactionCallbacks != null)
            {
                // background job often depends on the data of the enclosing transation, so enqueue after commit
                TransactionCallbacks.OnCommit(callback);
            }
            else
                callback();
        }
    }
}
