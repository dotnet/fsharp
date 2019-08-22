namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Immutable
open Microsoft.CodeAnalysis

[<NoEquality;NoComparison>]
type internal FSharpCompilationOptions =
    {
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        Script: FSharpSourceSnapshot option
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
        SourceSnapshots: ImmutableArray<FSharpSourceSnapshot>
        MetadataReferences: ImmutableArray<FSharpMetadataReference>
    }

and [<RequireQualifiedAccess>] FSharpMetadataReference =
    | PortableExecutable of PortableExecutableReference
    | FSharpCompilation of FSharpCompilation 

and [<Sealed>] FSharpCompilation =

    member internal Options: FSharpCompilationOptions

    member ReplaceSourceSnapshot: FSharpSourceSnapshot -> FSharpCompilation

    member GetSemanticModel: filePath: string -> FSharpSemanticModel

    member GetSyntaxTree: filePath: string -> FSharpSyntaxTree

    member SetOptions: FSharpCompilationOptions -> FSharpCompilation

    member GetDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    member Emit: peStream: Stream * ?pdbStreamOpt: Stream * ?ct: CancellationToken -> Result<unit, ImmutableArray<Diagnostic>>

    static member Create: assmeblyPath: string * projectDirectory: string * sourceSnapshots: ImmutableArray<FSharpSourceSnapshot> * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?args: string list -> FSharpCompilation

    static member CreateScript: assemblyPath: string * ProjectDirectory: string * scriptSnapshot: FSharpSourceSnapshot * metadataReferences: ImmutableArray<FSharpMetadataReference> * ?args: string list -> FSharpCompilation

    static member internal Create: FSharpCompilationOptions -> FSharpCompilation

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        member Compilation: FSharpCompilation
