using Microsoft.CodeAnalysis;
using SecondOfficer.Generator.Enums;
using SecondOfficer.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondOfficer.Generator.Syntax
{
    internal class DeleteMethods
    {
        internal static string GenerateDeleteMethods(ITypeSymbol symbol, List<RestConfig> restConfigs)
        {
            if (restConfigs.Count == 0 || restConfigs.All(a => (a.Actions & Actions.Delete) != Actions.Delete))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"ILambdaModel Delete(long id);");
            return sb.ToString();

        }

    }
}
