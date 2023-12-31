using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SecondOfficer.Generator.Attributes;
using SecondOfficer.Generator.Diagnostics;
using SecondOfficer.Generator.Enums;
using SecondOfficer.Generator.Helpers;
using SecondOfficer.Generator.Models;
using SecondOfficer.Generator.Syntax;

namespace SecondOfficer.Generator
{
    [Generator]
    public class RestGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            //  DebugGenerator.AttachDebugger();
#endif

            var diagnostic = context.CompilationProvider.SelectMany(
                static (compilation, _) => CompilationDiagnostics.BuildCompilationDiagnostics(compilation,
                    DiagnosticCategories.Rest)
            );
            context.RegisterSourceOutput(diagnostic, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

            var classProvider =
                context.SyntaxProvider.CreateSyntaxProvider(
                        IsClassDeclarationSyntax,
                        (syntaxContext, _) => (ClassDeclarationSyntax)syntaxContext.Node)
                    .Where(a => a is not null);

            var compilation = context.CompilationProvider.Combine(classProvider.Collect());

            context.RegisterSourceOutput(compilation,
                static (spc, source) =>
                    CreateSource(spc, source.Left, source.Right));

        }

        private static void CreateSource(SourceProductionContext spc, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classDeclarationSyntaxes)
        {
            var fields = new StringBuilder();
            var arguments = new List<string>();
            var initArguments = new StringBuilder();
            var sbFunctionFileParts = new StringBuilder();
            var jsonContextStringBuilder = new StringBuilder();
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                var model = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(classDeclarationSyntax) is not ITypeSymbol classSymbol
                    || !SyntaxHelper.IsRestConfigLambdaModel(classSymbol))
                {
                    continue;
                }


                var restConfigs = GetRestConfigs(classSymbol);
                var sbServiceFile = new StringBuilder(ServiceFilePrefix(classSymbol.Name));
                sbServiceFile.AppendLine(ReadMethods.GenerateReadMethods(classSymbol, restConfigs));
                sbServiceFile.AppendLine(WriteMethods.GenerateWriteMethods(classSymbol, restConfigs));
                sbServiceFile.AppendLine(DeleteMethods.GenerateDeleteMethods(classSymbol, restConfigs));
                sbServiceFile.AppendLine(ServiceFileSuffix(classSymbol.Name));
                //write to source
                spc.AddSource($"I{classSymbol.Name}Service.g.cs", sbServiceFile.ToString());

                //function file
                arguments.Add($"I{classSymbol.Name}Service {classSymbol.Name.First().ToString().ToLowerInvariant()}{classSymbol.Name.Substring(1)}Service");
                initArguments.AppendLine($"_{classSymbol.Name.First().ToString().ToLowerInvariant()}{classSymbol.Name.Substring(1)}Service = {classSymbol.Name.First().ToString().ToLowerInvariant()}{classSymbol.Name.Substring(1)}Service;");
                fields.AppendLine($"private readonly I{classSymbol.Name}Service _{classSymbol.Name.First().ToString().ToLowerInvariant()}{classSymbol.Name.Substring(1)}Service;");
                sbFunctionFileParts.AppendLine(FunctionMethod.GetFunctionServiceSection(classSymbol, restConfigs));

                //json context
                jsonContextStringBuilder.AppendLine($"[JsonSerializable(typeof({classSymbol.ToDisplayString()}))]");
            }

            var sbFunctionFile = new StringBuilder(FunctionFilePrefix(fields.ToString(), initArguments.ToString(), arguments.ToArray()));
            sbFunctionFile.AppendLine(sbFunctionFileParts.ToString());
            sbFunctionFile.AppendLine(FunctionFileSuffix());

            spc.AddSource("Function.g.cs", sbFunctionFile.ToString());

            //write json context
            spc.AddSource("JsonContext.g.cs", GetJsonContext(jsonContextStringBuilder.ToString()));

        }

        private bool IsClassDeclarationSyntax(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            return syntaxNode is ClassDeclarationSyntax && !SyntaxHelper.IsAbstract(syntaxNode);
        }


        private static string ServiceFilePrefix(string className)
        {
            return $@"
                using SecondOfficer.Lambda.Contracts;
                using Amazon.Lambda.Core;
                namespace SecondOfficer{{
                public partial interface I{className}Service {{                                        
                    
         ";
        }

        private static string ServiceFileSuffix(string classSymbolName)
        {
            return $@"
                }}

}}";
        }

        private static string FunctionFilePrefix(string fields, string initArguments, string[] arguments)
        {
            return $@"using Amazon.Lambda.APIGatewayEvents;
                        using Amazon.Lambda.Core;
                    using SecondOfficer.Lambda.Contracts;
                        using System.Collections;
                    using System.Text;

                        namespace SecondOfficer
                        {{
                            public class Function
                            {{
                                private readonly IUserService _userService;
                                private readonly ILambdaSerializer _lambdaSerializer;
                                {fields}

                                public Function(ILambdaSerializer lambdaSerializer, IUserService userService, {string.Join(",",arguments)})
                                {{
                                    _lambdaSerializer = lambdaSerializer;
                                    _userService = userService;
                                    {initArguments}             
                                }}

                                public virtual APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
                                {{
                                    var verb = request.HttpMethod.ToUpperInvariant();
                                    var pathParts = request.Path.Split('/');
                                    var method = string.Empty;
                                    var serviceType = pathParts[3];
                                    var body = string.Empty; 
                                    ILambdaModel model = null;
                                    IList models = null;
                                    Int64.TryParse(pathParts[4], out var id);
                                    
                                    using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(request.Body)))
                                    {{
                                    
                                    switch (verb)
                                    {{
                                        //id is present
                                        case ""GET"" when (pathParts.Length == 5 && !string.IsNullOrEmpty(pathParts.Last())):
                                            method = ""GetById"";
                                            break;
                                        case ""GET"":
                                            method = ""GetAll"";
                                            break;
                                        case ""POST"":
                                        case ""PUT"":
                                            method = ""Save"";
                                            break;
                                        case ""DELETE"":
                                            method = ""Delete"";
                                            break;
                                    }}


                                    ";

        }

        private static string FunctionFileSuffix()
        {
            return $@"

                              if(model is not null && models is null)
                              {{
                                    return new APIGatewayProxyResponse
                                    {{
                                        StatusCode = 404,
                                        Body = ""Method not allowed""
                                    }};
                              }}
                                if(models is not null)
                                {{

                                     using (var memoryStream = new MemoryStream())
                                        {{
                                            _lambdaSerializer.Serialize(models, memoryStream);                                            
                                            memoryStream.Position = 0; 
                                            string jsonString = new StreamReader(memoryStream).ReadToEnd();

                                    return new APIGatewayProxyResponse
                                    {{
                                        StatusCode = 200,
                                        Body = ""{{jsonString}}""
                                    }};

                                     }}


                                    
                                
                                }}
                                 using (var memoryStream = new MemoryStream())
                                        {{
                                            _lambdaSerializer.Serialize(model, memoryStream);                                            
                                            memoryStream.Position = 0; 
                                            string jsonString = new StreamReader(memoryStream).ReadToEnd();

                                    return new APIGatewayProxyResponse
                                    {{
                                        StatusCode = 200,
                                        Body = ""{{jsonString}}""
                                    }};

                                     }}
                            }}
                        }}

                    }}
                }}
                ";


        }

        private static string GetJsonContext(string typeAttributes)
        {
            return $@"

using System.Text.Json.Serialization;
namespace SecondOfficer{{
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
{typeAttributes}
public partial class JsonContext : JsonSerializerContext {{ }}
}}
";



        }

        private static List<RestConfig> GetRestConfigs(ITypeSymbol symbol)
        {
            var restConfigs = new List<RestConfig>();

            // get class attributes
            var attributes = symbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass?.ToDisplayString() == typeof(RestConfigAttribute).FullName)
                {
                    var restConfig = new RestConfig
                    {
                        Roles = attribute.ConstructorArguments.Last().Value?.ToString() ?? string.Empty,
                        Actions = (Actions)Enum.Parse(typeof(Actions), attribute.ConstructorArguments.First().Value?.ToString() ?? string.Empty)
                    };

                    restConfigs.Add(restConfig);
                }
            }

            return restConfigs;
            
        }
    }
}
