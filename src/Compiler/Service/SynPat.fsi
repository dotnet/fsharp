namespace FSharp.Compiler.Syntax

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module public SynPat =

    /// <summary>
    /// Returns true if the given pattern should be parenthesized in the given context, otherwise false.
    /// </summary>
    /// <param name="path">The pattern's ancestor nodes.</param>
    /// <param name="pat">The pattern to check.</param>
    /// <returns>True if the given pattern should be parenthesized in the given context, otherwise false.</returns>
    val shouldBeParenthesizedInContext: path: SyntaxVisitorPath -> pat: SynPat -> bool
