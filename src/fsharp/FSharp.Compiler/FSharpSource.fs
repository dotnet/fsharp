namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
type SourceStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage
    | DirectText of SourceText

[<RequireQualifiedAccess>]
type SourceKind =
    | Implementation // ".fs"
    | Signature // ".fsi"
    | Script // ".fsx"

[<Sealed>]
type FSharpSource (filePath: string, sourceStorage: SourceStorage, initialText: WeakReference<SourceText>) =

    let kind =
        match Path.GetExtension(filePath).ToLower() with
        | ".fs" -> SourceKind.Implementation
        | ".fsi" -> SourceKind.Signature
        | ".fsx" -> SourceKind.Script
        | ext -> failwithf "File extension, %s, not supported." ext

    let mutable weakText = initialText

    member __.FilePath = filePath

    member __.Kind = kind

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
                | SourceStorage.DirectText text ->
                    text
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

[<AutoOpen>]
module StorageHelpers =

    let workspace = new AdhocWorkspace ()
    let temporaryStorage = workspace.Services.TemporaryStorage

    [<Sealed;AbstractClass;Extension>]
    type ITemporaryStorageServiceExtensions =

        [<Extension>]
        static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, sourceText: SourceText, filePath, ct) =
            let storage = this.CreateTemporaryTextStorage ct
            storage.WriteText (sourceText, ct)
            let sourceStorage = SourceStorage.SourceText storage
            FSharpSource (filePath, sourceStorage, WeakReference<_> sourceText)

        [<Extension>]
        static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, filePath: string, ct) =
            let storage = this.CreateTemporaryStreamStorage ct
            use stream = File.OpenRead filePath
            storage.WriteStream (stream, ct)
            let sourceStorage = SourceStorage.Stream storage
            FSharpSource (filePath, sourceStorage, WeakReference<_> null)

type FSharpSource with

    static member FromFile (filePath: string, ?ct) =
        let ct = defaultArg ct CancellationToken.None
        temporaryStorage.CreateFSharpSourceSnapshot (filePath, ct)

    static member FromText (text: SourceText, filePath, ?ct) =
        let ct = defaultArg ct CancellationToken.None
        temporaryStorage.CreateFSharpSourceSnapshot (text, filePath, ct)

    static member FromText (text: string, filePath) =
        let text = SourceText.From text
        FSharpSource (filePath, SourceStorage.DirectText text, WeakReference<_> text)