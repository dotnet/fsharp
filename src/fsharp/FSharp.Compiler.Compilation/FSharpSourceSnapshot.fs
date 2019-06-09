namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
type SourceStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage

[<Sealed>]
type FSharpSourceSnapshot (filePath: string, sourceStorage: SourceStorage, initialText: WeakReference<SourceText>) =

    let mutable weakText = initialText

    member __.FilePath = filePath

    member this.GetText ct =
        match this.TryGetText () with
        | Some text -> text
        | _ ->
            let text =
                match sourceStorage with
                | SourceStorage.SourceText storage ->
                    storage.ReadText ct
                | SourceStorage.Stream storage ->
                    let stream = storage.ReadStream ct
                    SourceText.From stream
            weakText <- WeakReference<_> text
            text

    member __.TryGetText () =
        match weakText.TryGetTarget () with
        | true, text -> Some text
        | _ -> None

    member __.IsStream =
        match sourceStorage with
        | SourceStorage.Stream _ -> true
        | _ -> false

    member __.TryGetStream ct =
        match sourceStorage with
        | SourceStorage.Stream storage ->
            Some (storage.ReadStream ct)
        | _ ->
            None

[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, filePath: string, sourceText: SourceText, ct) =
        let storage = this.CreateTemporaryTextStorage ct
        storage.WriteText (sourceText, ct)
        let sourceStorage = SourceStorage.SourceText storage
        FSharpSourceSnapshot (filePath, sourceStorage, WeakReference<_> sourceText)

    [<Extension>]
    static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, filePath: string, ct) =
        let storage = this.CreateTemporaryStreamStorage ct
        use stream = File.OpenRead filePath
        storage.WriteStream (stream, ct)
        let sourceStorage = SourceStorage.Stream storage
        FSharpSourceSnapshot (filePath, sourceStorage, WeakReference<_> null)