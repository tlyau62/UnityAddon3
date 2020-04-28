using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Thread;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Check for circular dep.
    /// </summary>
    public class BeanDependencyValidatorStrategy : BuilderStrategy
    {
        [Dependency]
        public IThreadLocalFactory<Stack<ResolveStackEntry>> StackFactory { get; set; }

        /// <summary>
        /// Remove stack entry
        /// </summary>
        public override void PostBuildUp(ref BuilderContext context)
        {
            var stack = StackFactory.Get();
            ResolveStackEntry entry;

            while (stack.Peek().ResolveType != context.Type || stack.Peek().ResolveName != context.Name)
            {
                stack.Pop();
            }
            entry = stack.Pop();

            if (entry != null && entry.IsBaseResolve)
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
            var type = context.Type;

            // check cirular dep
            if (stack.Any(ent => ent.ResolveType == type && ent.ResolveName == name))
            {
                stack.Push(new ResolveStackEntry(type, name));
                var ex = new CircularDependencyException(string.Join("\r\n->", stack.Reverse().Select(t => $"type {t.ResolveType} (name: {t.ResolveName})").ToArray()));

                StackFactory.Delete();

                throw ex;
            }

            stack.Push(new ResolveStackEntry(type, name, !stackExist));

            base.PreBuildUp(ref context);
        }
    }
}
