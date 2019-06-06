namespace FSharp.Compiler.Compilation

open System.Runtime.CompilerServices
open System.Threading
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host

[<RequireQualifiedAccess>]
type internal SourceStorage =
    | SourceText of ITemporaryTextStorage 
    | Stream of ITemporaryStreamStorage

[<Sealed>]
type FSharpSourceSnapshot =

    member FilePath: string

    member internal SourceStorage: SourceStorage
    
[<Sealed;AbstractClass;Extension>]
type ITemporaryStorageServiceExtensions =

    /// Create a F# source snapshot with the given source text.
    [<Extension>]
    static member CreateFSharpSourceSnapshot : this: ITemporaryStorageService * filePath: string * sourceText: SourceText * cancellationToken: CancellationToken -> FSharpSourceSnapshot

    /// Create a F# source snapshot by reading contents from a file.
    [<Extension>]
    static member CreateFSharpSourceSnapshot : this: ITemporaryStorageService * filePath: string * cancellationToken: CancellationToken -> FSharpSourceSnapshot
