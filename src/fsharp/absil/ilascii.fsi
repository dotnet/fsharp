// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Various constants and utilities used when parsing the ILASM format for IL
module internal FSharp.Compiler.AbstractIL.AsciiConstants

open FSharp.Compiler.AbstractIL.IL 

// -------------------------------------------------------------------- 
// IL Lexer and pretty-printer tables
// -------------------------------------------------------------------- 

type NoArgInstr = unit -> ILInstr
type Int32Instr = int32 -> ILInstr
type Int32Int32Instr = int32 * int32 -> ILInstr
type Int64Instr = int64 -> ILInstr
type DoubleInstr = ILConst -> ILInstr
type MethodSpecInstr = ILMethodSpec * ILVarArgs -> ILInstr
type TypeInstr = ILType -> ILInstr
type IntTypeInstr = int * ILType -> ILInstr
type ValueTypeInstr = ILType -> ILInstr
type StringInstr = string -> ILInstr
type TokenInstr = ILToken -> ILInstr
type SwitchInstr = ILCodeLabel list * ILCodeLabel -> ILInstr

type InstrTable<'T> = (string list * 'T) list
type LazyInstrTable<'T> = Lazy<InstrTable<'T>>

val NoArgInstrs:  LazyInstrTable<NoArgInstr>
val Int64Instrs:  LazyInstrTable<Int64Instr>
val Int32Instrs:  LazyInstrTable<Int32Instr>
val Int32Int32Instrs: LazyInstrTable<Int32Int32Instr>
val DoubleInstrs:  LazyInstrTable<DoubleInstr>
val StringInstrs:  LazyInstrTable<StringInstr>
val TokenInstrs:  LazyInstrTable<TokenInstr>
val TypeInstrs:  LazyInstrTable<TypeInstr>
val IntTypeInstrs:  LazyInstrTable<IntTypeInstr>
val ValueTypeInstrs:  LazyInstrTable<ValueTypeInstr>

#if DEBUG
val wordsOfNoArgInstr : (ILInstr -> string list)
val isNoArgInstr : (ILInstr -> bool)
#endif



