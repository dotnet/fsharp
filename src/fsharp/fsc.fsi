// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Driver

open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals

[<AbstractClass>]
type DiagnosticsLoggerProvider =
    new: unit -> DiagnosticsLoggerProvider
    abstract CreateDiagnosticsLoggerUpToMaxErrors:
        tcConfigBuilder: TcConfigBuilder * exiter: Exiter -> DiagnosticsLogger

/// The default DiagnosticsLoggerProvider implementation, reporting messages to the Console up to the maxerrors maximum
type ConsoleLoggerProvider =
    new: unit -> ConsoleLoggerProvider
    inherit DiagnosticsLoggerProvider

/// An error logger that reports errors up to some maximum, notifying the exiter when that maximum is reached
[<AbstractClass>]
type DiagnosticsLoggerUpToMaxErrors =
    inherit DiagnosticsLogger
    new: tcConfigB: TcConfigBuilder * exiter: Exiter * nameForDebugging: string -> DiagnosticsLoggerUpToMaxErrors

    /// Called when an error or warning occurs
    abstract HandleIssue:
        tcConfigB: TcConfigBuilder * error: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

    /// Called when 'too many errors' has occurred
    abstract HandleTooManyErrors: text: string -> unit

    override ErrorCount: int

    override DiagnosticSink: phasedError: PhasedDiagnostic * severity: FSharpDiagnosticSeverity -> unit

/// The main (non-incremental) compilation entry point used by fsc.exe
val mainCompile:
    ctok: CompilationThreadToken *
    argv: string [] *
    legacyReferenceResolver: LegacyReferenceResolver *
    bannerAlreadyPrinted: bool *
    reduceMemoryUsage: ReduceMemoryFlag *
    defaultCopyFSharpCore: CopyFSharpCoreFlag *
    exiter: Exiter *
    loggerProvider: DiagnosticsLoggerProvider *
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * TcGlobals * string * ILModuleDef -> unit) option ->
        unit

/// An additional compilation entry point used by FSharp.Compiler.Service taking syntax trees as input
val compileOfAst:
    ctok: CompilationThreadToken *
    legacyReferenceResolver: LegacyReferenceResolver *
    reduceMemoryUsage: ReduceMemoryFlag *
    assemblyName: string *
    target: CompilerTarget *
    targetDll: string *
    targetPdb: string option *
    dependencies: string list *
    noframework: bool *
    exiter: Exiter *
    loggerProvider: DiagnosticsLoggerProvider *
    inputs: ParsedInput list *
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * TcGlobals * string * ILModuleDef -> unit) option ->
        unit
