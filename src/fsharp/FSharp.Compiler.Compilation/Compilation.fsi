namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library

type MetadataReference = Microsoft.CodeAnalysis.MetadataReference

type internal CompilationId = struct end

[<NoEquality;NoComparison>]
type internal CompilationCaches =
    {
        incrementalCheckerCache: MruWeakCache<struct (CompilationId * VersionStamp), IncrementalChecker>
        frameworkTcImportsCache: FrameworkImportsCache
    }

[<NoEquality;NoComparison>]
type CompilationGlobalOptions =
    {
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
    }

    static member Create: unit -> CompilationGlobalOptions

[<NoEquality;NoComparison>]
type CompilationOptions =
    {
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
        SourceSnapshots: ImmutableArray<SourceSnapshot>
        MetadataReferences: ImmutableArray<MetadataReference>
    }

    static member Create: assemblyPath: string * projectDirectory: string * ImmutableArray<SourceSnapshot> * ImmutableArray<MetadataReference> -> CompilationOptions

and [<Sealed>] Compilation =

    member internal Id: CompilationId

    member Options: CompilationOptions

    member ReplaceSourceSnapshot: SourceSnapshot -> Compilation

    member GetSemanticModel: filePath: string -> SemanticModel

    member GetSyntaxTree: filePath: string -> SyntaxTree

[<RequireQualifiedAccess>]
module internal Compilation =

    val create: CompilationOptions -> CompilationGlobalOptions -> CompilationCaches -> Compilation
