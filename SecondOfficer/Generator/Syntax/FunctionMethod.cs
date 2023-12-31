using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using SecondOfficer.Generator.Enums;
using SecondOfficer.Generator.Models;

namespace SecondOfficer.Generator.Syntax
{
    internal static class FunctionMethod
    {
        internal static string GetFunctionServiceSection(ITypeSymbol symbol, List<RestConfig> restConfigs)
        {

            if (!restConfigs.Any())
            {
                return string.Empty;
            }

            var name = symbol.Name.First().ToString().ToLowerInvariant() + symbol.Name.Substring(1);

            var sb = new StringBuilder($@"
                    if(serviceType == ""{name}"")
                    {{
                    ");

            foreach (var restConfig in restConfigs)
            {
                var roles = restConfig.Roles.Split(',').Select(a => $"\"{a}\"").ToArray();
                sb.AppendLine($@"
                    if (_userService.IsInRoles(new string[] {{{string.Join(",", roles)}}}))
                    {{");

                if ((restConfig.Actions & Actions.Read) == Actions.Read)
                {
                    sb.AppendLine($"if(method == \"GetAll\") models = _{name}Service.GetAll();");
                    sb.AppendLine($"if(method == \"GetById\") model = _{name}Service.GetById(id);");
                }
                if ((restConfig.Actions & Actions.Write) == Actions.Write)
                {
                    sb.AppendLine($"if(method == \"Save\") model = _{name}Service.Save(_lambdaSerializer.Deserialize<{symbol.ToDisplayString()}>(inputStream));");
                }
                if ((restConfig.Actions & Actions.Delete) == Actions.Delete)
                {
                    sb.AppendLine($"if(method == \"Delete\") model = _{name}Service.Delete(id);");
                }
                sb.AppendLine("}");
                
            }

            sb.AppendLine("}");
            return sb.ToString();

        }


    }
}
