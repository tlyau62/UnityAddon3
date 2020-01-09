using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanPostprocessor
{
    public interface IBeanPostProcessor
    {
        /// <summary>
        /// Executed after all internal bean build strategy has executed.
        /// </summary>
        void PostProcess(Object bean, string beanName);
    }
}
