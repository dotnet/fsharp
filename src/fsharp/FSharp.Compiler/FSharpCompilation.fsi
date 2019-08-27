[<AutoOpen>]
module FSharp.Compiler.Compilation.FSharpCompilation

open System.IO
open System.Threading
open System.Collections.Immutable
open Microsoft.CodeAnalysis

type FSharpOutputKind =
    | Exe = 0
    | WinExe = 1
    | Library = 2

[<Sealed>]
type FSharpMetadataReference =

    static member FromPortableExecutableReference: PortableExecutableReference -> FSharpMetadataReference

    static member FromFSharpCompilation: FSharpCompilation -> FSharpMetadataReference

and [<Sealed>] FSharpCompilation =

    member ReplaceSource: oldSrc: FSharpSource * newSrc: FSharpSource -> FSharpCompilation

    member GetSemanticModel: FSharpSource -> FSharpSemanticModel

    member GetSyntaxTree: FSharpSource -> FSharpSyntaxTree

    member GetSyntaxAndSemanticDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    member Emit: peStream: Stream * ?pdbStream: Stream * ?ct: CancellationToken -> Result<unit, ImmutableArray<Diagnostic>>

    static member Create: assemblyName: string * srcs: ImmutableArray<FSharpSource> * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?outputKind: FSharpOutputKind * ?args: string list -> FSharpCompilation

    static member CreateScript: scriptSnapshot: FSharpSource * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?args: string list -> FSharpCompilation

    static member CreateScript: previousCompilation: FSharpCompilation * scriptSnapshot: FSharpSource * ?additionalMetadataReferences: ImmutableArray<FSharpMetadataReference> -> FSharpCompilation

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        member Compilation: FSharpCompilation
