using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SecondOfficer.Generator.Attributes;

namespace SecondOfficer.Generator.Helpers
{
    internal static class SyntaxHelper
    {
        internal static bool IsLambdaModel(ISymbol node)
        {
            return node is ITypeSymbol symbol && IsTypeOrImplementsInterface(symbol, "ILambdaModel");
        }

        internal static bool IsRestConfigLambdaModel(ISymbol node)
        {
            if (!IsLambdaModel(node))
            {
                return false;
            }
            // get class attributes
            var attributes = node.GetAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass?.ToDisplayString() == typeof(RestConfigAttribute).FullName)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsTypeOrImplementsInterface(ITypeSymbol? typeSymbol, string targetType)
        {
            if (typeSymbol == null)
            {
                return false;
            }

            if (typeSymbol.ToDisplayString().Split('<').First().EndsWith(targetType))
            {
                return true;
            }

            foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
            {
                if (interfaceSymbol.ToDisplayString().Split('<').First().EndsWith(targetType))
                {
                    return true;
                }
            }

            return false;
        }
        internal static bool IsAbstract(SyntaxNode node)
        {
            switch (node)
            {
                case ClassDeclarationSyntax classDecl:
                    return classDecl.Modifiers.Any(SyntaxKind.AbstractKeyword);

                case MethodDeclarationSyntax methodDecl:
                    return methodDecl.Modifiers.Any(SyntaxKind.AbstractKeyword);

                default:
                    return false;
            }
        }

    }
}
