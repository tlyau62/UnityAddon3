using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity;
using UnityAddon.Core.Attributes;

// TODO Hangfire should be moved to a new Addon plugin project, it depends on Addon.Ef
namespace UnityAddon.Hangfire
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class HangfireProxyAttribute : ComponentAttribute
    {
    }
}