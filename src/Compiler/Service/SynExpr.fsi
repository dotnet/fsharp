namespace FSharp.Compiler.Syntax

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module public SynExpr =

    /// <summary>
    /// Returns true if the given expression should be parenthesized in the given context, otherwise false.
    /// </summary>
    /// <param name="getSourceLineStr">A function for getting the text of a given source line.</param>
    /// <param name="path">The expression's ancestor nodes.</param>
    /// <param name="expr">The expression to check.</param>
    /// <returns>True if the given expression should be parenthesized in the given context, otherwise false.</returns>
    val shouldBeParenthesizedInContext:
        getSourceLineStr: (int -> string) -> path: SyntaxVisitorPath -> expr: SynExpr -> bool
