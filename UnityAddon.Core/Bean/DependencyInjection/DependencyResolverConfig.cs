using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    public abstract class DependencyResolverConfig
    {
        [Bean]
        public abstract DependencyResolverOption DependencyResolverOption();
    }
}
