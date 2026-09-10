// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Write Abstract IL structures at runtime using Reflection.Emit
module internal FSharp.Compiler.AbstractIL.ILDynamicAssemblyWriter

open System.Reflection
open System.Reflection.Emit

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Text

/// A type reference's name as reflection spells it, classifying the namespace, the enclosing types and
/// the name itself separately. Only a reference is at hand, so what kind of type it is is not known.
val richTextOfILTypeRef: tref: ILTypeRef -> RichText

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
