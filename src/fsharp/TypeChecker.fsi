// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.TypeChecker

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.AccessibilityLogic
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.TcGlobals

open System.Collections.Generic

[<Sealed>]
type TcEnv =
    member DisplayEnv : DisplayEnv
    member NameEnv : NameResolution.NameResolutionEnv
    member AccessRights : AccessorDomain

val CreateInitialTcEnv : TcGlobals * ImportMap * range * assemblyName: string * (CcuThunk * string list * string list) list -> TcEnv 
val AddCcuToTcEnv      : TcGlobals * ImportMap * range * TcEnv * assemblyName: string * ccu: CcuThunk * autoOpens: string list * internalsVisibleToAttributes: string list -> TcEnv 
val AddLocalRootModuleOrNamespace : NameResolution.TcResultsSink -> TcGlobals -> ImportMap -> range -> TcEnv -> ModuleOrNamespaceType -> TcEnv
val TcOpenDecl         : NameResolution.TcResultsSink  -> TcGlobals -> ImportMap -> range -> range -> TcEnv -> Ast.LongIdent -> TcEnv 

type TopAttribs =
    { mainMethodAttrs : Attribs;
      netModuleAttrs  : Attribs;
      assemblyAttrs   : Attribs  }

type ConditionalDefines = 
    string list

val EmptyTopAttrs : TopAttribs
val CombineTopAttrs : TopAttribs -> TopAttribs -> TopAttribs

val TypeCheckOneImplFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk * (unit -> bool) * ConditionalDefines * NameResolution.TcResultsSink
      -> TcEnv
      -> (string -> SynExpr)
      -> Tast.ModuleOrNamespaceType option
      -> ParsedImplFileInput
      -> Eventually<TopAttribs * Tast.TypedImplFile * TcEnv>

val TypeCheckOneSigFile : 
      TcGlobals * NiceNameGenerator * ImportMap * CcuThunk  * (unit -> bool) * ConditionalDefines * NameResolution.TcResultsSink 
      -> TcEnv
      -> (string -> SynExpr)
      -> ParsedSigFileInput
      -> Eventually<TcEnv * TcEnv * ModuleOrNamespaceType >

//-------------------------------------------------------------------------
// Some of the exceptions arising from type checking. These should be moved to 
// use ErrorLogger.
//------------------------------------------------------------------------- 

exception BakedInMemberConstraintName of string * range
exception FunctionExpected of DisplayEnv * TType * range
exception NotAFunction of DisplayEnv * TType * range * range
exception Recursion of DisplayEnv * Ast.Ident * TType * TType * range
exception RecursiveUseCheckedAtRuntime of DisplayEnv * ValRef * range
exception LetRecEvaluatedOutOfOrder of DisplayEnv * ValRef * ValRef * range
exception LetRecCheckedAtRuntime of range
exception LetRecUnsound of DisplayEnv * ValRef list * range
exception TyconBadArgs of DisplayEnv * TyconRef * int * range
exception UnionCaseWrongArguments of DisplayEnv * int * int * range
exception UnionCaseWrongNumberOfArgs of DisplayEnv * int * int * range
exception FieldsFromDifferentTypes of DisplayEnv * RecdFieldRef * RecdFieldRef * range
exception FieldGivenTwice of DisplayEnv * RecdFieldRef * range
exception MissingFields of string list * range
exception UnitTypeExpected of DisplayEnv * TType * bool * range
exception FunctionValueUnexpected of DisplayEnv * TType * range
exception UnionPatternsBindDifferentNames of range
exception VarBoundTwice of Ast.Ident
exception ValueRestriction of DisplayEnv * bool * Val * Typar * range
exception FieldNotMutable of DisplayEnv * RecdFieldRef * range
exception ValNotMutable of DisplayEnv * ValRef * range
exception ValNotLocal of DisplayEnv * ValRef * range
exception InvalidRuntimeCoercion of DisplayEnv * TType * TType * range
exception IndeterminateRuntimeCoercion of DisplayEnv * TType * TType * range
exception IndeterminateStaticCoercion of DisplayEnv * TType * TType * range
exception StaticCoercionShouldUseBox of DisplayEnv * TType * TType * range
exception RuntimeCoercionSourceSealed of DisplayEnv * TType * range
exception CoercionTargetSealed of DisplayEnv * TType * range
exception UpcastUnnecessary of range
exception TypeTestUnnecessary of range
exception SelfRefObjCtor of bool * range
exception VirtualAugmentationOnNullValuedType of range
exception NonVirtualAugmentationOnNullValuedType of range
exception UseOfAddressOfOperator of range
exception DeprecatedThreadStaticBindingWarning of range
exception NotUpperCaseConstructor of range
exception IntfImplInIntrinsicAugmentation of range
exception IntfImplInExtrinsicAugmentation of range
exception OverrideInIntrinsicAugmentation of range
exception OverrideInExtrinsicAugmentation of range
exception NonUniqueInferredAbstractSlot of TcGlobals * DisplayEnv * string * MethInfo * MethInfo * range
exception StandardOperatorRedefinitionWarning of string * range
exception ParameterlessStructCtor of range
exception InvalidInternalsVisibleToAssemblyName of (*badName*)string * (*fileName option*) string option

val TcFieldInit : range -> ILFieldInit -> Tast.Const

val IsSecurityAttribute : TcGlobals -> ImportMap -> Dictionary<Stamp,bool> -> Attrib -> range -> bool
val IsSecurityCriticalAttribute : TcGlobals -> Attrib -> bool
val LightweightTcValForUsingInBuildMethodCall : g : TcGlobals -> vref:ValRef -> vrefFlags : ValUseFlag -> vrefTypeInst : TTypes -> m : range -> Expr * TType