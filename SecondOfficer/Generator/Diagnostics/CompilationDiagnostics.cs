using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SecondOfficer.Generator.Diagnostics
{
    public static class CompilationDiagnostics
    {
        public static IEnumerable<Diagnostic> BuildCompilationDiagnostics(Compilation compilationDiagnostics, DiagnosticCategories diagnosticCategories)
        {
            if (compilationDiagnostics is CSharpCompilation { LanguageVersion: < LanguageVersion.CSharp9 } cSharpCompilation)
            {
                yield return Diagnostic.Create(
                    DiagnosticDescriptors.LanguageVersionNotSupported(diagnosticCategories),
                    null,
                    cSharpCompilation.LanguageVersion.ToDisplayString(),
                    LanguageVersion.CSharp9.ToDisplayString()
                );
            }


        }
    }
}
