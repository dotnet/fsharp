namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

[<NoEquality;NoComparison>]
type CompilationOptions =
    {
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
    }

    static member Create: assemblyPath: string * commandLineArgs: string list * projectDirectory: string * isExecutable: bool -> CompilationOptions

[<Sealed>]
type Compilation =

    member CheckAsync: filePath: string -> Async<unit>

[<NoEquality;NoComparison>]
type CompilationInfo =
    {
        Options: CompilationOptions
        SourceSnapshots: ImmutableArray<SourceSnapshot>
        CompilationReferences: ImmutableArray<Compilation>
    }

[<Sealed>]
type CompilationService =

    new: compilationCacheSize: int * keepStrongly: int * Microsoft.CodeAnalysis.Workspace -> CompilationService

    member CreateSourceSnapshot: filePath: string * Microsoft.CodeAnalysis.Text.SourceText -> SourceSnapshot

    member CreateCompilation: CompilationInfo -> Compilation
