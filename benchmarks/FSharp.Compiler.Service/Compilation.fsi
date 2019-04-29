namespace FSharp.Compiler.Service

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
open FSharp.Compiler.Service.Utilities

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
type Compilation

[<NoEquality;NoComparison>]
type CompilationInfo =
    {
        Options: CompilationOptions
        Sources: ImmutableArray<Source>
        CompilationReferences: ImmutableArray<Compilation>
    }

[<Sealed>]
type CompilerService =

    new: compilationCacheSize: int * keepStrongly: int -> CompilerService

    member TryCreateCompilationAsync: CompilationInfo -> Async<Compilation option>
