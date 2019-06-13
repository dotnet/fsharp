namespace FSharp.Compiler.Compilation

open System.IO
open System.Runtime.CompilerServices
open System.Threading
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host

[<Sealed>]
type FSharpSourceSnapshot =

    member FilePath: string

    member internal GetText: CancellationToken -> SourceText

    member internal TryGetText: unit -> SourceText option

    member internal IsStream: bool

    member internal TryGetStream: CancellationToken -> Stream option

    static member FromText: filePath: string * SourceText -> FSharpSourceSnapshot
    
[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    /// Create a F# source snapshot with the given source text.
    [<Extension>]
    static member CreateFSharpSourceSnapshot : this: ITemporaryStorageService * filePath: string * sourceText: SourceText * cancellationToken: CancellationToken -> FSharpSourceSnapshot

    /// Create a F# source snapshot by reading contents from a file.
    [<Extension>]
    static member CreateFSharpSourceSnapshot : this: ITemporaryStorageService * filePath: string * cancellationToken: CancellationToken -> FSharpSourceSnapshot
