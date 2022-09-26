// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Some general F# utilities for mangling / unmangling / manipulating names.
/// Anything to do with special names of identifiers and other lexical rules
module public FSharp.Compiler.Syntax.PrettyNaming

open FSharp.Compiler.Text

[<Literal>]
val internal parenGet: string = ".()"

[<Literal>]
val internal parenSet: string = ".()<-"

[<Literal>]
val internal qmark: string = "?"

[<Literal>]
val internal qmarkSet: string = "?<-"

/// Prefix for compiled (mangled) operator names.
[<Literal>]
val internal opNamePrefix: string = "op_"

/// Returns `true` if given string is an operator display name, e.g.
///    ( |>> )
///    |>>
///    ..
val IsOperatorDisplayName: name: string -> bool

/// Is the name a valid F# identifier, primarily used internally in PrettyNaming.fs for determining if an
/// identifier needs backticks.
///
/// In general do not use this routine. It is only used in one quick fix, for determining if it is valid
/// to add "_" in front of an identifier.
///
///     A            --> true
///     A'           --> true
///     _A           --> true
///     A0           --> true
///     |A|B|        --> false
///     op_Addition  --> true
///     +            --> false
///     let          --> false
///     base         --> false
///
/// TBD: needs unit testing
val IsIdentifierName: name: string -> bool

/// Determines if the specified name is a valid name for an active pattern.
///     |A|_|        --> true
///     |A|B|        --> true
///     |A|          --> true
///     |            --> false
///     ||           --> false
///     op_Addition  --> false
///
/// TBD: needs unit testing
val IsActivePatternName: name: string -> bool

/// Adds double backticks if necessary to make a valid identifier, e.g.
///     op_Addition  -->  op_Addition
///     +            -->  ``+``    (this is not op_Addition)
///     |>>          -->  ``|>>``  (this is not an op_)
///     A-B          -->  ``A-B``
///     AB           -->  AB
///     |A|_|        -->  |A|_|    this is an active pattern name, needs parens not backticks
/// Removes double backticks if not necessary to make a valid identifier, e.g.
///     ``A``        --> A
///     ``A-B``      --> ``A-B``
val NormalizeIdentifierBackticks: name: string -> string

/// Is the name a logical operator name, including unary, binary and ternary operators
///    op_UnaryPlus         - yes
///    op_Addition          - yes
///    op_Range             - yes (?)
///    op_RangeStep         - yes (?)
///    op_DynamicAssignment - yes
///    op_Quack             - no
///    +                    - no
///    ABC                  - no
///    ABC DEF              - no
///    base                 - no
///    |A|_|                - no
val IsLogicalOpName: logicalName: string -> bool

/// Converts the core of an operator name into a logical name. For example,
///    +  --> op_Addition
///    !%  --> op_DereferencePercent
/// Only used on actual operator names
val CompileOpName: op: string -> string

/// Take a core display name (e.g. "List" or "Strange module name") and convert it to display text
/// by adding backticks if necessary.
///     Foo                   --> Foo
///     +                     --> ``+``
///     A-B                   --> ``A-B``
val internal ConvertLogicalNameToDisplayName: name: string -> string

/// Converts the logical name for and operator back into the core of a display name. For example:
///     Foo                   --> Foo
///     +                     --> +
///     op_Addition           --> +
///     op_DereferencePercent --> !%
///     A-B                   --> A-B
///     |A|_|                 --> |A|_|
///     base                  --> base        regardless of IsBaseVal
/// Used on names of all kinds
///
/// TODO: We should assess uses of this function.
///
/// In any cases it is used it probably indicates that text is being
/// generated which:
///    1. does not contain double-backticks for non-identifiers
///    2. does not put parentheses arounf operators or active pattern names
///
/// If the text is immediately in quotes, this is generally ok, e.g.
///
///         error FS0038: '+' is bound twice in this pattern
///         error FS0038: '|A|_|' is bound twice in this pattern
///         error FS0038: 'a a' is bound twice in this pattern
///
/// If not, the it is likely this should be replaced by ConvertValLogicalNameToDisplayName.
val ConvertValLogicalNameToDisplayNameCore: opName: string -> string

/// Take a core display name for a value (e.g. op_Addition or PropertyName) and convert it to display text
///     Foo                   --> Foo
///     +                     --> ``+``
///     op_Addition           --> (+)
///     op_Multiply           --> ( * )
///     op_DereferencePercent --> (!%)
///     A-B                   --> ``A-B``
///     |A|_|                 --> (|A|_|)
///     let                   --> ``let``
///     type                  --> ``type``
///     params                --> ``params``
///     base                  --> base
///     or                    --> or
///     mod                   --> mod
val internal ConvertValLogicalNameToDisplayName: isBaseVal: bool -> name: string -> string

/// Like ConvertLogicalNameToDisplayName but produces a tagged layout
val internal ConvertLogicalNameToDisplayLayout: nonOpLayout: (string -> Layout) -> name: string -> Layout

/// Like ConvertValLogicalNameToDisplayName but produces a tagged layout
val internal ConvertValLogicalNameToDisplayLayout:
    isBaseVal: bool -> nonOpLayout: (string -> Layout) -> name: string -> Layout

val internal opNameCons: string

val internal opNameNil: string

val internal opNameEquals: string

val internal opNameEqualsNullable: string

val internal opNameNullableEquals: string

val internal opNameNullableEqualsNullable: string

/// The characters that are allowed to be the first character of an identifier.
val IsIdentifierFirstCharacter: c: char -> bool

/// The characters that are allowed to be in an identifier.
val IsIdentifierPartCharacter: c: char -> bool

/// Is this character a part of a long identifier?
val IsLongIdentifierPartCharacter: c: char -> bool

val internal isTildeOnlyString: s: string -> bool

val internal IsValidPrefixOperatorUse: s: string -> bool

val internal IsValidPrefixOperatorDefinitionName: s: string -> bool

val IsLogicalPrefixOperator: logicalName: string -> bool

val IsLogicalInfixOpName: logicalName: string -> bool

val IsLogicalTernaryOperator: logicalName: string -> bool

val IsPunctuation: s: string -> bool

val internal (|Control|Equality|Relational|Indexer|FixedTypes|Other|):
    opName: string -> Choice<unit, unit, unit, unit, unit, unit>

val IsCompilerGeneratedName: nm: string -> bool

val internal CompilerGeneratedName: nm: string -> string

val internal GetBasicNameOfPossibleCompilerGeneratedName: name: string -> string

val internal CompilerGeneratedNameSuffix: basicName: string -> suffix: string -> string

val internal TryDemangleGenericNameAndPos: n: string -> int voption

type internal NameArityPair = NameArityPair of string * int

val internal DemangleGenericTypeNameWithPos: pos: int -> mangledName: string -> string

val internal DecodeGenericTypeNameWithPos: pos: int -> mangledName: string -> NameArityPair

val internal DemangleGenericTypeName: mangledName: string -> string

val internal DecodeGenericTypeName: mangledName: string -> NameArityPair

/// Try to chop "get_" or "set_" from a string
val TryChopPropertyName: s: string -> string option

/// Try to chop "get_" or "set_" from a string.
/// If the string does not start with "get_" or "set_", this function raises an exception.
val internal ChopPropertyName: s: string -> string

val internal SplitNamesForILPath: s: string -> string list

[<Literal>]
val internal FSharpModuleSuffix: string = "Module"

[<Literal>]
val internal MangledGlobalName: string = "`global`"

val internal IllegalCharactersInTypeAndNamespaceNames: char[]

type internal ActivePatternInfo =
    | APInfo of bool * (string * range) list * range

    member ActiveTags: string list
    member ActiveTagsWithRanges: (string * range) list
    member IsTotal: bool
    member Range: range

val internal ActivePatternInfoOfValName: nm: string -> m: range -> ActivePatternInfo option

exception internal InvalidMangledStaticArg of string

val internal DemangleProvidedTypeName: typeLogicalName: string -> string * (string * string)[]

/// Mangle the static parameters for a provided type or method
val internal MangleProvidedTypeName: typeLogicalName: string * nonDefaultArgs: (string * string)[] -> string

/// Mangle the static parameters for a provided type or method
val internal ComputeMangledNameWithoutDefaultArgValues:
    nm: string * staticArgs: 'a[] * defaultArgValues: (string * string option)[] -> string

val internal outArgCompilerGeneratedName: string

val internal ExtraWitnessMethodName: nm: string -> string

/// Reuses generated union case field name objects for common field numbers
val internal mkUnionCaseFieldName: (int -> int -> string)

/// Reuses generated exception field name objects for common field numbers
val internal mkExceptionFieldName: (int -> string)

/// The prefix of the names used for the fake namespace path added to all dynamic code entries in FSI.EXE
val FsiDynamicModulePrefix: string

module internal FSharpLib =
    val Root: string
    val RootPath: string list
    val Core: string
    val CorePath: string list

module internal CustomOperations =
    [<Literal>]
    val Into: string = "into"

val internal unassignedTyparName: string

val internal FSharpOptimizationDataResourceName: string

val internal FSharpSignatureDataResourceName: string

val internal FSharpOptimizationCompressedDataResourceName: string

val internal FSharpSignatureCompressedDataResourceName: string

val internal FSharpOptimizationDataResourceName2: string

val internal FSharpSignatureDataResourceName2: string

val GetLongNameFromString: string -> string list

val FormatAndOtherOverloadsString: int -> string

val FSharpSignatureDataResourceName2: string

/// Mark some variables (the ones we introduce via abstractBigTargets) as don't-eliminate
[<Literal>]
val internal suffixForVariablesThatMayNotBeEliminated: string = "$cont"

/// Indicates a ValRef generated to facilitate tuple eliminations
[<Literal>]
val internal suffixForTupleElementAssignmentTarget: string = "$tupleElem"

[<Literal>]
val internal stackVarPrefix: string = "__stack_"

/// Keywords paired with their descriptions. Used in completion and quick info.
val internal keywordsWithDescription: (string * string) list
