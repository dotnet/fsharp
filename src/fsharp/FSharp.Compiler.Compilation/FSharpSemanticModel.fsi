namespace FSharp.Compiler.Compilation

open System.Threading
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type FSharpSymbol = 

    member internal InternalSymbolUse: InternalFSharpSymbolUse

[<Sealed>]
type FSharpSymbolInfo =

    /// The symbol that was referred to by the syntax node, if any. If None is returned, it may
    /// still be that case that we have one or more "best guesses" as to what symbol was
    /// intended. These best guesses are available via the CandidateSymbols property.
    member Symbol: FSharpSymbol option

    member CandidateSymbols: ImmutableArray<FSharpSymbol>

    member GetAllSymbols: unit -> ImmutableArray<FSharpSymbol>

[<Sealed>]
type FSharpSemanticModel =

    internal new: filePath: string * AsyncLazy<IncrementalChecker> * compilationObj: obj -> FSharpSemanticModel

    member internal GetToolTipTextAsync: line: int * column: int -> Async<FSharp.Compiler.SourceCodeServices.FSharpToolTipText<FSharp.Compiler.SourceCodeServices.Layout> option>

    member internal GetCompletionSymbolsAsync: line: int * column: int -> Async<InternalFSharpSymbol list>

    member GetSymbolInfo: node: FSharpSyntaxNode * ct: CancellationToken -> FSharpSymbolInfo

    member TryGetEnclosingSymbol: position: int * ct: CancellationToken -> FSharpSymbol option

    member GetSpeculativeSymbolInfo: position: int * node: FSharpSyntaxNode * ct: CancellationToken -> FSharpSymbolInfo

    member SyntaxTree: FSharpSyntaxTree

    member internal CompilationObj: obj

    member GetSyntaxDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    member GetSemanticDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    member GetDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>