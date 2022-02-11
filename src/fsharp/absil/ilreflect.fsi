// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Write Abstract IL structures at runtime using Reflection.Emit
module internal FSharp.Compiler.AbstractIL.ILRuntimeWriter    

open System.Reflection
open System.Reflection.Emit

open FSharp.Compiler.AbstractIL.IL

val mkDynamicAssemblyAndModule: assemblyName: string * optimize: bool * debugInfo: bool * collectible: bool -> AssemblyBuilder * ModuleBuilder

type cenv = 
    { ilg: ILGlobals
      emitTailcalls: bool
      tryFindSysILTypeRef: string -> ILTypeRef option
      generatePdb: bool
      resolveAssemblyRef: ILAssemblyRef -> Choice<string, Assembly> option }

type ILReflectEmitEnv

val emEnv0: ILReflectEmitEnv

val emitModuleFragment: 
    ilg: ILGlobals *
    emitTailcalls: bool *
    emEnv: ILReflectEmitEnv *
    asmB: AssemblyBuilder *
    modB: ModuleBuilder *
    modul: ILModuleDef *
    debugInfo: bool *
    resolveAssemblyRef: (ILAssemblyRef -> Choice<string,Assembly> option) *
    tryFindSysILTypeRef: (string -> ILTypeRef option) ->
      ILReflectEmitEnv * (unit -> exn option) list

val LookupTypeRef: cenv: cenv -> emEnv: ILReflectEmitEnv -> tref: ILTypeRef -> System.Type

val LookupType: cenv: cenv -> emEnv: ILReflectEmitEnv -> ty: ILType -> System.Type

