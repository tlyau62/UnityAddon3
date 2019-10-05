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
        public static object[] FillAllParamaters(MethodBase method, IUnityContainer container)
        {
            return method.GetParameters().Select(param =>
            {
                var depAttr = param.GetAttribute<DependencyAttribute>();
                var paramType = param.ParameterType;

                if (depAttr != null)
                {
                    return container.Resolve(param.ParameterType, depAttr.Name);
                }

                return container.Resolve(param.ParameterType, null);
            })
            .ToArray();
        }
    }
}
