// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// --------------------------------------------------------------------
// Compiler use only.  Erase discriminated unions.
// --------------------------------------------------------------------

module internal FSharp.Compiler.AbstractIL.ILX.EraseUnions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types
open FSharp.Compiler.TcGlobals

/// How to access union data at a given call site.
[<RequireQualifiedAccess>]
type DataAccess =
    | RawFields
    | ViaHelpers
    | ViaListHelpers

/// Compute the access strategy from the per-call-site flag and per-union helpers setting.
val computeDataAccess: avoidHelpers: bool -> cuspec: IlxUnionSpec -> DataAccess

/// Make the instruction sequence for a "newdata" operation
val mkNewData: ilg: ILGlobals -> cuspec: IlxUnionSpec * cidx: int -> ILInstr list

/// Make the instruction sequence for a "isdata" operation
val mkIsData: ilg: ILGlobals -> access: DataAccess * cuspec: IlxUnionSpec * cidx: int -> ILInstr list

/// Make the instruction for a "lddata" operation
val mkLdData: access: DataAccess * cuspec: IlxUnionSpec * cidx: int * fidx: int -> ILInstr

/// Make the instruction for a "lddataa" operation
val mkLdDataAddr: access: DataAccess * cuspec: IlxUnionSpec * cidx: int * fidx: int -> ILInstr

/// Make the instruction for a "stdata" operation
val mkStData: cuspec: IlxUnionSpec * cidx: int * fidx: int -> ILInstr

/// Make the instruction sequence for a "brisnotdata" operation
val mkBrIsData:
    ilg: ILGlobals ->
    sense: bool ->
    access: DataAccess * cuspec: IlxUnionSpec * cidx: int * tg: ILCodeLabel ->
        ILInstr list

/// Make the type definition for a union type
val mkClassUnionDef:
    addMethodGeneratedAttrs: (ILMethodDef -> ILMethodDef) *
    addPropertyGeneratedAttrs: (ILPropertyDef -> ILPropertyDef) *
    addPropertyNeverAttrs: (ILPropertyDef -> ILPropertyDef) *
    addFieldGeneratedAttrs: (ILFieldDef -> ILFieldDef) *
    addFieldNeverAttrs: (ILFieldDef -> ILFieldDef) *
    mkDebuggerTypeProxyAttribute: (ILType -> ILAttribute) ->
        g: TcGlobals ->
        tref: ILTypeRef ->
        td: ILTypeDef ->
        cud: IlxUnionInfo ->
            ILTypeDef

/// Make the IL type for a union type alternative
val GetILTypeForAlternative: cuspec: IlxUnionSpec -> alt: int -> ILType

/// Used to emit instructions (an interface to the IlxGen.fs code generator)
type ICodeGen<'Mark> =
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark -> unit
    abstract EmitInstr: ILInstr -> unit
    abstract EmitInstrs: ILInstr list -> unit
    abstract MkInvalidCastExnNewobj: unit -> ILInstr

/// Emit the instruction sequence for a "castdata" operation
val emitCastData:
    ilg: ILGlobals -> cg: ICodeGen<'Mark> -> canfail: bool * access: DataAccess * cuspec: IlxUnionSpec * int -> unit

/// Emit the instruction sequence for a "lddatatag" operation
val emitLdDataTag: ilg: ILGlobals -> cg: ICodeGen<'Mark> -> access: DataAccess * cuspec: IlxUnionSpec -> unit

/// Emit the instruction sequence for a "switchdata" operation
val emitDataSwitch:
    ilg: ILGlobals ->
    cg: ICodeGen<'Mark> ->
    access: DataAccess * cuspec: IlxUnionSpec * cases: (int * ILCodeLabel) list ->
        unit
