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

    member ReplaceSourceSnapshot: sourceSnapshot: SourceSnapshot -> IncrementalChecker

    member CheckAsync: filePath: string -> Async<(TcAccumulator * TcResultsSinkImpl * SymbolEnv)>

    member GetSyntaxTree: filePath: string -> SyntaxTree

[<RequireQualifiedAccess>]
module internal IncrementalChecker =

    val create: TcConfig -> TcGlobals -> TcImports -> TcAccumulator -> CheckerOptions -> ImmutableArray<SourceSnapshot> -> Cancellable<IncrementalChecker>