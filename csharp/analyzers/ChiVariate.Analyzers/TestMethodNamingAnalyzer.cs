using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ChiVariate.Analyzers;

/// <summary>
///     Flags test methods whose names do not follow the
///     [UnitOfWork]_[TestedScenario]_[ExpectedBehavior] convention
///     (exactly three PascalCase segments separated by underscores).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TestMethodNamingAnalyzer : DiagnosticAnalyzer
{
    private static readonly Regex NamingPattern = new(
        @"^[A-Z][a-zA-Z0-9]*_[A-Z][a-zA-Z0-9]*_[A-Z][a-zA-Z0-9]*$",
        RegexOptions.Compiled);

    private static readonly ImmutableHashSet<string> TestAttributes = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Fact", "FactAttribute",
        "Theory", "TheoryAttribute",
        "Test", "TestAttribute",
        "TestMethod", "TestMethodAttribute");

    private static readonly DiagnosticDescriptor NamingRule = new(
        "CV1001",
        "Test method naming convention",
        "Test method '{0}' must be named [UnitOfWork]_[TestedScenario]_[ExpectedBehavior]: " +
        "exactly 3 PascalCase segments, each starting with a capital letter; " +
        "UnitOfWork = the method or member under test, not the class; " +
        "TestedScenario = the meaningful condition, general not granular (do not enumerate params; " +
        "add a type only when it is the variation, written bare like 'UInt128'); " +
        "ExpectedBehavior = a concrete assertion that starts with a verb (Returns/Throws/Produces/Matches/Is) and names " +
        "the specific outcome, such as ReturnsZero or ThrowsArgumentException, never vague like 'IsCorrect' or 'Works'; " +
        "derive the name from what the test body does, not the old name",
        "Naming",
        DiagnosticSeverity.Error,
        true,
        "Test methods must follow the [UnitOfWork]_[TestedScenario]_[ExpectedBehavior] naming convention.");

    private static readonly ImmutableArray<DiagnosticDescriptor> Rules = ImmutableArray.Create(NamingRule);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Rules;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodSyntax = (MethodDeclarationSyntax)context.Node;

        if (methodSyntax.AttributeLists.Count == 0)
            return;

        var hasTestAttribute = methodSyntax.AttributeLists
            .SelectMany(static al => al.Attributes)
            .Any(attr => IsTestAttribute(attr, context.SemanticModel));

        if (!hasTestAttribute)
            return;

        var methodName = methodSyntax.Identifier.Text;
        if (NamingPattern.IsMatch(methodName))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            NamingRule,
            methodSyntax.Identifier.GetLocation(),
            methodName));
    }

    private static bool IsTestAttribute(AttributeSyntax attribute, SemanticModel semanticModel)
    {
        var name = attribute.Name switch
        {
            SimpleNameSyntax simple => simple.Identifier.Text,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
            _ => null
        };

        if (name is not null && TestAttributes.Contains(name))
            return true;

        var symbolInfo = semanticModel.GetSymbolInfo(attribute);
        if (symbolInfo.Symbol is IMethodSymbol { ContainingType: { } attrType })
        {
            var fullName = attrType.ToDisplayString();
            if (fullName.EndsWith("FactAttribute", StringComparison.Ordinal) ||
                fullName.EndsWith("TheoryAttribute", StringComparison.Ordinal) ||
                fullName.EndsWith("TestAttribute", StringComparison.Ordinal) ||
                fullName.EndsWith("TestMethodAttribute", StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}