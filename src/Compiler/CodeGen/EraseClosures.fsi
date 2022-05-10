// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Compiler use only.  Erase closures
module internal FSharp.Compiler.AbstractIL.ILX.EraseClosures

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

type cenv

val mkCallFunc:
    cenv -> allocLocal: (ILType -> uint16) -> numThisGenParams: int -> ILTailcall -> IlxClosureApps -> ILInstr list

val mkILFuncTy: cenv -> ILType -> ILType -> ILType

val mkILTyFuncTy: cenv -> ILType

val newIlxPubCloEnv:
    ilg: ILGlobals *
    addMethodGeneratedAttrs: (ILMethodDef -> ILMethodDef) *
    addFieldGeneratedAttrs: (ILFieldDef -> ILFieldDef) *
    addFieldNeverAttrs: (ILFieldDef -> ILFieldDef) ->
        cenv

val mkTyOfLambdas: cenv -> IlxClosureLambdas -> ILType

val convIlxClosureDef: cenv -> encl: string list -> ILTypeDef -> IlxClosureInfo -> ILTypeDef list
