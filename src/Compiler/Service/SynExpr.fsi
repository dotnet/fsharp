namespace FSharp.Compiler.Syntax

[<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module public SynExpr =

    /// Returns true if the given expression should be parenthesized in the given context, otherwise false.
    val shouldBeParenthesizedInContext:
        getSourceLineStr: (int -> string) -> path: SyntaxNode list -> expr: SynExpr -> bool
