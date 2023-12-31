using Microsoft.CodeAnalysis;

namespace SecondOfficer.Generator.Diagnostics
{
    public static class DiagnosticDescriptors
    {

        //ERRORS
        public static DiagnosticDescriptor LanguageVersionNotSupported(DiagnosticCategories diagnosticCategories)
        {
            return new DiagnosticDescriptor(
                "SHIP000001",
                "The used C# language version is not supported by Second Officer, at least C# 12.0 is required",
                "Second Officer does not support the C# language version {0} but requires at C# least version {1}",
                diagnosticCategories.ToString(),
                DiagnosticSeverity.Error,
                true);
        }








        //INFO
    }
}
