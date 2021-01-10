// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Driver 

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

[<AbstractClass>]
type ErrorLoggerProvider =
    new : unit -> ErrorLoggerProvider
    abstract CreateErrorLoggerUpToMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

/// The default ErrorLoggerProvider implementation, reporting messages to the Console up to the maxerrors maximum
type ConsoleLoggerProvider = 
    new : unit -> ConsoleLoggerProvider
    inherit ErrorLoggerProvider

/// Encode the F# interface data into a set of IL attributes and resources
val EncodeSignatureData:
    tcConfig:TcConfig *
    tcGlobals:TcGlobals *
    exportRemapping:Remap *
    generatedCcu: CcuThunk *
    outfile: string *
    isIncrementalBuild: bool
      -> ILAttribute list * ILResource list

/// The main (non-incremental) compilation entry point used by fsc.exe
val mainCompile: 
    ctok: CompilationThreadToken *
    argv: string[] * 
    legacyReferenceResolver: LegacyReferenceResolver * 
    bannerAlreadyPrinted: bool * 
    reduceMemoryUsage: ReduceMemoryFlag * 
    defaultCopyFSharpCore: CopyFSharpCoreFlag * 
    exiter: Exiter * 
    loggerProvider: ErrorLoggerProvider * 
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * TcGlobals * string * ILModuleDef -> unit) option
      -> unit

/// An additional compilation entry point used by FSharp.Compiler.Service taking syntax trees as input
val compileOfAst: 
    ctok: CompilationThreadToken *
    legacyReferenceResolver: LegacyReferenceResolver * 
    reduceMemoryUsage: ReduceMemoryFlag * 
    assemblyName:string * 
    target:CompilerTarget * 
    targetDll:string * 
    targetPdb:string option * 
    dependencies:string list * 
    noframework:bool *
    exiter:Exiter * 
    loggerProvider: ErrorLoggerProvider * 
    inputs:ParsedInput list *
    tcImportsCapture : (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcConfig * TcGlobals * string * ILModuleDef -> unit) option
      -> unit

/// Part of LegacyHostedCompilerForTesting
type InProcErrorLoggerProvider = 
    new : unit -> InProcErrorLoggerProvider
    member Provider : ErrorLoggerProvider
    member CapturedWarnings : Diagnostic[]
    member CapturedErrors : Diagnostic[]

