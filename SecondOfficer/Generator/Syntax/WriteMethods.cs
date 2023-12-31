using Microsoft.CodeAnalysis;
using SecondOfficer.Generator.Enums;
using SecondOfficer.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondOfficer.Generator.Syntax
{
    internal class WriteMethods
    {
        internal static string GenerateWriteMethods(ITypeSymbol symbol, List<RestConfig> restConfigs)
        {
            if (restConfigs.Count == 0 || restConfigs.All(a => (a.Actions & Actions.Write) != Actions.Write))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"ILambdaModel Save({symbol.ToDisplayString()} model);");
            return sb.ToString();

        }
    }
}
