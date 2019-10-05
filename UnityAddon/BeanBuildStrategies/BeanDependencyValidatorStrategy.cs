using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Attributes;

namespace UnityAddon.BeanBuildStrategies
{
    /// <summary>
    /// Check
    /// 1. circular dep
    /// 2. null dep
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

            // check null dep
            if (!context.Container.IsRegistered(context.RegistrationType, context.Name))
            {
                if (stack.Count == 0)
                {
                    throw new InvalidOperationException($"{context.RegistrationType} with name {context.Name} is not found.");
                }
                else
                {
                    throw new InvalidOperationException($"{context.RegistrationType} with name {context.Name} is not found in {stack.Peek()}.");
                }
            }

            var ctx = context;

            // check cirular dep
            if (stack.Any(ent => ent.ResolveType == ctx.RegistrationType && ent.ResolveName == ctx.Name))
            {
                stack.Push(new ResolveStackEntry(context.RegistrationType, context.Name));
                var ex = new InvalidOperationException("circular dep: " + string.Join("->", stack.Select(t => $"type {t.ResolveType}, name: {t.ResolveName}").ToArray()));

                StackFactory.Delete();

                throw ex;
            }

            stack.Push(new ResolveStackEntry(context.RegistrationType, context.Name, !stackExist));

            base.PreBuildUp(ref context);
        }
    }
}
