namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.CompileOps
open FSharp.Compiler

[<Sealed>]
type SourceSnapshot =

    member FilePath: string

[<NoEquality; NoComparison>]
type internal ParsingConfig =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<Sealed;AbstractClass;Extension>]
type internal ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string * SourceText -> Cancellable<SourceSnapshot>

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string -> Cancellable<SourceSnapshot>

[<Sealed>]
type SyntaxTree =

    internal new: filePath: string * ParsingConfig * SourceSnapshot -> SyntaxTree

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member GetParseResultAsync: unit -> Async<ParseResult>

    member GetSourceTextAsync:  unit -> Async<SourceText>

    //member GetTokensAsync: line: int -> Async<ImmutableArray<FSharpTokenInfo>>

    //member TryGetTokenAsync: line: int * column: int -> Async<FSharpTokenInfo option>