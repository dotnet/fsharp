// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The ILX generator.
module internal FSharp.Compiler.IlxGenSupport

open System.Reflection
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals

val mkLdfldMethodDef:
    ilMethName: string *
    iLAccess: ILMemberAccess *
    isStatic: bool *
    ilTy: ILType *
    ilFieldName: string *
    ilPropType: ILType *
    customAttrs: ILAttribute list ->
        ILMethodDef

val GetDynamicDependencyAttribute: g: TcGlobals -> memberTypes: int32 -> ilType: ILType -> ILAttribute
val GenReadOnlyModReqIfNecessary: g: TcGlobals -> ty: TypedTree.TType -> ilTy: ILType -> ILType
val GenReadOnlyAttributeIfNecessary: g: TcGlobals -> ty: TypedTree.TType -> ILAttribute option
val GetReadOnlyAttribute: g: TcGlobals -> ILAttribute
