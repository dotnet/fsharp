// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Write Abstract IL structures at runtime using Reflection.Emit
module internal FSharp.Compiler.AbstractIL.ILDynamicAssemblyWriter

open System.Reflection
open System.Reflection.Emit

open FSharp.Compiler.AbstractIL.IL

val mkDynamicAssemblyAndModule:
    assemblyName: string * optimize: bool * collectible: bool -> AssemblyBuilder * ModuleBuilder

type cenv =
    { ilg: ILGlobals
      emitTailcalls: bool
      tryFindSysILTypeRef: string -> ILTypeRef option
      generatePdb: bool
      resolveAssemblyRef: ILAssemblyRef -> Choice<string, Assembly> option }

type ILDynamicAssemblyEmitEnv

val emEnv0: ILDynamicAssemblyEmitEnv

val EmitDynamicAssemblyFragment:
    ilg: ILGlobals *
    emitTailcalls: bool *
    emEnv: ILDynamicAssemblyEmitEnv *
    asmB: AssemblyBuilder *
    modB: ModuleBuilder *
    modul: ILModuleDef *
    debugInfo: bool *
    resolveAssemblyRef: (ILAssemblyRef -> Choice<string, Assembly> option) *
    tryFindSysILTypeRef: (string -> ILTypeRef option) ->
        ILDynamicAssemblyEmitEnv * (unit -> exn option) list

val LookupTypeRef: cenv: cenv -> emEnv: ILDynamicAssemblyEmitEnv -> tref: ILTypeRef -> System.Type

val LookupType: cenv: cenv -> emEnv: ILDynamicAssemblyEmitEnv -> ty: ILType -> System.Type
