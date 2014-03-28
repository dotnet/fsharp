// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.TypeChecker

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler 

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Env

open System.Collections.Generic

[<Sealed>]
type TcEnv =
    member DisplayEnv : DisplayEnv
    member NameEnv : Nameres.NameResolutionEnv

(* Incremental construction of environments, e.g. for F# Interactive *)
val internal CreateInitialTcEnv : TcGlobals * ImportMap * range * (CcuThunk * string list * bool) list -> TcEnv 
val internal AddCcuToTcEnv      : TcGlobals * ImportMap * range * TcEnv * CcuThunk * autoOpens: string list * bool -> TcEnv 
val internal AddLocalRootModuleOrNamespace : Nameres.TcResultsSink -> TcGlobals -> ImportMap -> range -> TcEnv -> ModuleOrNamespaceType -> TcEnv
val internal TcOpenDecl         : Nameres.TcResultsSink  -> TcGlobals -> ImportMap -> range -> range -> TcEnv -> Ast.LongIdent -> TcEnv 

type TopAttribs =
    { mainMethodAttrs : Attribs;
      netModuleAttrs  : Attribs;
      assemblyAttrs   : Attribs  }

type ConditionalDefines = 
    string list

val internal EmptyTopAttrs : TopAttribs
val internal CombineTopAttrs : TopAttribs -> TopAttribs -> TopAttribs

val internal TypecheckOneImplFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk * (unit -> bool) * ConditionalDefines * Nameres.TcResultsSink
      -> TcEnv 
      -> Tast.ModuleOrNamespaceType option
      -> ParsedImplFileInput
      -> Eventually<TopAttribs * Tast.TypedImplFile * TcEnv>

val internal TypecheckOneSigFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk  * (unit -> bool) * ConditionalDefines * Nameres.TcResultsSink 
      -> TcEnv                             
      -> ParsedSigFileInput
      -> Eventually<TcEnv * TcEnv * ModuleOrNamespaceType >

//-------------------------------------------------------------------------
// exceptions arising from type checking 
//------------------------------------------------------------------------- 

exception internal BakedInMemberConstraintName of string * range
exception internal FunctionExpected of DisplayEnv * TType * range
exception internal NotAFunction of DisplayEnv * TType * range * range
exception internal Recursion of DisplayEnv * Ast.Ident * TType * TType * range
exception internal RecursiveUseCheckedAtRuntime of DisplayEnv * ValRef * range
exception internal LetRecEvaluatedOutOfOrder of DisplayEnv * ValRef * ValRef * range
exception internal LetRecCheckedAtRuntime of range
exception internal LetRecUnsound of DisplayEnv * ValRef list * range
exception internal TyconBadArgs of DisplayEnv * TyconRef * int * range
exception internal UnionCaseWrongArguments of DisplayEnv * int * int * range
exception internal UnionCaseWrongNumberOfArgs of DisplayEnv * int * int * range
exception internal FieldsFromDifferentTypes of DisplayEnv * RecdFieldRef * RecdFieldRef * range
exception internal FieldGivenTwice of DisplayEnv * RecdFieldRef * range
exception internal MissingFields of string list * range
exception internal UnitTypeExpected of DisplayEnv * TType * bool * range
exception internal FunctionValueUnexpected of DisplayEnv * TType * range
exception internal UnionPatternsBindDifferentNames of range
exception internal VarBoundTwice of Ast.Ident
exception internal ValueRestriction of DisplayEnv * bool * Val * Typar * range
exception internal FieldNotMutable of DisplayEnv * RecdFieldRef * range
exception internal ValNotMutable of DisplayEnv * ValRef * range
exception internal ValNotLocal of DisplayEnv * ValRef * range
exception internal InvalidRuntimeCoercion of DisplayEnv * TType * TType * range
exception internal IndeterminateRuntimeCoercion of DisplayEnv * TType * TType * range
exception internal IndeterminateStaticCoercion of DisplayEnv * TType * TType * range
exception internal StaticCoercionShouldUseBox of DisplayEnv * TType * TType * range
exception internal RuntimeCoercionSourceSealed of DisplayEnv * TType * range
exception internal CoercionTargetSealed of DisplayEnv * TType * range
exception internal UpcastUnnecessary of range
exception internal TypeTestUnnecessary of range
exception internal SelfRefObjCtor of bool * range
exception internal VirtualAugmentationOnNullValuedType of range
exception internal NonVirtualAugmentationOnNullValuedType of range
exception internal UseOfAddressOfOperator of range
exception internal DeprecatedThreadStaticBindingWarning of range
exception internal NotUpperCaseConstructor of range
exception internal IntfImplInIntrinsicAugmentation of range
exception internal IntfImplInExtrinsicAugmentation of range
exception internal OverrideInIntrinsicAugmentation of range
exception internal OverrideInExtrinsicAugmentation of range
exception internal NonUniqueInferredAbstractSlot of TcGlobals * DisplayEnv * string * MethInfo * MethInfo * range
exception internal StandardOperatorRedefinitionWarning of string * range
exception internal ParameterlessStructCtor of range

val internal TcFieldInit : range -> ILFieldInit -> Tast.Const

val IsSecurityAttribute : TcGlobals -> ImportMap -> Dictionary<Stamp,bool> -> Attrib -> range -> bool
val IsSecurityCriticalAttribute : TcGlobals -> Attrib -> bool
val LightweightTcValForUsingInBuildMethodCall : g : TcGlobals -> vref:ValRef -> vrefFlags : ValUseFlag -> vrefTypeInst : TTypes -> m : range -> Expr * TType