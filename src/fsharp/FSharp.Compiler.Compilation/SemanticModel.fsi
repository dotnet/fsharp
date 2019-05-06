namespace FSharp.Compiler.Compilation

open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type SemanticModel =

    internal new: AsyncLazy<IncrementalChecker> -> SemanticModel

    member TryFindSymbolAsync: line: int * column: int -> Async<FSharpSymbol option>