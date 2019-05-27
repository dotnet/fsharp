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

    member internal GetSourceValueAsync: unit -> Async<SourceText>

[<Sealed;AbstractClass;Extension>]
type internal ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string * SourceText -> Cancellable<SourceSnapshot>

[<Sealed>]
type SyntaxTree =

    internal new: filePath: string * ParsingInfo * AsyncLazyWeak<ParseResult> -> SyntaxTree

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingInfo: ParsingInfo

    member GetParseResultAsync: unit -> Async<ParseResult>

    member GetSourceText:  unit -> Async<SourceText>