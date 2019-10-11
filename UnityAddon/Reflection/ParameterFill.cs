using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Reflection
{
    public static class ParameterFill
    {
        public static object[] FillAllParamaters(MethodBase method, IContainerRegistry containerRegistry)
        {
            return method.GetParameters().Select(param =>
            {
                var depAttr = param.GetAttribute<DependencyAttribute>();
                var optDepAttr = param.GetAttribute<OptionalDependencyAttribute>();
                var paramType = param.ParameterType;

                if (depAttr != null)
                {
                    return containerRegistry.Resolve(param.ParameterType, depAttr.Name);
                }
                else if (optDepAttr != null)
                {
                    return containerRegistry.IsRegistered(param.ParameterType, optDepAttr.Name) ?
                        containerRegistry.Resolve(param.ParameterType, optDepAttr.Name) : null;
                }

                return containerRegistry.Resolve(param.ParameterType, null);
            })
            .ToArray();
        }
    }
}
