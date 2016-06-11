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
val mkBrIsNotData : ILGlobals -> avoidHelpers:bool * IlxUnionSpec * int * ILCodeLabel -> ILInstr list
val mkClassUnionDef : ILGlobals -> ILTypeRef -> ILTypeDef -> IlxUnionInfo -> ILTypeDef
val GetILTypeForAlternative : IlxUnionSpec -> int -> ILType

type ICodeGen<'Mark> = 
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark  -> unit
    abstract EmitInstr : ILInstr -> unit
    abstract EmitInstrs : ILInstr list -> unit

val emitCastData : ILGlobals -> ICodeGen<'Mark> -> canfail: bool * IlxUnionSpec * int -> unit
val emitLdDataTag : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec -> unit
val emitDataSwitch : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec * (int * ILCodeLabel) list -> unit
