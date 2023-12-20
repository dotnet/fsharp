namespace FSharp.Compiler.Syntax

[<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module public SynPat =

    /// Returns true if the given pattern should be parenthesized in the given context, otherwise false.
    val shouldBeParenthesizedInContext: path: SyntaxNode list -> pat: SynPat -> bool
