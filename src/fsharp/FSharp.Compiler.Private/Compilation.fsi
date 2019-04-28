namespace FSharp.Compiler

open System
open System.Threading
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.CompileOptions

type Stamp = struct end

type SourceId = struct end

[<Sealed>]
type Source =

    member Id: SourceId

    member FilePath: string

    member TryGetSourceText: unit -> FSharp.Compiler.Text.ISourceText option

[<NoEquality;NoComparison>]
type CompilationOptions =
    {
        CompilationThreadToken: CompilationThreadToken
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
    }

    static member Create: assemblyPath: AssemblyPath * commandLineArgs: string list * projectDirectory: string -> CompilationOptions

type CompilationId = struct end

[<Sealed>]
type Compilation =

    member Id: CompilationId

[<NoEquality;NoComparison>]
type CompilationInfo =
    {
        Options: CompilationOptions
        Sources: SourceId seq
        CompilationReferences: CompilationId seq
    }

type ParseResult = (ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) [])

[<Sealed>]
type CompilationManager =

    new: compilationCacheSize: int * keepStrongly: int -> CompilationManager

    member AddSourceAsync: filePath: string -> Async<Source>

    member TryCreateCompilationAsync: CompilationInfo -> Async<Compilation option>

    member TryParseSourceFileAsync: SourceId * CompilationId -> Async<ParseResult option>

    member ParseSourceFilesAsync: SourceId seq * CompilationId -> Async<ParseResult seq>

