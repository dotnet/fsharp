// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckDeclarations

open Internal.Utilities.Library
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Import
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val AddLocalRootModuleOrNamespace:
    TcResultsSink -> TcGlobals -> ImportMap -> range -> TcEnv -> ModuleOrNamespaceType -> TcEnv

val CreateInitialTcEnv:
    TcGlobals * ImportMap * range * assemblyName: string * (CcuThunk * string list * string list) list ->
        OpenDeclaration list * TcEnv

val AddCcuToTcEnv:
    TcGlobals *
    ImportMap *
    range *
    TcEnv *
    assemblyName: string *
    ccu: CcuThunk *
    autoOpens: string list *
    internalsVisibleToAttributes: string list ->
        OpenDeclaration list * TcEnv

type TopAttribs =
    { mainMethodAttrs: Attribs
      netModuleAttrs: Attribs
      assemblyAttrs: Attribs }

type ConditionalDefines = string list

val EmptyTopAttrs: TopAttribs

val CombineTopAttrs: TopAttribs -> TopAttribs -> TopAttribs

val TcOpenModuleOrNamespaceDecl:
    TcResultsSink -> TcGlobals -> ImportMap -> range -> TcEnv -> LongIdent * range -> TcEnv * OpenDeclaration list

val AddLocalSubModule:
    g: TcGlobals -> amap: ImportMap -> m: range -> env: TcEnv -> moduleEntity: ModuleOrNamespace -> TcEnv

val CheckOneImplFile:
    TcGlobals *
    ImportMap *
    CcuThunk *
    OpenDeclaration list *
    (unit -> bool) *
    ConditionalDefines option *
    TcResultsSink *
    bool *
    TcEnv *
    ModuleOrNamespaceType option *
    ParsedImplFileInput ->
        Cancellable<TopAttribs * CheckedImplFile * TcEnv * bool>

val CheckOneSigFile:
    TcGlobals * ImportMap * CcuThunk * (unit -> bool) * ConditionalDefines option * TcResultsSink * bool ->
        TcEnv ->
        ParsedSigFileInput ->
            Cancellable<TcEnv * ModuleOrNamespaceType * bool>

exception NotUpperCaseConstructor of range: range

exception NotUpperCaseConstructorWithoutRQA of range: range
