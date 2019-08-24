namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Immutable
open Microsoft.CodeAnalysis

type [<RequireQualifiedAccess>] FSharpMetadataReference =
    | PortableExecutable of PortableExecutableReference
    | FSharpCompilation of FSharpCompilation 

and [<Sealed>] FSharpCompilation =

    member ReplaceSourceSnapshot: FSharpSourceSnapshot -> FSharpCompilation

    member GetSemanticModel: filePath: string -> FSharpSemanticModel

    member GetSyntaxTree: filePath: string -> FSharpSyntaxTree

    member GetDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    member Emit: peStream: Stream * ?pdbStreamOpt: Stream * ?ct: CancellationToken -> Result<unit, ImmutableArray<Diagnostic>>

    static member Create: assemblyName: string * sourceSnapshots: ImmutableArray<FSharpSourceSnapshot> * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?args: string list -> FSharpCompilation

    static member CreateScript: assemblyName: string * scriptSnapshot: FSharpSourceSnapshot * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?args: string list -> FSharpCompilation

    static member CreateScript: previousCompilation: FSharpCompilation * scriptSnapshot: FSharpSourceSnapshot * ?additionalMetadataReferences: ImmutableArray<FSharpMetadataReference> -> FSharpCompilation

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        member Compilation: FSharpCompilation
