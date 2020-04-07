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
namespace UnityAddon.Hangfire
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class HangfireProxyAttribute : Attribute
    {
        // TODO name should be set in [Qualifier], but that attr is only on class, not interface
        public string Name { get; set; }
    }
}