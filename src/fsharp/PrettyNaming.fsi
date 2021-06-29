// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Some general F# utilities for mangling / unmangling / manipulating names.
/// Anything to do with special names of identifiers and other lexical rules 
module public FSharp.Compiler.PrettyNaming

open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open Internal.Utilities.StructuredFormat

[<LiteralAttribute>]
val parenGet: string = ".()"

[<LiteralAttribute>]
val parenSet: string = ".()<-"

[<LiteralAttribute>]
val qmark: string = "?"

[<LiteralAttribute>]
val qmarkSet: string = "?<-"

/// Prefix for compiled (mangled) operator names.
[<LiteralAttribute>]
val opNamePrefix: string = "op_"

/// Returns `true` if given string is an operator or double backticked name, e.g. ( |>> ) or ( long identifier ).
/// (where ( long identifier ) is the display name for ``long identifier``).
val IsOperatorOrBacktickedName: name:string -> bool

/// Returns `true` if given string is an operator display name, e.g. ( |>> )
val IsOperatorName: name:string -> bool

val IsMangledOpName: n:string -> bool

/// Compiles an operator into a mangled operator name.
/// For example, "!%" becomes "op_DereferencePercent".
/// This function accepts both built-in and custom operators.
val CompileOpName: (string -> string)

/// Decompiles a mangled operator name back into an operator.
/// For example, "op_DereferencePercent" becomes "!%".
/// This function accepts mangled names for both built-in and custom operators.
val DecompileOpName: (string -> string)

val DemangleOperatorName: nm:string -> string

val DemangleOperatorNameAsLayout: nonOpTagged:(string -> #TaggedText) -> nm:string -> Layout

val opNameCons: string

val opNameNil: string

val opNameEquals: string

val opNameEqualsNullable: string

val opNameNullableEquals: string

val opNameNullableEqualsNullable: string

/// The characters that are allowed to be the first character of an identifier.
val IsIdentifierFirstCharacter: c:char -> bool

/// The characters that are allowed to be in an identifier.
val IsIdentifierPartCharacter: c:char -> bool

/// Is this character a part of a long identifier?
val IsLongIdentifierPartCharacter: c:char -> bool

val isTildeOnlyString: s:string -> bool

val IsValidPrefixOperatorUse: s:string -> bool

val IsValidPrefixOperatorDefinitionName: s:string -> bool

val IsPrefixOperator: s:string -> bool

val IsPunctuation: s:string -> bool

val IsTernaryOperator: s:string -> bool

val IsInfixOperator: (string -> bool)

val ( |Control|Equality|Relational|Indexer|FixedTypes|Other| ):
    opName:string -> Choice<unit,unit,unit,unit,unit,unit>

val IsCompilerGeneratedName: nm:string -> bool

val CompilerGeneratedName: nm:string -> string

val GetBasicNameOfPossibleCompilerGeneratedName: name:string -> string

val CompilerGeneratedNameSuffix: basicName:string -> suffix:string -> string

val TryDemangleGenericNameAndPos: n:string -> int voption

type NameArityPair = | NameArityPair of string * int

val DemangleGenericTypeNameWithPos: pos:int -> mangledName:string -> string

val DecodeGenericTypeNameWithPos: pos:int -> mangledName:string -> NameArityPair

val DemangleGenericTypeName: mangledName:string -> string

val DecodeGenericTypeName: mangledName:string -> NameArityPair

/// Try to chop "get_" or "set_" from a string
val TryChopPropertyName: s:string -> string option

/// Try to chop "get_" or "set_" from a string.
/// If the string does not start with "get_" or "set_", this function raises an exception.
val ChopPropertyName: s:string -> string

val SplitNamesForILPath: s:string -> string list

[<LiteralAttribute>]
val FSharpModuleSuffix: string = "Module"

[<LiteralAttribute>]
val MangledGlobalName: string = "`global`"

val IllegalCharactersInTypeAndNamespaceNames: char []

/// Determines if the specified name is a valid name for an active pattern.
val IsActivePatternName: name:string -> bool

type ActivePatternInfo =
    | APInfo of bool * (string * Range.range) list * Range.range
    member ActiveTags: string list
    member ActiveTagsWithRanges: (string * Range.range) list
    member IsTotal: bool
    member Range: Range.range
  
val ActivePatternInfoOfValName: nm:string -> m:Range.range -> ActivePatternInfo option

exception InvalidMangledStaticArg of string

val demangleProvidedTypeName: typeLogicalName:string -> string * (string * string) []

/// Mangle the static parameters for a provided type or method
val mangleProvidedTypeName: typeLogicalName:string * nonDefaultArgs:(string * string) [] -> string

/// Mangle the static parameters for a provided type or method
val computeMangledNameWithoutDefaultArgValues: nm:string * staticArgs:'a [] * defaultArgValues:(string * string option) [] -> string

val outArgCompilerGeneratedName: string

val ExtraWitnessMethodName: nm:string -> string

/// Reuses generated union case field name objects for common field numbers
val mkUnionCaseFieldName: (int -> int -> string)

/// Reuses generated exception field name objects for common field numbers
val mkExceptionFieldName: (int -> string)

/// The prefix of the names used for the fake namespace path added to all dynamic code entries in FSI.EXE
val FsiDynamicModulePrefix: string

module FSharpLib =
    val Root: string
    val RootPath: string list
    val Core: string
    val CorePath: string list

module CustomOperations =
    [<LiteralAttribute>]
    val Into: string = "into"

val unassignedTyparName: string

val FSharpOptimizationDataResourceName: string

val FSharpSignatureDataResourceName: string

val FSharpOptimizationDataResourceName2: string

val FSharpSignatureDataResourceName2: string
