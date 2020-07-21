// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Driver 

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

[<AbstractClass>]
type ErrorLoggerProvider =
    new : unit -> ErrorLoggerProvider
    abstract CreateErrorLoggerUpToMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

type StrongNameSigningInfo 

val EncodeInterfaceData: tcConfig:TcConfig * tcGlobals:TcGlobals * exportRemapping:Remap * generatedCcu: CcuThunk * outfile: string * isIncrementalBuild: bool -> ILAttribute list * ILResource list

val ValidateKeySigningAttributes : tcConfig:TcConfig * tcGlobals:TcGlobals * TopAttribs -> StrongNameSigningInfo

val GetStrongNameSigner : StrongNameSigningInfo -> ILBinaryWriter.ILStrongNameSigner option

/// Process the given set of command line arguments
val internal ProcessCommandLineFlags : TcConfigBuilder * setProcessThreadLocals:(TcConfigBuilder -> unit) * lcidFromCodePage : int option * argv:string[] -> string list

//---------------------------------------------------------------------------
// The entry point used by fsc.exe

val typecheckAndCompile : 
    ctok: CompilationThreadToken *
    argv : string[] * 
    legacyReferenceResolver: ReferenceResolver.Resolver * 
    bannerAlreadyPrinted : bool * 
    reduceMemoryUsage: ReduceMemoryFlag * 
    defaultCopyFSharpCore: CopyFSharpCoreFlag * 
    exiter : Exiter *
    loggerProvider: ErrorLoggerProvider *
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcGlobals * string * ILModuleDef -> unit) option
      -> unit

val mainCompile : 
    ctok: CompilationThreadToken *
    argv: string[] * 
    legacyReferenceResolver: ReferenceResolver.Resolver * 
    bannerAlreadyPrinted: bool * 
    reduceMemoryUsage: ReduceMemoryFlag * 
    defaultCopyFSharpCore: CopyFSharpCoreFlag * 
    exiter: Exiter * 
    loggerProvider: ErrorLoggerProvider * 
    tcImportsCapture: (TcImports -> unit) option *
    dynamicAssemblyCreator: (TcGlobals * string * ILModuleDef -> unit) option
      -> unit

val compileOfAst : 
    ctok: CompilationThreadToken *
    legacyReferenceResolver: ReferenceResolver.Resolver * 
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
    dynamicAssemblyCreator: (TcGlobals * string * ILModuleDef -> unit) option
      -> unit

/// Part of LegacyHostedCompilerForTesting
type InProcErrorLoggerProvider = 
    new : unit -> InProcErrorLoggerProvider
    member Provider : ErrorLoggerProvider
    member CapturedWarnings : Diagnostic[]
    member CapturedErrors : Diagnostic[]

/// The default ErrorLogger implementation, reporting messages to the Console up to the maxerrors maximum
type ConsoleLoggerProvider = 
    new : unit -> ConsoleLoggerProvider
    inherit ErrorLoggerProvider

// For unit testing
module internal MainModuleBuilder =
    
    val fileVersion: findStringAttr: (string -> string option) -> assemblyVersion: ILVersionInfo -> ILVersionInfo
    val productVersion: findStringAttr: (string -> string option) -> fileVersion: ILVersionInfo -> string
    val productVersionToILVersionInfo: string -> ILVersionInfo
