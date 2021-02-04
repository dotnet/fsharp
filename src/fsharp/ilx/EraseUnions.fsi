// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// -------------------------------------------------------------------- 
// Compiler use only.  Erase discriminated unions.
// -------------------------------------------------------------------- 

module internal FSharp.Compiler.AbstractIL.ILX.EraseUnions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

/// Make the instruction sequence for a "newdata" operation
val mkNewData : ILGlobals -> IlxUnionSpec * int -> ILInstr list

/// Make the instruction sequence for a "isdata" operation
val mkIsData : ILGlobals -> bool * IlxUnionSpec * int -> ILInstr list

/// Make the instruction sequence for a "lddata" operation
val mkLdData : bool * IlxUnionSpec * int * int -> ILInstr list

/// Make the instruction sequence for a "lddataa" operation
val mkLdDataAddr : bool * IlxUnionSpec * int * int -> ILInstr list

/// Make the instruction sequence for a "stdata" operation
val mkStData : IlxUnionSpec * int * int -> ILInstr list

/// Make the instruction sequence for a "brisnotdata" operation
val mkBrIsData : ILGlobals -> sense: bool -> avoidHelpers:bool * IlxUnionSpec * int * ILCodeLabel -> ILInstr list

/// Make the type definition for a union type
val mkClassUnionDef : addMethodGeneratedAttrs:(ILMethodDef -> ILMethodDef) * addPropertyGeneratedAttrs:(ILPropertyDef -> ILPropertyDef) * addPropertyNeverAttrs:(ILPropertyDef -> ILPropertyDef) * addFieldGeneratedAttrs:(ILFieldDef -> ILFieldDef) * addFieldNeverAttrs:(ILFieldDef -> ILFieldDef) * mkDebuggerTypeProxyAttribute:(ILType -> ILAttribute) -> ilg:ILGlobals -> tref:ILTypeRef -> td:ILTypeDef -> cud:IlxUnionInfo -> ILTypeDef    

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
    abstract MkInvalidCastExnNewobj : unit -> ILInstr

/// Emit the instruction sequence for a "castdata" operation
val emitCastData : ILGlobals -> ICodeGen<'Mark> -> canfail: bool * avoidHelpers:bool * IlxUnionSpec * int -> unit

/// Emit the instruction sequence for a "lddatatag" operation
val emitLdDataTag : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec -> unit

/// Emit the instruction sequence for a "switchdata" operation
val emitDataSwitch : ILGlobals -> ICodeGen<'Mark> -> avoidHelpers:bool * IlxUnionSpec * (int * ILCodeLabel) list -> unit
