using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Aop
{
    public abstract class AopInterceptorConfig
    {
        [Bean]
        public abstract AopInterceptorOption AopInterceptorOption();
    }
}
