// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckDeclarations

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Range
open FSharp.Compiler.Import
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val AddLocalRootModuleOrNamespace : NameResolution.TcResultsSink -> TcGlobals -> ImportMap -> Range -> TcEnv -> ModuleOrNamespaceType -> TcEnv
val CreateInitialTcEnv : TcGlobals * ImportMap * Range * assemblyName: string * (CcuThunk * string list * string list) list -> TcEnv 
val AddCcuToTcEnv: TcGlobals * ImportMap * Range * TcEnv * assemblyName: string * ccu: CcuThunk * autoOpens: string list * internalsVisibleToAttributes: string list -> TcEnv 

type TopAttribs =
    { mainMethodAttrs: Attribs
      netModuleAttrs: Attribs
      assemblyAttrs: Attribs  }

type ConditionalDefines = string list

val EmptyTopAttrs : TopAttribs
val CombineTopAttrs : TopAttribs -> TopAttribs -> TopAttribs

val TcOpenModuleOrNamespaceDecl: TcResultsSink  -> TcGlobals -> ImportMap -> Range -> TcEnv -> (LongIdent * Range) -> TcEnv

val AddLocalSubModule: g: TcGlobals -> amap: ImportMap -> m: Range -> env: TcEnv -> modul: ModuleOrNamespace -> TcEnv

val TypeCheckOneImplFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk * (unit -> bool) * ConditionalDefines option * NameResolution.TcResultsSink * bool
      -> TcEnv 
      -> ModuleOrNamespaceType option
      -> ParsedImplFileInput
      -> Eventually<TopAttribs * TypedImplFile * ModuleOrNamespaceType * TcEnv * bool>

val TypeCheckOneSigFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk  * (unit -> bool) * ConditionalDefines option * NameResolution.TcResultsSink * bool
      -> TcEnv                             
      -> ParsedSigFileInput
      -> Eventually<TcEnv * ModuleOrNamespaceType * bool>

exception ParameterlessStructCtor of Range
exception NotUpperCaseConstructor of Range
