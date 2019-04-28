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

    static member Create: assemblyPath: AssemblyPath * commandLineArgs: string list * projectDirectory: string * isExecutable: bool -> CompilationOptions

type SyntaxTree = 
    {
        FilePath: string
        ParseResult: ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) []
    }

[<Sealed>]
type Compilation

[<NoEquality;NoComparison>]
type CompilationInfo =
    {
        Options: CompilationOptions
        ParseResults: SyntaxTree seq
        CompilationReferences: Compilation seq
    }

[<Sealed>]
type CompilerService =

    new: compilationCacheSize: int * keepStrongly: int -> CompilerService

    member TryCreateCompilationAsync: CompilationInfo -> Async<Compilation option>
