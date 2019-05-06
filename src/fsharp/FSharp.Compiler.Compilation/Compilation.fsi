namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open FSharp.Compiler
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library

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
        CompilationReferences: ImmutableArray<Compilation>
    }

    static member Create: assemblyPath: string * projectDirectory: string * ImmutableArray<SourceSnapshot> * ImmutableArray<Compilation> -> CompilationOptions

and [<Sealed>] Compilation =

    member internal Id: CompilationId

    member Options: CompilationOptions

    member ReplaceSourceSnapshot: SourceSnapshot -> Compilation

    member GetSemanticModel: filePath: string -> SemanticModel

[<RequireQualifiedAccess>]
module internal Compilation =

    val create: CompilationOptions -> CompilationGlobalOptions -> CompilationCaches -> Compilation
