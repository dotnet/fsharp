namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open Microsoft.CodeAnalysis

[<NoEquality;NoComparison>]
type FSharpCompilationOptions =
    {
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
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

    member Options: FSharpCompilationOptions

    member ReplaceSourceSnapshot: FSharpSourceSnapshot -> FSharpCompilation

    member GetSemanticModel: filePath: string -> FSharpSemanticModel

    member GetSyntaxTree: filePath: string -> FSharpSyntaxTree

    static member Create: assmeblyPath: string * projectDirectory: string * sourceSnapshots: ImmutableArray<FSharpSourceSnapshot> * metadataReferences: ImmutableArray<FSharpMetadataReference> -> FSharpCompilation

    static member Create: FSharpCompilationOptions -> FSharpCompilation

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        member Compilation: FSharpCompilation