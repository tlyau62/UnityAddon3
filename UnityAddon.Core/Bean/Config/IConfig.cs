using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Bean.Config
{
    public interface IConfigs<TConfigs> where TConfigs : class, new()
    {
        public TConfigs Value { get; }
    }
}
