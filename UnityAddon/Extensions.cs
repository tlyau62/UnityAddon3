using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace UnityAddon
{
    [Component]
    public class BeanUnityExtension : UnityContainerExtension
    {
        [Dependency]
        public BeanTypeMappingStrategy BeanTypeMappingStrategy { get; set; }

        [Dependency]
        public BeanDependencyValidatorStrategy BeanDependencyValidatorStrategy { get; set; }

        [Dependency]
        public BeanPostConstructStrategy BeanPostConstructStrategy { get; set; }

        protected override void Initialize()
        {
            Context.Strategies.Add(BeanTypeMappingStrategy, UnityBuildStage.TypeMapping);
            Context.Strategies.Add(BeanDependencyValidatorStrategy, UnityBuildStage.PreCreation);
            Context.Strategies.Add(BeanPostConstructStrategy, UnityBuildStage.PostInitialization);
        }
    }

    /// <summary>
    /// Map supertype to implementation type
    /// </summary>
    [Component]
    public class BeanTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (BeanDefinitionContainer.HasBeanDefinition(context.Type) && (context.Name == null || (context.Name != null && !context.Name.StartsWith("#")))) // bad
            {
                context.RegistrationType = context.Type = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name).GetBeanType(); // redirect to factory with unity cache
                context.Name = null; // factory has no name
            }

            base.PreBuildUp(ref context);
        }
    }

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
        /// <param name="context"></param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            var stackExist = StackFactory.Exist();
            Stack<ResolveStackEntry> stack = stackExist ? StackFactory.Get() : StackFactory.Set();

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

    /// <summary>
    /// Invoke postconstruct
    /// </summary>
    [Component]
    public class BeanPostConstructStrategy : BuilderStrategy
    {
        public override void PostBuildUp(ref BuilderContext context)
        {
            var postConstructors = MethodSelector.GetAllMethodsByAttribute<PostConstructAttribute>(context.Type);

            foreach (var pc in postConstructors)
            {
                if (pc.GetParameters().Length > 0 || pc.ReturnType != typeof(void))
                {
                    throw new InvalidOperationException("no-arg, void");
                }

                pc.Invoke(context.Existing, new object[0]);
            }

            base.PostBuildUp(ref context);
        }
    }

    public class ResolveStackEntry
    {
        public Type ResolveType { get; set; }
        public string ResolveName { get; set; }
        public bool IsBaseResolve { get; set; } // true: the 1st call to container.resolve

        public ResolveStackEntry(Type resolveType, string resolveName, bool isBaseResolve = false)
        {
            ResolveType = resolveType;
            ResolveName = resolveName;
            IsBaseResolve = isBaseResolve;
        }
    }
}
