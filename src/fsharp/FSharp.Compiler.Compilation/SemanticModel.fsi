namespace FSharp.Compiler.Compilation

open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type SemanticModel =

    internal new: filePath: string * AsyncLazy<IncrementalChecker> -> SemanticModel

    member TryFindSymbolAsync: line: int * column: int -> Async<FSharpSymbol option>