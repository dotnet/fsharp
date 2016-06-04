// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// -------------------------------------------------------------------- 
// Compiler use only.  Erase discriminated unions.
// -------------------------------------------------------------------- 

module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.EraseUnions

open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types

val mkNewData : ILGlobals -> IlxUnionSpec * int -> ILInstr list
val mkIsData : ILGlobals -> bool * IlxUnionSpec * int -> ILInstr list
val mkLdData : bool * IlxUnionSpec * int * int -> ILInstr list
val mkStData : IlxUnionSpec * int * int -> ILInstr list
val mkBrIsData : ILGlobals -> avoidHelpers:bool * IlxUnionSpec * int * ILCodeLabel * ILCodeLabel -> ILInstr list
val ConvModule: ILGlobals -> ILModuleDef -> ILModuleDef 
val GetILTypeForAlternative : IlxUnionSpec -> int -> ILType
