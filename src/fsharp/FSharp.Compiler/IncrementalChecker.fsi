module internal FSharp.Compiler.Compilation.IncrementalChecker

open System.Threading
open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution

[<NoEquality; NoComparison>]
type CheckerParsingOptions =
    {
        isExecutable: bool
        isScript: bool
    }

[<NoEquality; NoComparison>]
type CheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: CheckerParsingOptions
    }

/// This is immutable.
/// Its job is to do the least amount of work to get a result.
[<Sealed>]
type IncrementalChecker =

    member ReplaceSourceSnapshot: sourceSnapshot: FSharpSourceSnapshot -> IncrementalChecker

    member CheckAsync: filePath: string -> Async<(TcAccumulator * TcResultsSinkImpl * SymbolEnv)>

    member SpeculativeCheckAsync: filePath: string * TcState * Ast.SynExpr -> Async<(Tast.TType * TcResultsSinkImpl) option>

    member GetSyntaxTree: filePath: string -> FSharpSyntaxTree

    member TcInitial: TcInitial

    member TcGlobals: TcGlobals

    member TcImports: TcImports

    /// Finishes checking everything.
    /// Once finished, the results will be cached.
    member FinishAsync: unit -> Async<TcAccumulator []>

    member SubmitSourceSnapshot: FSharpSourceSnapshot * CancellationToken -> IncrementalChecker

    static member Create: TcInitial * TcGlobals * TcImports * TcAccumulator * CheckerOptions * ImmutableArray<FSharpSourceSnapshot> -> Cancellable<IncrementalChecker>
