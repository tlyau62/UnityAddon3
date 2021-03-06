﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Value
{
    /// <summary>
    /// Convert a value expression to the given value type.
    /// The main for value injection.
    /// 
    /// see also:
    /// https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible?view=netcore-3.0
    /// </summary>
    [Component]
    public class ValueProvider
    {
        [Dependency]
        public ConfigBracketParser ConfigBracketParser { get; set; }

        public object GetValue(Type valType, string valExpr)
        {
            var parsed = ConfigBracketParser.Parse(valExpr);

            if (valType.IsEnum)
            {
                return Enum.Parse(valType, parsed);
            }
            else if (typeof(IConvertible).IsAssignableFrom(valType))
            {
                return Convert.ChangeType(parsed, valType);
            }

            throw new NotImplementedException();
        }
    }

}
