// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Compiler use only.  Erase closures
module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.EraseClosures

open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types

val ConvModule: ILGlobals -> ILModuleDef -> ILModuleDef 

type cenv
val mkILFuncTy : cenv -> ILType -> ILType -> ILType
val mkILTyFuncTy : cenv -> ILType
val new_cenv : ILGlobals -> cenv
val mkTyOfLambdas: cenv -> IlxClosureLambdas -> ILType
