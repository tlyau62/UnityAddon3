using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Value
{
    [Component]
    public class ValueProvider
    {
        [Dependency]
        public ConfigBracketParser ConfigBracketParser { get; set; }

        public object GetValue(Type valType, string valExpr)
        {
            var parsed = ConfigBracketParser.Parse(valExpr);

            if (typeof(Enum).IsAssignableFrom(valType))
            {
                return Enum.Parse(valType, parsed);
            }
            else if (valType.IsPrimitive || valType == typeof(string))
            {
                return Convert.ChangeType(parsed, valType);
            }

            throw new NotImplementedException();
        }
    }

}
