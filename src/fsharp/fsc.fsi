// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Driver 

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TypeChecker

[<AbstractClass>]
type ErrorLoggerProvider =
    new : unit -> ErrorLoggerProvider
    abstract CreateErrorLoggerThatQuitsAfterMaxErrors : tcConfigBuilder : TcConfigBuilder * exiter : Exiter -> ErrorLogger

#if NO_COMPILER_BACKEND
#else

type SigningInfo = SigningInfo of (* delaysign:*) bool * (* publicsign:*) bool * (*signer:*)  string option * (*container:*) string option

val EncodeInterfaceData: tcConfig:TcConfig * tcGlobals:TcGlobals * exportRemapping:Tastops.Remap * generatedCcu: Tast.CcuThunk * outfile: string * isIncrementalBuild: bool -> ILAttribute list * ILResource list
val ValidateKeySigningAttributes : tcConfig:TcConfig * tcGlobals:TcGlobals * TypeChecker.TopAttribs -> SigningInfo
val GetSigner : SigningInfo -> ILBinaryWriter.ILStrongNameSigner option

type ILResource with 
    /// Read the bytes from a resource local to an assembly
    member internal Bytes : byte[]

/// Proccess the given set of command line arguments
#if FX_LCIDFROMCODEPAGE
val internal ProcessCommandLineFlags : TcConfigBuilder * setProcessThreadLocals:(TcConfigBuilder -> unit) * lcidFromCodePage : int option * argv:string[] -> string list
#else
val internal ProcessCommandLineFlags : TcConfigBuilder * setProcessThreadLocals:(TcConfigBuilder -> unit) * argv:string[] -> string list
#endif

//---------------------------------------------------------------------------
// The entry point used by fsc.exe

val mainCompile : argv : string[] * bannerAlreadyPrinted : bool * exiter : Exiter -> unit

//---------------------------------------------------------------------------
// The micro API into the compiler used by the visualfsharp test infrastructure

[<RequireQualifiedAccess>]
type CompilationOutput = 
    { Errors : ErrorOrWarning[]
      Warnings : ErrorOrWarning[] }

type InProcCompiler = 
    new : unit -> InProcCompiler
    member Compile : args : string[] -> bool * CompilationOutput


module internal MainModuleBuilder =
    
    val fileVersion: warn: (exn -> unit) -> findStringAttr: (string -> string option) -> assemblyVersion: AbstractIL.IL.ILVersionInfo -> AbstractIL.IL.ILVersionInfo
    val productVersion: warn: (exn -> unit) -> findStringAttr: (string -> string option) -> fileVersion: AbstractIL.IL.ILVersionInfo -> string
    val productVersionToILVersionInfo: string -> AbstractIL.IL.ILVersionInfo
#endif
