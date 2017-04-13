// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Driver 

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TypeChecker

[<AbstractClass>]
type ErrorLoggerProvider =
    new : unit -> ErrorLoggerProvider
    abstract CreateErrorLoggerUpToMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

type StrongNameSigningInfo 

val EncodeInterfaceData: tcConfig:TcConfig * tcGlobals:TcGlobals * exportRemapping:Tastops.Remap * generatedCcu: Tast.CcuThunk * outfile: string * isIncrementalBuild: bool -> ILAttribute list * ILResource list
val ValidateKeySigningAttributes : tcConfig:TcConfig * tcGlobals:TcGlobals * TypeChecker.TopAttribs -> StrongNameSigningInfo
val GetStrongNameSigner : StrongNameSigningInfo -> ILBinaryWriter.ILStrongNameSigner option

/// Process the given set of command line arguments
val internal ProcessCommandLineFlags : TcConfigBuilder * setProcessThreadLocals:(TcConfigBuilder -> unit) * lcidFromCodePage : int option * argv:string[] -> string list

//---------------------------------------------------------------------------
// The entry point used by fsc.exe

val typecheckAndCompile : 
    argv : string[] * 
    referenceResolver: ReferenceResolver.Resolver * 
    bannerAlreadyPrinted : bool * 
    exiter : Exiter *
    loggerProvider: ErrorLoggerProvider -> unit

val mainCompile : 
    argv : string[] * 
    referenceResolver: ReferenceResolver.Resolver * 
    bannerAlreadyPrinted : bool * 
    exiter : Exiter -> unit


/// Part of LegacyHostedCompilerForTesting
type InProcErrorLoggerProvider = 
    new : unit -> InProcErrorLoggerProvider
    member Provider : ErrorLoggerProvider
    member CapturedWarnings : Diagnostic[]
    member CapturedErrors : Diagnostic[]


module internal MainModuleBuilder =
    
    val fileVersion: warn: (exn -> unit) -> findStringAttr: (string -> string option) -> assemblyVersion: AbstractIL.IL.ILVersionInfo -> AbstractIL.IL.ILVersionInfo
    val productVersion: warn: (exn -> unit) -> findStringAttr: (string -> string option) -> fileVersion: AbstractIL.IL.ILVersionInfo -> string
    val productVersionToILVersionInfo: string -> AbstractIL.IL.ILVersionInfo
