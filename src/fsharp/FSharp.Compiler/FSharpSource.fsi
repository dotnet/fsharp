namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open Microsoft.CodeAnalysis.Text

[<RequireQualifiedAccess>]
type SourceKind =
    | Implementation // ".fs"
    | Signature // ".fsi"
    | Script // ".fsx"

[<Sealed>]
type FSharpSource =

    /// The file associated with the source code.
    /// Will be empty if there is no file associated with the source code.
    /// Will never be null.
    member FilePath: string

    member GetText: CancellationToken -> SourceText

    member internal TryGetText: unit -> SourceText option

    member internal IsStream: bool

    member internal TryGetStream: CancellationToken -> Stream option

    static member FromText: SourceText * filePath: string * ?ct: CancellationToken -> FSharpSource

    static member FromText: string * filePath: string -> FSharpSource

    static member FromFile: filePath: string * ?ct: CancellationToken -> FSharpSource
