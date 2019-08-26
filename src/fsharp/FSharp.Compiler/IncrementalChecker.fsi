module internal FSharp.Compiler.Compilation.IncrementalChecker

open System.Threading
open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.Tast

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

[<NoEquality;NoComparison>]
type PreEmitState =
    {
        tcErrors: FSharpErrorInfo []
        tcEnvAtEndOfLastFile: TcEnv
        topAttribs: TopAttribs
        implFiles: TypedImplFile list
        tcState: TcState
    }

/// This is immutable.
/// Its job is to do the least amount of work to get a result.
[<Sealed>]
type IncrementalChecker =

    member ReplaceSource: oldSrc: FSharpSource * newSrc: FSharpSource -> IncrementalChecker

    member CheckAsync: FSharpSource -> Async<(TcAccumulator * TcResultsSinkImpl * SymbolEnv)>

    member SpeculativeCheckAsync: FSharpSource * TcState * Ast.SynExpr -> Async<(Tast.TType * TcResultsSinkImpl) option>

    member GetSyntaxTree: FSharpSource -> FSharpSyntaxTree

    member TcInitial: TcInitial

    member TcGlobals: TcGlobals

    member TcImports: TcImports

    /// Finishes checking everything.
    /// Once finished, the results will be cached.
    /// Returns the state used prior to emitting an assembly.
    member FinishAsync: unit -> Async<PreEmitState>

    member SubmitSource: FSharpSource * CancellationToken -> IncrementalChecker

    static member Create: TcInitial * TcGlobals * TcImports * TcAccumulator * CheckerOptions * ImmutableArray<FSharpSource> -> Cancellable<IncrementalChecker>
