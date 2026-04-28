// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.TypedTreeOps

open System.Collections.Generic
open Internal.Utilities.Collections
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

[<AutoOpen>]
module internal FreeTypeVars =

    val emptyFreeLocals: FreeLocals

    val unionFreeLocals: FreeLocals -> FreeLocals -> FreeLocals

    val emptyFreeRecdFields: Zset<RecdFieldRef>

    val unionFreeRecdFields: Zset<RecdFieldRef> -> Zset<RecdFieldRef> -> Zset<RecdFieldRef>

    val emptyFreeUnionCases: Zset<UnionCaseRef>

    val unionFreeUnionCases: Zset<UnionCaseRef> -> Zset<UnionCaseRef> -> Zset<UnionCaseRef>

    val emptyFreeTycons: FreeTycons

    val unionFreeTycons: FreeTycons -> FreeTycons -> FreeTycons

    /// An ordering for type parameters, based on stamp
    val typarOrder: IComparer<Typar>

    val emptyFreeTypars: FreeTypars

    val unionFreeTypars: FreeTypars -> FreeTypars -> FreeTypars

    val emptyFreeTyvars: FreeTyvars

    val isEmptyFreeTyvars: FreeTyvars -> bool

    val unionFreeTyvars: FreeTyvars -> FreeTyvars -> FreeTyvars

    /// Represents the options to activate when collecting free variables
    type FreeVarOptions =
        { canCache: bool
          collectInTypes: bool
          includeLocalTycons: bool
          includeTypars: bool
          includeLocalTyconReprs: bool
          includeRecdFields: bool
          includeUnionCases: bool
          includeLocals: bool
          templateReplacement: ((TyconRef -> bool) * Typars) option
          stackGuard: StackGuard option }

        /// During backend code generation of state machines, register a template replacement for struct types.
        /// This may introduce new free variables related to the instantiation of the struct type.
        member WithTemplateReplacement: (TyconRef -> bool) * Typars -> FreeVarOptions

    val CollectLocalsNoCaching: FreeVarOptions

    val CollectTyparsNoCaching: FreeVarOptions

    val CollectTyparsAndLocalsNoCaching: FreeVarOptions

    val CollectTyparsAndLocals: FreeVarOptions

    val CollectLocals: FreeVarOptions

    val CollectLocalsWithStackGuard: unit -> FreeVarOptions

    val CollectTyparsAndLocalsWithStackGuard: unit -> FreeVarOptions

    val CollectTypars: FreeVarOptions

    val CollectAllNoCaching: FreeVarOptions

    val CollectAll: FreeVarOptions

    val accFreeInTypes: FreeVarOptions -> TType list -> FreeTyvars -> FreeTyvars

    val accFreeInType: FreeVarOptions -> TType -> FreeTyvars -> FreeTyvars

    val accFreeTycon: FreeVarOptions -> TyconRef -> FreeTyvars -> FreeTyvars

    val boundTypars: FreeVarOptions -> Typars -> FreeTyvars -> FreeTyvars

    val accFreeInTrait: FreeVarOptions -> TraitConstraintInfo -> FreeTyvars -> FreeTyvars

    val accFreeInTraitSln: FreeVarOptions -> TraitConstraintSln -> FreeTyvars -> FreeTyvars

    val accFreeInTupInfo: FreeVarOptions -> TupInfo -> FreeTyvars -> FreeTyvars

    val accFreeInVal: FreeVarOptions -> Val -> FreeTyvars -> FreeTyvars

    val accFreeInTypars: FreeVarOptions -> Typars -> FreeTyvars -> FreeTyvars

    val freeInType: FreeVarOptions -> TType -> FreeTyvars

    val freeInTypes: FreeVarOptions -> TType list -> FreeTyvars

    val freeInVal: FreeVarOptions -> Val -> FreeTyvars

    // This one puts free variables in canonical left-to-right order.
    val freeInTypeLeftToRight: TcGlobals -> bool -> TType -> Typars

    val freeInTypesLeftToRight: TcGlobals -> bool -> TType list -> Typars

    val freeInTypesLeftToRightSkippingConstraints: TcGlobals -> TType list -> Typars

    val freeInModuleTy: ModuleOrNamespaceType -> FreeTyvars

[<AutoOpen>]
module internal MemberRepresentation =

    val GetMemberTypeInFSharpForm:
        TcGlobals -> SynMemberFlags -> ValReprInfo -> TType -> range -> Typars * CurriedArgInfos * TType * ArgReprInfo

    val checkMemberValRef: ValRef -> ValMemberInfo * ValReprInfo

    val generalTyconRefInst: TyconRef -> TypeInst

    val generalizeTyconRef: TcGlobals -> TyconRef -> TTypes * TType

    val generalizedTyconRef: TcGlobals -> TyconRef -> TType

    val GetValReprTypeInCompiledForm:
        TcGlobals ->
        ValReprInfo ->
        int ->
        TType ->
        range ->
            Typars * TraitWitnessInfos * CurriedArgInfos * TType option * ArgReprInfo

    val GetFSharpViewOfReturnType: TcGlobals -> TType option -> TType

    //-------------------------------------------------------------------------
    // Members
    //-------------------------------------------------------------------------

    val GetTypeOfMemberInFSharpForm: TcGlobals -> ValRef -> Typars * CurriedArgInfos * TType * ArgReprInfo

    val GetTypeOfMemberInMemberForm:
        TcGlobals -> ValRef -> Typars * TraitWitnessInfos * CurriedArgInfos * TType option * ArgReprInfo

    val GetMemberTypeInMemberForm:
        TcGlobals ->
        SynMemberFlags ->
        ValReprInfo ->
        int ->
        TType ->
        range ->
            Typars * TraitWitnessInfos * CurriedArgInfos * TType option * ArgReprInfo

    /// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
    val PartitionValTyparsForApparentEnclosingType:
        TcGlobals -> Val -> (Typars * Typars * Typars * TyparInstantiation * TType list) option

    /// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
    val PartitionValTypars: TcGlobals -> Val -> (Typars * Typars * Typars * TyparInstantiation * TType list) option

    /// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
    val PartitionValRefTypars:
        TcGlobals -> ValRef -> (Typars * Typars * Typars * TyparInstantiation * TType list) option

    /// Count the number of type parameters on the enclosing type
    val CountEnclosingTyparsOfActualParentOfVal: Val -> int

    val ReturnTypeOfPropertyVal: TcGlobals -> Val -> TType

    val ArgInfosOfPropertyVal: TcGlobals -> Val -> UncurriedArgInfos

    val ArgInfosOfMember: TcGlobals -> ValRef -> CurriedArgInfos

    /// Check if the order of defined typars is different from the order of used typars in the curried arguments.
    val isTyparOrderMismatch: Typars -> CurriedArgInfos -> bool

    //-------------------------------------------------------------------------
    // Printing
    //-------------------------------------------------------------------------

    type TyparConstraintsWithTypars = (Typar * TyparConstraint) list

    module PrettyTypes =

        val NeedsPrettyTyparName: Typar -> bool

        val NewPrettyTypars: TyparInstantiation -> Typars -> string list -> Typars * TyparInstantiation

        val PrettyTyparNames: (Typar -> bool) -> string list -> Typars -> string list

        /// Assign previously generated pretty names to typars
        val AssignPrettyTyparNames: Typars -> string list -> unit

        val PrettifyType: TcGlobals -> TType -> TType * TyparConstraintsWithTypars

        val PrettifyInstAndTyparsAndType:
            TcGlobals ->
            TyparInstantiation * Typars * TType ->
                (TyparInstantiation * Typars * TType) * TyparConstraintsWithTypars

        val PrettifyTypePair: TcGlobals -> TType * TType -> (TType * TType) * TyparConstraintsWithTypars

        val PrettifyTypes: TcGlobals -> TTypes -> TTypes * TyparConstraintsWithTypars

        /// same as PrettifyTypes, but allows passing the types along with a discriminant value
        /// useful to prettify many types that need to be sorted out after prettifying operation
        /// took place.
        val PrettifyDiscriminantAndTypePairs:
            TcGlobals -> ('Discriminant * TType) list -> ('Discriminant * TType) list * TyparConstraintsWithTypars

        val PrettifyInst: TcGlobals -> TyparInstantiation -> TyparInstantiation * TyparConstraintsWithTypars

        val PrettifyInstAndType:
            TcGlobals -> TyparInstantiation * TType -> (TyparInstantiation * TType) * TyparConstraintsWithTypars

        val PrettifyInstAndTypes:
            TcGlobals -> TyparInstantiation * TTypes -> (TyparInstantiation * TTypes) * TyparConstraintsWithTypars

        val PrettifyInstAndSig:
            TcGlobals ->
            TyparInstantiation * TTypes * TType ->
                (TyparInstantiation * TTypes * TType) * TyparConstraintsWithTypars

        val PrettifyCurriedTypes: TcGlobals -> TType list list -> TType list list * TyparConstraintsWithTypars

        val PrettifyCurriedSigTypes:
            TcGlobals -> TType list list * TType -> (TType list list * TType) * TyparConstraintsWithTypars

        val PrettifyInstAndUncurriedSig:
            TcGlobals ->
            TyparInstantiation * UncurriedArgInfos * TType ->
                (TyparInstantiation * UncurriedArgInfos * TType) * TyparConstraintsWithTypars

        val PrettifyInstAndCurriedSig:
            TcGlobals ->
            TyparInstantiation * TTypes * CurriedArgInfos * TType ->
                (TyparInstantiation * TTypes * CurriedArgInfos * TType) * TyparConstraintsWithTypars

    /// Describes how generic type parameters in a type will be formatted during printing
    type GenericParameterStyle =
        /// Use the IsPrefixDisplay member of the TyCon to determine the style
        | Implicit
        /// Force the prefix style: List<int>
        | Prefix
        /// Force the suffix style: int List
        | Suffix
        /// Force the prefix style for a top-level type,
        /// for example, `seq<int list>` instead of `int list seq`
        | TopLevelPrefix of nested: GenericParameterStyle

    type DisplayEnv =
        {
            includeStaticParametersInTypeNames: bool
            openTopPathsSorted: InterruptibleLazy<string list list>
            openTopPathsRaw: string list list
            shortTypeNames: bool
            suppressNestedTypes: bool
            maxMembers: int option
            showObsoleteMembers: bool
            showHiddenMembers: bool
            showTyparBinding: bool
            showInferenceTyparAnnotations: bool
            suppressInlineKeyword: bool
            suppressMutableKeyword: bool
            showMemberContainers: bool
            shortConstraints: bool
            useColonForReturnType: bool
            showAttributes: bool
            showCsharpCodeAnalysisAttributes: bool
            showOverrides: bool
            showStaticallyResolvedTyparAnnotations: bool
            showNullnessAnnotations: bool option
            abbreviateAdditionalConstraints: bool
            showTyparDefaultConstraints: bool
            /// If set, signatures will be rendered with XML documentation comments for members if they exist
            /// Defaults to false, expected use cases include things like signature file generation.
            showDocumentation: bool
            shrinkOverloads: bool
            printVerboseSignatures: bool
            escapeKeywordNames: bool
            g: TcGlobals
            contextAccessibility: Accessibility
            generatedValueLayout: Val -> Layout option
            genericParameterStyle: GenericParameterStyle
        }

        member SetOpenPaths: string list list -> DisplayEnv

        static member Empty: TcGlobals -> DisplayEnv

        member AddAccessibility: Accessibility -> DisplayEnv

        member AddOpenPath: string list -> DisplayEnv

        member AddOpenModuleOrNamespace: ModuleOrNamespaceRef -> DisplayEnv

        member UseGenericParameterStyle: GenericParameterStyle -> DisplayEnv

        member UseTopLevelPrefixGenericParameterStyle: unit -> DisplayEnv

        static member InitialForSigFileGeneration: TcGlobals -> DisplayEnv

    val tagEntityRefName: xref: EntityRef -> name: string -> TaggedText

    /// Return the full text for an item as we want it displayed to the user as a fully qualified entity
    val fullDisplayTextOfModRef: ModuleOrNamespaceRef -> string

    val fullDisplayTextOfParentOfModRef: ModuleOrNamespaceRef -> string voption

    val fullDisplayTextOfValRef: ValRef -> string

    val fullDisplayTextOfValRefAsLayout: ValRef -> Layout

    val fullDisplayTextOfTyconRef: TyconRef -> string

    val fullDisplayTextOfTyconRefAsLayout: TyconRef -> Layout

    val fullDisplayTextOfExnRef: TyconRef -> string

    val fullDisplayTextOfExnRefAsLayout: TyconRef -> Layout

    val fullDisplayTextOfUnionCaseRef: UnionCaseRef -> string

    val fullDisplayTextOfRecdFieldRef: RecdFieldRef -> string

    val fullMangledPathToTyconRef: TyconRef -> string array

    /// A unique qualified name for each type definition, used to qualify the names of interface implementation methods
    val qualifiedMangledNameOfTyconRef: TyconRef -> string -> string

    val qualifiedInterfaceImplementationName: TcGlobals -> TType -> string -> string

    val trimPathByDisplayEnv: DisplayEnv -> string list -> string

    val prefixOfStaticReq: TyparStaticReq -> string

    val prefixOfInferenceTypar: Typar -> string

    /// Utilities used in simplifying types for visual presentation
    module SimplifyTypes =

        type TypeSimplificationInfo =
            { singletons: Typar Zset
              inplaceConstraints: Zmap<Typar, TType>
              postfixConstraints: TyparConstraintsWithTypars }

        val typeSimplificationInfo0: TypeSimplificationInfo

        val CollectInfo: bool -> TType list -> TyparConstraintsWithTypars -> TypeSimplificationInfo

    val superOfTycon: TcGlobals -> Tycon -> TType

    val GetTraitConstraintInfosOfTypars: TcGlobals -> Typars -> TraitConstraintInfo list

    val GetTraitWitnessInfosOfTypars: TcGlobals -> numParentTypars: int -> typars: Typars -> TraitWitnessInfos

    type TraitConstraintInfo with

        /// Get the argument types recorded in the member constraint suitable for building a TypedTree call.
        member GetCompiledArgumentTypes: unit -> TType list

        /// Get the argument types when the trait is used as a first-class value "^T.TraitName" which can then be applied
        member GetLogicalArgumentTypes: g: TcGlobals -> TType list

        member GetObjectType: unit -> TType option

        member GetReturnType: g: TcGlobals -> TType

        /// Get the name of the trait for textual call.
        member MemberDisplayNameCore: string

        /// Get the key associated with the member constraint.
        member GetWitnessInfo: unit -> TraitWitnessInfo
