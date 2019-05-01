namespace FSharp.Compiler.Compilation

open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities

[<Sealed>]
type SourceSnapshot =

    member FilePath: string

    member internal GetSourceValueAsync: unit -> Async<SourceValue>

[<Sealed;AbstractClass;Extension>]
type internal ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string * SourceText -> Cancellable<SourceSnapshot>

[<Sealed>]
type internal SyntaxTree =

    new: filePath: string * ParsingInfo * AsyncLazyWeak<ParseResult> -> SyntaxTree

    member FilePath: string

    member ParsingInfo: ParsingInfo

    member GetParseResultAsync: unit -> Async<ParseResult>