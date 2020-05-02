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
        private readonly IThreadLocalFactory<Stack<ResolveStackEntry>> _stackFactory;

        public BeanDependencyValidatorStrategy()
        {
            _stackFactory = new ThreadLocalFactory<Stack<ResolveStackEntry>>(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>()));
        }

        /// <summary>
        /// Remove stack entry
        /// </summary>
        public override void PostBuildUp(ref BuilderContext context)
        {
            var stack = _stackFactory.Get();
            ResolveStackEntry entry;

            while (stack.Peek().ResolveType != context.Type || stack.Peek().ResolveName != context.Name)
            {
                stack.Pop();
            }
            entry = stack.Pop();

            if (entry != null && entry.IsBaseResolve)
            {
                _stackFactory.Delete();
            }

            base.PostBuildUp(ref context);
        }

        /// <summary>
        /// RegistrationType => container.RegisterType<RegistrationType, type>
        /// Type => used by container to build up the object. It searches the [Dependency] in Type and fill them up in the object
        /// </summary>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var stackExist = _stackFactory.Exist();
            Stack<ResolveStackEntry> stack = stackExist ? _stackFactory.Get() : _stackFactory.Set();
            var name = context.Name;
            var type = context.Type;

            // check cirular dep
            if (stack.Any(ent => ent.ResolveType == type && ent.ResolveName == name))
            {
                stack.Push(new ResolveStackEntry(type, name));
                var ex = new CircularDependencyException(string.Join("\r\n->", stack.Reverse().Select(t => $"type {t.ResolveType} (name: {t.ResolveName})").ToArray()));

                _stackFactory.Delete();

                throw ex;
            }

            stack.Push(new ResolveStackEntry(type, name, !stackExist));

            base.PreBuildUp(ref context);
        }
    }
}
