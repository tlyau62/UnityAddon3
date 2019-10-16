using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using UnityAddon.Thread;

namespace UnityAddon.BeanBuildStrategies
{
    /// <summary>
    /// Check
    /// 1. circular dep
    /// 2. null dep (removed)
    /// </summary>
    [Component]
    public class BeanDependencyValidatorStrategy : BuilderStrategy
    {
        [Dependency]
        public IAsyncLocalFactory<Stack<ResolveStackEntry>> StackFactory { get; set; }

        /// <summary>
        /// Remove stack entry
        /// </summary>
        public override void PostBuildUp(ref BuilderContext context)
        {
            var stack = StackFactory.Get();
            var entry = stack.Pop();

            if (entry.IsBaseResolve)
            {
                StackFactory.Delete();
            }

            base.PostBuildUp(ref context);
        }

        /// <summary>
        /// RegistrationType => container.RegisterType<RegistrationType, type>
        /// Type => used by container to build up the object. It searches the [Dependency] in Type and fill them up in the object
        /// </summary>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var stackExist = StackFactory.Exist();
            Stack<ResolveStackEntry> stack = stackExist ? StackFactory.Get() : StackFactory.Set();
            var name = context.Name;
            //var type = context.RegistrationType.IsGenericType ?
            //        context.RegistrationType.GetGenericTypeDefinition() :
            //        context.RegistrationType;
            var type = context.Type;

            // check cirular dep
            if (stack.Any(ent => ent.ResolveType == type && ent.ResolveName == name))
            {
                stack.Push(new ResolveStackEntry(type, name));
                var ex = new CircularDependencyException(string.Join("\r\n->", stack.Select(t => $"type {t.ResolveType} (name: {t.ResolveName})").ToArray()));

                StackFactory.Delete();

                throw ex;
            }

            stack.Push(new ResolveStackEntry(type, name, !stackExist));

            base.PreBuildUp(ref context);
        }
    }
}
