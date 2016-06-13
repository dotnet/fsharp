// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// -------------------------------------------------------------------- 
// Compiler use only.  Erase discriminated unions.
// -------------------------------------------------------------------- 

module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.EraseUnions

open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types

/// Make the instruction sequence for an ILX "newdata" instruction
val mkNewData : ILGlobals -> IlxUnionSpec * int -> ILInstr list

/// Make the instruction sequence for an ILX "isdata" instruction
val mkIsData : ILGlobals -> bool * IlxUnionSpec * int -> ILInstr list

/// Make the instruction sequence for an ILX "lddata" instruction
val mkLdData : bool * IlxUnionSpec * int * int -> ILInstr list

/// Make the instruction sequence for an ILX "stdata" instruction
val mkStData : IlxUnionSpec * int * int -> ILInstr list

/// Make the instruction sequence for an ILX "brisnotdata" instruction
val mkBrIsNotData : ILGlobals -> avoidHelpers:bool * IlxUnionSpec * int * ILCodeLabel -> ILInstr list

/// Make the type definition for a union type
val mkClassUnionDef : ILGlobals -> ILTypeRef -> ILTypeDef -> IlxUnionInfo -> ILTypeDef

/// Make the IL type for a union type alternative
val GetILTypeForAlternative : IlxUnionSpec -> int -> ILType

/// Used to emit instructions (an interface to the IlxGen.fs code generator)
type ICodeGen<'Mark> = 
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark  -> unit
    abstract EmitInstr : ILInstr -> unit
    abstract EmitInstrs : ILInstr list -> unit

/// Emit the instruction sequence for an ILX "castdata" instruction
val emitCastData : ILGlobals -> ICodeGen<'Mark> -> canfail: bool * IlxUnionSpec * int -> unit

/// Emit the instruction sequence for an ILX "lddatatag" instruction
val emitLdDataTag : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec -> unit

/// Emit the instruction sequence for an ILX "switchdata" instruction
val emitDataSwitch : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec * (int * ILCodeLabel) list -> unit
