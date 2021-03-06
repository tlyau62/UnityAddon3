﻿using Castle.DynamicProxy;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.Hangfire
{
    [Component]
    [AopAttribute(typeof(HangfireProxyAttribute))]
    public class HangfireProxyInterceptor : IInterceptor
    {
        [OptionalDependency]
        public ITransactionCallbacks TransactionCallbacks { get; set; }

        [Dependency]
        public IBackgroundJobClient BackgroundJobClient { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var intrfcType = invocation.Method.DeclaringType;
            var paramExpr = Expression.Parameter(intrfcType);
            var argExprs = invocation.Arguments
                .Select(a => Expression.Constant(a));
            var callExpr = Expression.Call(paramExpr, invocation.Method,
                argExprs);
            var create = typeof(HangfireProxyInterceptor).GetMethod(nameof(CreateCallBack)).MakeGenericMethod(intrfcType);
            var lambda = Expression.Lambda(callExpr, paramExpr);

            Action callback = (Action)create.Invoke(this, new[] { lambda });

            if (TransactionCallbacks != null)
            {
                // background job often depends on the data of the enclosing transation, so enqueue after commit
                TransactionCallbacks.OnCommit(callback);
            }
            else
            {
                callback();
            }
        }

        public Action CreateCallBack<T>(Expression<Action<T>> expression)
        {
            return () => BackgroundJobClient.Enqueue(expression);
        }
    }
}
