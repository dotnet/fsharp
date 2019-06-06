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
type FSharpSourceSnapshot (filePath: string, sourceStorage: SourceStorage) =

    member __.FilePath = filePath

    member __.SourceStorage = sourceStorage

[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, filePath: string, sourceText: SourceText, ct) =
        let storage = this.CreateTemporaryTextStorage ct
        storage.WriteText (sourceText, ct)
        let sourceStorage = SourceStorage.SourceText storage
        FSharpSourceSnapshot (filePath, sourceStorage)

    [<Extension>]
    static member CreateFSharpSourceSnapshot (this: ITemporaryStorageService, filePath: string, ct) =
        let storage = this.CreateTemporaryStreamStorage ct
        use stream = File.OpenRead filePath
        storage.WriteStream (stream, ct)
        let sourceStorage = SourceStorage.Stream storage
        FSharpSourceSnapshot (filePath, sourceStorage)