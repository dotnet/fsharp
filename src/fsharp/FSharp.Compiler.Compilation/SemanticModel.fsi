namespace FSharp.Compiler.Compilation

open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type SemanticModel =

    internal new: filePath: string * AsyncLazy<IncrementalChecker> -> SemanticModel

    member TryFindSymbolAsync: line: int * column: int -> Async<FSharpSymbol option>

    member GetToolTipTextAsync: line: int * column: int -> Async<FSharp.Compiler.SourceCodeServices.FSharpToolTipText<FSharp.Compiler.SourceCodeServices.Layout> option>

    member GetCompletionSymbolsAsync: line: int * column: int -> Async<FSharpSymbol list>