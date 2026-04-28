// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.Attributes: IL extensions, attribute helpers, and debug printing.
namespace FSharp.Compiler.TypedTreeOps

open System.Collections.Generic
open Internal.Utilities.Library
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics

[<AutoOpen>]
module internal ILExtensions =

    val isILAttribByName: string list * string -> ILAttribute -> bool

    val TryDecodeILAttribute: ILTypeRef -> ILAttributes -> (ILAttribElem list * ILAttributeNamedArg list) option

    val IsILAttrib: BuiltinAttribInfo -> ILAttribute -> bool

    val TryFindILAttribute: BuiltinAttribInfo -> ILAttributes -> bool

    val inline hasFlag: flags: ^F -> flag: ^F -> bool when ^F: enum<uint64>

    /// Compute well-known attribute flags for an ILAttributes collection.
    val classifyILAttrib: attr: ILAttribute -> WellKnownILAttributes

    val computeILWellKnownFlags: _g: TcGlobals -> attrs: ILAttributes -> WellKnownILAttributes

    val tryFindILAttribByFlag:
        flag: WellKnownILAttributes -> cattrs: ILAttributes -> (ILAttribElem list * ILAttributeNamedArg list) option

    [<return: Struct>]
    val (|ILAttribDecoded|_|):
        flag: WellKnownILAttributes -> cattrs: ILAttributes -> (ILAttribElem list * ILAttributeNamedArg list) voption

    type ILAttributesStored with

        member HasWellKnownAttribute: g: TcGlobals * flag: WellKnownILAttributes -> bool

    type ILTypeDef with

        member HasWellKnownAttribute: g: TcGlobals * flag: WellKnownILAttributes -> bool

    type ILMethodDef with

        member HasWellKnownAttribute: g: TcGlobals * flag: WellKnownILAttributes -> bool

    type ILFieldDef with

        member HasWellKnownAttribute: g: TcGlobals * flag: WellKnownILAttributes -> bool

    type ILAttributes with

        /// Non-caching (unlike ILAttributesStored.HasWellKnownAttribute which caches).
        member HasWellKnownAttribute: flag: WellKnownILAttributes -> bool

    val IsMatchingFSharpAttribute: TcGlobals -> BuiltinAttribInfo -> Attrib -> bool

    val HasFSharpAttribute: TcGlobals -> BuiltinAttribInfo -> Attribs -> bool

    val TryFindFSharpAttribute: TcGlobals -> BuiltinAttribInfo -> Attribs -> Attrib option

    [<return: Struct>]
    val (|ExtractAttribNamedArg|_|): string -> AttribNamedArg list -> AttribExpr voption

    [<return: Struct>]
    val (|ExtractILAttributeNamedArg|_|): string -> ILAttributeNamedArg list -> ILAttribElem voption

    [<return: Struct>]
    val (|StringExpr|_|): (Expr -> string voption)

    [<return: Struct>]
    val (|AttribInt32Arg|_|): (AttribExpr -> int32 voption)

    [<return: Struct>]
    val (|AttribInt16Arg|_|): (AttribExpr -> int16 voption)

    [<return: Struct>]
    val (|AttribBoolArg|_|): (AttribExpr -> bool voption)

    [<return: Struct>]
    val (|AttribStringArg|_|): (AttribExpr -> string voption)

    val (|AttribElemStringArg|_|): (ILAttribElem -> string option)

[<AutoOpen>]
module internal AttributeHelpers =

    val computeEntityWellKnownFlags: g: TcGlobals -> attribs: Attribs -> WellKnownEntityAttributes

    /// Classify a single entity-level attrib to its well-known flag (or None).
    val classifyEntityAttrib: g: TcGlobals -> attrib: Attrib -> WellKnownEntityAttributes

    /// Classify a single val-level attrib to its well-known flag (or None).
    val classifyValAttrib: g: TcGlobals -> attrib: Attrib -> WellKnownValAttributes

    /// Classify a single assembly-level attrib to its well-known flag (or None).
    val classifyAssemblyAttrib: g: TcGlobals -> attrib: Attrib -> WellKnownAssemblyAttributes

    /// Check if an Entity has a specific well-known attribute, computing and caching flags if needed.
    val attribsHaveEntityFlag: g: TcGlobals -> flag: WellKnownEntityAttributes -> attribs: Attribs -> bool

    val filterOutWellKnownAttribs:
        g: TcGlobals ->
        entityMask: WellKnownEntityAttributes ->
        valMask: WellKnownValAttributes ->
        attribs: Attribs ->
            Attribs

    val tryFindEntityAttribByFlag: g: TcGlobals -> flag: WellKnownEntityAttributes -> attribs: Attribs -> Attrib option

    [<return: Struct>]
    val (|EntityAttrib|_|): g: TcGlobals -> flag: WellKnownEntityAttributes -> attribs: Attribs -> Attrib voption

    [<return: Struct>]
    val (|EntityAttribInt|_|): g: TcGlobals -> flag: WellKnownEntityAttributes -> attribs: Attribs -> int voption

    [<return: Struct>]
    val (|EntityAttribString|_|): g: TcGlobals -> flag: WellKnownEntityAttributes -> attribs: Attribs -> string voption

    val attribsHaveValFlag: g: TcGlobals -> flag: WellKnownValAttributes -> attribs: Attribs -> bool

    val tryFindValAttribByFlag: g: TcGlobals -> flag: WellKnownValAttributes -> attribs: Attribs -> Attrib option

    [<return: Struct>]
    val (|ValAttrib|_|): g: TcGlobals -> flag: WellKnownValAttributes -> attribs: Attribs -> Attrib voption

    [<return: Struct>]
    val (|ValAttribInt|_|): g: TcGlobals -> flag: WellKnownValAttributes -> attribs: Attribs -> int voption

    [<return: Struct>]
    val (|ValAttribString|_|): g: TcGlobals -> flag: WellKnownValAttributes -> attribs: Attribs -> string voption

    val EntityHasWellKnownAttribute: g: TcGlobals -> flag: WellKnownEntityAttributes -> entity: Entity -> bool

    /// Get the computed well-known attribute flags for an entity.
    val GetEntityWellKnownFlags: g: TcGlobals -> entity: Entity -> WellKnownEntityAttributes

    /// Map a WellKnownILAttributes flag to its entity flag + provided-type AttribInfo equivalents.
    val mapILFlag:
        g: TcGlobals -> flag: WellKnownILAttributes -> struct (WellKnownEntityAttributes * BuiltinAttribInfo option)

    val computeValWellKnownFlags: g: TcGlobals -> attribs: Attribs -> WellKnownValAttributes

    /// Check if an ArgReprInfo has a specific well-known attribute, computing and caching flags if needed.
    val ArgReprInfoHasWellKnownAttribute: g: TcGlobals -> flag: WellKnownValAttributes -> argInfo: ArgReprInfo -> bool

    /// Check if a Val has a specific well-known attribute, computing and caching flags if needed.
    val ValHasWellKnownAttribute: g: TcGlobals -> flag: WellKnownValAttributes -> v: Val -> bool

    /// Query a three-state bool attribute on an entity. Returns bool option.
    val EntityTryGetBoolAttribute:
        g: TcGlobals ->
        trueFlag: WellKnownEntityAttributes ->
        falseFlag: WellKnownEntityAttributes ->
        entity: Entity ->
            bool option

    /// Query a three-state bool attribute on a Val. Returns bool option.
    val ValTryGetBoolAttribute:
        g: TcGlobals -> trueFlag: WellKnownValAttributes -> falseFlag: WellKnownValAttributes -> v: Val -> bool option

    /// Try to find a specific attribute on a type definition, where the attribute accepts a string argument.
    ///
    /// This is used to detect the 'DefaultMemberAttribute' and 'ConditionalAttribute' attributes (on type definitions)
    val TryFindTyconRefStringAttribute: TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> string option

    /// Like TryFindTyconRefStringAttribute but with a fast-path flag check on the IL path.
    /// Use this when the attribute has a corresponding WellKnownILAttributes flag for O(1) early exit.
    val TryFindTyconRefStringAttributeFast:
        TcGlobals -> range -> WellKnownILAttributes -> BuiltinAttribInfo -> TyconRef -> string option

    /// Try to find a specific attribute on a type definition, where the attribute accepts a bool argument.
    val TryFindTyconRefBoolAttribute: TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> bool option

    /// Try to find a specific attribute on a type definition
    val TyconRefHasAttribute: TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> bool

    /// Try to find an attribute with a specific full name on a type definition
    val TyconRefHasAttributeByName: range -> string -> TyconRef -> bool

    /// Check if a TyconRef has a well-known attribute, handling both IL and F# metadata with O(1) flag tests.
    val TyconRefHasWellKnownAttribute: g: TcGlobals -> flag: WellKnownILAttributes -> tcref: TyconRef -> bool

    /// Check if a TyconRef has AllowNullLiteralAttribute, returning Some true/Some false/None.
    val TyconRefAllowsNull: g: TcGlobals -> tcref: TyconRef -> bool option

    /// Try to find the AllowMultiple value of the AttributeUsage attribute on a type definition.
    val TryFindAttributeUsageAttribute: TcGlobals -> range -> TyconRef -> bool option

    val (|AttribBitwiseOrExpr|_|): TcGlobals -> Expr -> (Expr * Expr) voption

    [<return: Struct>]
    val (|EnumExpr|_|): TcGlobals -> Expr -> Expr voption

    [<return: Struct>]
    val (|TypeOfExpr|_|): TcGlobals -> Expr -> TType voption

    [<return: Struct>]
    val (|TypeDefOfExpr|_|): TcGlobals -> Expr -> TType voption

    val isNameOfValRef: TcGlobals -> ValRef -> bool

    [<return: Struct>]
    val (|NameOfExpr|_|): TcGlobals -> Expr -> TType voption

    [<return: Struct>]
    val (|SeqExpr|_|): TcGlobals -> Expr -> unit voption

    val HasDefaultAugmentationAttribute: g: TcGlobals -> tcref: TyconRef -> bool

    [<return: Struct>]
    val (|UnopExpr|_|): TcGlobals -> Expr -> (ValRef * Expr) voption

    [<return: Struct>]
    val (|BinopExpr|_|): TcGlobals -> Expr -> (ValRef * Expr * Expr) voption

    [<return: Struct>]
    val (|SpecificUnopExpr|_|): TcGlobals -> ValRef -> Expr -> Expr voption

    [<return: Struct>]
    val (|SpecificBinopExpr|_|): TcGlobals -> ValRef -> Expr -> (Expr * Expr) voption

    [<return: Struct>]
    val (|SignedConstExpr|_|): Expr -> unit voption

    [<return: Struct>]
    val (|IntegerConstExpr|_|): Expr -> unit voption

    [<return: Struct>]
    val (|FloatConstExpr|_|): Expr -> unit voption

    [<return: Struct>]
    val (|UncheckedDefaultOfExpr|_|): TcGlobals -> Expr -> TType voption

    [<return: Struct>]
    val (|SizeOfExpr|_|): TcGlobals -> Expr -> TType voption

    val mkCompilationMappingAttr: TcGlobals -> int -> ILAttribute

    val mkCompilationMappingAttrWithSeqNum: TcGlobals -> int -> int -> ILAttribute

    val mkCompilationMappingAttrWithVariantNumAndSeqNum: TcGlobals -> int -> int -> int -> ILAttribute

    val mkCompilationArgumentCountsAttr: TcGlobals -> int list -> ILAttribute

    val mkCompilationSourceNameAttr: TcGlobals -> string -> ILAttribute

    val mkCompilationMappingAttrForQuotationResource: TcGlobals -> string * ILTypeRef list -> ILAttribute

#if !NO_TYPEPROVIDERS
    /// returns Some(assemblyName) for success
    val TryDecodeTypeProviderAssemblyAttr: ILAttribute -> (string | null) option
#endif

    val IsSignatureDataVersionAttr: ILAttribute -> bool

    val TryFindAutoOpenAttr: ILAttribute -> string option

    val TryFindInternalsVisibleToAttr: ILAttribute -> string option

    val IsMatchingSignatureDataVersionAttr: ILVersionInfo -> ILAttribute -> bool

    val mkSignatureDataVersionAttr: TcGlobals -> ILVersionInfo -> ILAttribute

    val isSealedTy: TcGlobals -> TType -> bool

    val IsUnionTypeWithNullAsTrueValue: TcGlobals -> Tycon -> bool

    val TyconHasUseNullAsTrueValueAttribute: TcGlobals -> Tycon -> bool

    val CanHaveUseNullAsTrueValueAttribute: TcGlobals -> Tycon -> bool

    val ModuleNameIsMangled: TcGlobals -> Attribs -> bool

    val CompileAsEvent: TcGlobals -> Attribs -> bool

    val ValCompileAsEvent: TcGlobals -> Val -> bool

    val MemberIsCompiledAsInstance: TcGlobals -> TyconRef -> bool -> ValMemberInfo -> Attribs -> bool

    val ValSpecIsCompiledAsInstance: TcGlobals -> Val -> bool

    val ValRefIsCompiledAsInstanceMember: TcGlobals -> ValRef -> bool

    val tryFindExtensionAttribute: g: TcGlobals -> attribs: Attrib list -> Attrib option

    /// Add an System.Runtime.CompilerServices.ExtensionAttribute to the module Entity if found via predicate and not already present.
    val tryAddExtensionAttributeIfNotAlreadyPresentForModule:
        g: TcGlobals ->
        tryFindExtensionAttributeIn: ((Attrib list -> Attrib option) -> Attrib option) ->
        moduleEntity: Entity ->
            Entity

    /// Add an System.Runtime.CompilerServices.ExtensionAttribute to the type Entity if found via predicate and not already present.
    val tryAddExtensionAttributeIfNotAlreadyPresentForType:
        g: TcGlobals ->
        tryFindExtensionAttributeIn: ((Attrib list -> Attrib option) -> Attrib option) ->
        moduleOrNamespaceTypeAccumulator: ModuleOrNamespaceType ref ->
        typeEntity: Entity ->
            Entity

[<AutoOpen>]
module internal ByrefAndSpanHelpers =

    val isByrefLikeTyconRef: TcGlobals -> range -> TyconRef -> bool

    val isSpanLikeTyconRef: TcGlobals -> range -> TyconRef -> bool

    val isByrefLikeTy: TcGlobals -> range -> TType -> bool

    /// Check if the type is a byref-like but not a byref.
    val isSpanLikeTy: TcGlobals -> range -> TType -> bool

    val isSpanTy: TcGlobals -> range -> TType -> bool

    val tryDestSpanTy: TcGlobals -> range -> TType -> (TyconRef * TType) option

    val destSpanTy: TcGlobals -> range -> TType -> (TyconRef * TType)

    val isReadOnlySpanTy: TcGlobals -> range -> TType -> bool

    val tryDestReadOnlySpanTy: TcGlobals -> range -> TType -> (TyconRef * TType) option

    val destReadOnlySpanTy: TcGlobals -> range -> TType -> (TyconRef * TType)

module internal DebugPrint =

    /// A global flag indicating whether debug output should include ValReprInfo
    val mutable layoutValReprInfo: bool

    /// A global flag indicating whether debug output should include stamps of Val and Entity
    val mutable layoutStamps: bool

    /// A global flag indicating whether debug output should include ranges
    val mutable layoutRanges: bool

    /// A global flag indicating whether debug output should include type information
    val mutable layoutTypes: bool

    /// Convert a type to a string for debugging purposes
    val showType: TType -> string

    /// Convert an expression to a string for debugging purposes
    val showExpr: Expr -> string

    /// Debug layout for a reference to a value
    val valRefL: ValRef -> Layout

    /// Debug layout for a reference to a union case
    val unionCaseRefL: UnionCaseRef -> Layout

    /// Debug layout for an value definition at its binding site
    val valAtBindL: Val -> Layout

    /// Debug layout for an integer
    val intL: int -> Layout

    /// Debug layout for a value definition
    val valL: Val -> Layout

    /// Debug layout for a type parameter definition
    val typarDeclL: Typar -> Layout

    /// Debug layout for a trait constraint
    val traitL: TraitConstraintInfo -> Layout

    /// Debug layout for a type parameter
    val typarL: Typar -> Layout

    /// Debug layout for a set of type parameters
    val typarsL: Typars -> Layout

    /// Debug layout for a type
    val typeL: TType -> Layout

    /// Debug layout for a method slot signature
    val slotSigL: SlotSig -> Layout

    /// Debug layout for a module or namespace definition
    val entityL: ModuleOrNamespace -> Layout

    /// Debug layout for a binding of an expression to a value
    val bindingL: Binding -> Layout

    /// Debug layout for an expression
    val exprL: Expr -> Layout

    /// Debug layout for a type definition
    val tyconL: Tycon -> Layout

    /// Debug layout for a decision tree
    val decisionTreeL: DecisionTree -> Layout

    /// Debug layout for an implementation file
    val implFileL: CheckedImplFile -> Layout

    /// Debug layout for a list of implementation files
    val implFilesL: CheckedImplFile list -> Layout

    /// Debug layout for class and record fields
    val recdFieldRefL: RecdFieldRef -> Layout

    /// Serialize an entity to a very basic json structure.
    val serializeEntity: path: string -> entity: Entity -> unit
