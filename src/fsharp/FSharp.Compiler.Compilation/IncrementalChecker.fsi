namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution

[<NoEquality; NoComparison>]
type internal CheckerParsingOptions =
    {
        isExecutable: bool
        isScript: bool
    }

[<NoEquality; NoComparison>]
type internal CheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: CheckerParsingOptions
    }

/// This is immutable.
/// Its job is to do the least amount of work to get a result.
[<Sealed>]
type internal IncrementalChecker =

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

[<RequireQualifiedAccess>]
module internal IncrementalChecker =

    val create: TcInitial -> TcGlobals -> TcImports -> TcAccumulator -> CheckerOptions -> ImmutableArray<FSharpSourceSnapshot> -> Cancellable<IncrementalChecker>