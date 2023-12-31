using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Enums;
using SecondOfficer.Generator.Models;

namespace SecondOfficer.Generator.Syntax
{
    internal static class ReadMethods
    {
        internal static string GenerateReadMethods(ITypeSymbol symbol, List<RestConfig> restConfigs)
        {
            if (restConfigs.Count == 0 || restConfigs.All(a => (a.Actions & Actions.Read) != Actions.Read))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"ILambdaModel GetById(long id);");
            sb.AppendLine($"List<{symbol.ToDisplayString()}> GetAll();");

            return sb.ToString();
        }
    }
}
