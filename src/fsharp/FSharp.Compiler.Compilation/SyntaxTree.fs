namespace FSharp.Compiler.Compilation

open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library

[<RequireQualifiedAccess>]
type SourceValueStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage

[<Sealed>]
type SourceSnapshot (filePath: string, value: SourceValueStorage) =
    
    member __.FilePath = filePath

    member __.GetSourceValueAsync () =
        async {
            let! cancellationToken = Async.CancellationToken
            match value with
            | SourceValueStorage.SourceText storage -> 
                let! sourceText = storage.ReadTextAsync cancellationToken |> Async.AwaitTask
                return SourceValue.SourceText sourceText
            | SourceValueStorage.Stream storage -> 
                let! stream = storage.ReadStreamAsync cancellationToken |> Async.AwaitTask
                return SourceValue.Stream stream
        }

[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot (this: ITemporaryStorageService, filePath: string, sourceText: SourceText) =
        cancellable {
            let! cancellationToken = Cancellable.token ()
            let storage = this.CreateTemporaryTextStorage cancellationToken
            storage.WriteText (sourceText, cancellationToken)
            let sourceValueStorage = SourceValueStorage.SourceText storage
            return SourceSnapshot (filePath, sourceValueStorage)
        }

[<Sealed>]
type SyntaxTree (filePath: string, parsingInfo: ParsingInfo, asyncLazyWeakGetParseResult: AsyncLazyWeak<ParseResult>) =

    member __.FilePath = filePath

    member __.ParsingInfo = parsingInfo

    member this.GetParseResultAsync () =
        asyncLazyWeakGetParseResult.GetValueAsync ()
