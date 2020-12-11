// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Write Abstract IL structures at runtime using Reflection.Emit
//----------------------------------------------------------------------------


module internal FSharp.Compiler.AbstractIL.ILRuntimeWriter    

open System
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Collections.Generic

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.Internal.Utils
open FSharp.Compiler.AbstractIL.Diagnostics 
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Range
open FSharp.Core.Printf

val mkDynamicAssemblyAndModule: assemblyName:string * optimize:bool * debugInfo:bool * collectible:bool -> AssemblyBuilder * ModuleBuilder

type cenv = 
    { ilg: ILGlobals
      tryFindSysILTypeRef: string -> ILTypeRef option
      generatePdb: bool
      resolveAssemblyRef: (ILAssemblyRef -> Choice<string, Assembly> option) }

type emEnv

val emEnv0: emEnv

val emitModuleFragment:
    ilg:ILGlobals *
    emEnv:emEnv *
    asmB:AssemblyBuilder *
    modB:ModuleBuilder *
    modul:ILModuleDef *
    debugInfo:bool *
    resolveAssemblyRef:(ILAssemblyRef -> Choice<string,Assembly> option) *
    tryFindSysILTypeRef:(string -> ILTypeRef option) ->
      emEnv * (unit -> exn option) list

val LookupTypeRef: cenv:cenv -> emEnv:emEnv -> tref:ILTypeRef -> System.Type

val LookupType: cenv:cenv -> emEnv:emEnv -> ty:ILType -> System.Type

val LookupFieldRef: emEnv:emEnv -> fref:ILFieldRef -> FieldInfo option

val LookupMethodRef: emEnv:emEnv -> mref:ILMethodRef -> MethodInfo option
