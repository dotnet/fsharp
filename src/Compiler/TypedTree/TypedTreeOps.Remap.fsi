// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.TypedTreeOps

open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Rational
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

[<AutoOpen>]
module internal TypeRemapping =

    val inline compareBy: x: ('T | null) -> y: ('T | null) -> func: ('T -> 'K) -> int when 'K: comparison

    /// Maps type parameters to entries based on stamp keys
    [<Sealed>]
    type TyparMap<'T> =

        /// Get the entry for the given type parameter
        member Item: Typar -> 'T with get

        /// Determine is the map contains an entry for the given type parameter
        member ContainsKey: Typar -> bool

        member TryGetValue: Typar -> bool * 'T

        /// Try to find the entry for the given type parameter
        member TryFind: Typar -> 'T option

        /// Make a new map, containing a new entry for the given type parameter
        member Add: Typar * 'T -> TyparMap<'T>

        /// The empty map
        static member Empty: TyparMap<'T>

    /// Maps TyconRef to T based on stamp keys
    [<NoEquality; NoComparison; Sealed>]
    type TyconRefMap<'T> =

        /// Get the entry for the given type definition
        member Item: TyconRef -> 'T with get

        /// Try to find the entry for the given type definition
        member TryFind: TyconRef -> 'T option

        /// Determine is the map contains an entry for the given type definition
        member ContainsKey: TyconRef -> bool

        /// Make a new map, containing a new entry for the given type definition
        member Add: TyconRef -> 'T -> TyconRefMap<'T>

        /// Remove the entry for the given type definition, if any
        member Remove: TyconRef -> TyconRefMap<'T>

        /// Determine if the map is empty
        member IsEmpty: bool

        member TryGetValue: TyconRef -> bool * 'T

        /// The empty map
        static member Empty: TyconRefMap<'T>

        /// Make a new map, containing entries for the given type definitions
        static member OfList: (TyconRef * 'T) list -> TyconRefMap<'T>

    /// Maps Val to T, based on stamps
    [<Struct; NoEquality; NoComparison>]
    type ValMap<'T> =

        member Contents: StampMap<'T>

        member Item: Val -> 'T with get

        member TryFind: Val -> 'T option

        member ContainsVal: Val -> bool

        member Add: Val -> 'T -> ValMap<'T>

        member Remove: Val -> ValMap<'T>

        member IsEmpty: bool

        static member Empty: ValMap<'T>

        static member OfList: (Val * 'T) list -> ValMap<'T>

    /// Represents an instantiation where types replace type parameters
    type TyparInstantiation = (Typar * TType) list

    /// Represents an instantiation where type definition references replace other type definition references
    type TyconRefRemap = TyconRefMap<TyconRef>

    /// Represents an instantiation where value references replace other value references
    type ValRemap = ValMap<ValRef>

    val emptyTyconRefRemap: TyconRefRemap

    val emptyTyparInst: TyparInstantiation

    /// Represents a combination of substitutions/instantiations where things replace other things during remapping
    [<NoEquality; NoComparison>]
    type Remap =
        { tpinst: TyparInstantiation
          valRemap: ValRemap
          tyconRefRemap: TyconRefRemap
          removeTraitSolutions: bool }

        static member Empty: Remap

    val emptyRemap: Remap

    val addTyconRefRemap: TyconRef -> TyconRef -> Remap -> Remap

    val isRemapEmpty: Remap -> bool

    val instTyparRef: tpinst: (Typar * 'a) list -> ty: 'a -> tp: Typar -> 'a

    /// Remap a reference to a type definition using the given remapping substitution
    val remapTyconRef: TyconRefMap<TyconRef> -> TyconRef -> TyconRef

    /// Remap a reference to a union case using the given remapping substitution
    val remapUnionCaseRef: TyconRefMap<TyconRef> -> UnionCaseRef -> UnionCaseRef

    /// Remap a reference to a record field using the given remapping substitution
    val remapRecdFieldRef: TyconRefMap<TyconRef> -> RecdFieldRef -> RecdFieldRef

    val mkTyparInst: Typars -> TTypes -> TyparInstantiation

    val generalizeTypar: Typar -> TType

    /// From typars to types
    val generalizeTypars: Typars -> TypeInst

    val remapTypeAux: Remap -> TType -> TType

    val remapMeasureAux: Remap -> Measure -> Measure

    val remapTupInfoAux: Remap -> TupInfo -> TupInfo

    val remapTypesAux: Remap -> TType list -> TType list

    val remapTyparConstraintsAux: Remap -> TyparConstraint list -> TyparConstraint list

    val remapTraitInfo: Remap -> TraitConstraintInfo -> TraitConstraintInfo

    val bindTypars: tps: 'a list -> tyargs: 'b list -> tpinst: ('a * 'b) list -> ('a * 'b) list

    val copyAndRemapAndBindTyparsFull: (Attrib list -> Attrib list) -> Remap -> Typars -> Typars * Remap

    val copyAndRemapAndBindTypars: Remap -> Typars -> Typars * Remap

    val remapValLinkage: Remap -> ValLinkageFullKey -> ValLinkageFullKey

    val remapNonLocalValRef: Remap -> NonLocalValOrMemberRef -> NonLocalValOrMemberRef

    /// Remap a reference to a value using the given remapping substitution
    val remapValRef: Remap -> ValRef -> ValRef

    val remapType: Remap -> TType -> TType

    val remapTypes: Remap -> TType list -> TType list

    /// Use this one for any type that may be a forall type where the type variables may contain attributes
    val remapTypeFull: (Attrib list -> Attrib list) -> Remap -> TType -> TType

    val remapParam: Remap -> SlotParam -> SlotParam

    val remapSlotSig: (Attrib list -> Attrib list) -> Remap -> SlotSig -> SlotSig

    val mkInstRemap: TyparInstantiation -> Remap

    val instType: TyparInstantiation -> TType -> TType

    val instTypes: TyparInstantiation -> TypeInst -> TypeInst

    val instTrait: TyparInstantiation -> TraitConstraintInfo -> TraitConstraintInfo

    val instTyparConstraints: TyparInstantiation -> TyparConstraint list -> TyparConstraint list

    /// Instantiate the generic type parameters in a method slot signature, building a new one
    val instSlotSig: TyparInstantiation -> SlotSig -> SlotSig

    /// Copy a method slot signature, including new generic type parameters if the slot signature represents a generic method
    val copySlotSig: SlotSig -> SlotSig

    /// Decouple SRTP constraint solution ref cells on typars from shared expression-tree nodes.
    val decoupleTraitSolutions: Typars -> unit

    val mkTyparToTyparRenaming: Typars -> Typars -> TyparInstantiation * TTypes

    val mkTyconInst: Tycon -> TypeInst -> TyparInstantiation

    val mkTyconRefInst: TyconRef -> TypeInst -> TyparInstantiation

[<AutoOpen>]
module internal MeasureOps =

    /// Equality for type definition references
    val tyconRefEq: TcGlobals -> TyconRef -> TyconRef -> bool

    /// Equality for value references
    val valRefEq: TcGlobals -> ValRef -> ValRef -> bool

    val reduceTyconRefAbbrevMeasureable: TyconRef -> Measure

    val stripUnitEqnsFromMeasureAux: bool -> Measure -> Measure

    val stripUnitEqnsFromMeasure: Measure -> Measure

    val MeasureExprConExponent: TcGlobals -> bool -> TyconRef -> Measure -> Rational

    val MeasureConExponentAfterRemapping: TcGlobals -> (TyconRef -> TyconRef) -> TyconRef -> Measure -> Rational

    val MeasureVarExponent: Typar -> Measure -> Rational

    val ListMeasureVarOccs: Measure -> Typar list

    val ListMeasureVarOccsWithNonZeroExponents: Measure -> (Typar * Rational) list

    val ListMeasureConOccsWithNonZeroExponents: TcGlobals -> bool -> Measure -> (TyconRef * Rational) list

    val ListMeasureConOccsAfterRemapping: TcGlobals -> (TyconRef -> TyconRef) -> Measure -> TyconRef list

    val MeasurePower: Measure -> int -> Measure

    val MeasureProdOpt: Measure -> Measure -> Measure

    val ProdMeasures: Measure list -> Measure

    val isDimensionless: TcGlobals -> TType -> bool

    val destUnitParMeasure: TcGlobals -> Measure -> Typar

    val isUnitParMeasure: TcGlobals -> Measure -> bool

    val normalizeMeasure: TcGlobals -> Measure -> Measure

    val tryNormalizeMeasureInType: TcGlobals -> TType -> TType

[<AutoOpen>]
module internal TypeBuilders =

    val mkForallTy: Typars -> TType -> TType

    /// Build a type-forall anonymous generic type if necessary
    val mkForallTyIfNeeded: Typars -> TType -> TType

    val (+->): Typars -> TType -> TType

    /// Build a function type
    val mkFunTy: TcGlobals -> TType -> TType -> TType

    /// Build a curried function type
    val mkIteratedFunTy: TcGlobals -> TTypes -> TType -> TType

    /// Build a nativeptr type
    val mkNativePtrTy: TcGlobals -> TType -> TType

    val mkByrefTy: TcGlobals -> TType -> TType

    /// Make a in-byref type with a in kind parameter
    val mkInByrefTy: TcGlobals -> TType -> TType

    /// Make an out-byref type with an out kind parameter
    val mkOutByrefTy: TcGlobals -> TType -> TType

    val mkByrefTyWithFlag: TcGlobals -> bool -> TType -> TType

    val mkByref2Ty: TcGlobals -> TType -> TType -> TType

    /// Build a 'voidptr' type
    val mkVoidPtrTy: TcGlobals -> TType

    /// Make a byref type with a in/out kind inference parameter
    val mkByrefTyWithInference: TcGlobals -> TType -> TType -> TType

    /// Build an array type of the given rank
    val mkArrayTy: TcGlobals -> int -> Nullness -> TType -> range -> TType

    /// The largest tuple before we start encoding, i.e. 7
    val maxTuple: int

    /// The number of fields in the largest tuple before we start encoding, i.e. 7
    val goodTupleFields: int

    /// Check if a TyconRef is for a .NET tuple type
    val isCompiledTupleTyconRef: TcGlobals -> TyconRef -> bool

    /// Get a TyconRef for a .NET tuple type
    val mkCompiledTupleTyconRef: TcGlobals -> bool -> int -> TyconRef

    /// Convert from F# tuple types to .NET tuple types.
    val mkCompiledTupleTy: TcGlobals -> bool -> TTypes -> TType

    /// Convert from F# tuple types to .NET tuple types, but only the outermost level
    val mkOuterCompiledTupleTy: TcGlobals -> bool -> TTypes -> TType

[<AutoOpen>]
module internal TypeAbbreviations =

    val applyTyconAbbrev: TType -> Tycon -> TypeInst -> TType

    val reduceTyconAbbrev: Tycon -> TypeInst -> TType

    val reduceTyconRefAbbrev: TyconRef -> TypeInst -> TType

    val reduceTyconMeasureableOrProvided: TcGlobals -> Tycon -> TypeInst -> TType

    val reduceTyconRefMeasureableOrProvided: TcGlobals -> TyconRef -> TypeInst -> TType

[<AutoOpen>]
module internal TypeDecomposition =

    val stripTyEqnsA: TcGlobals -> canShortcut: bool -> TType -> TType

    val stripTyEqns: TcGlobals -> TType -> TType

    /// Evaluate the TupInfo to work out if it is a struct or a ref.
    val evalTupInfoIsStruct: TupInfo -> bool

    /// Evaluate the AnonRecdTypeInfo to work out if it is a struct or a ref.
    val evalAnonInfoIsStruct: AnonRecdTypeInfo -> bool

    val stripTyEqnsAndErase: bool -> TcGlobals -> TType -> TType

    val stripTyEqnsAndMeasureEqns: TcGlobals -> TType -> TType

    type Erasure =
        | EraseAll
        | EraseMeasures
        | EraseNone

    /// Reduce a type to its more canonical form subject to an erasure flag, inference equations and abbreviations
    val stripTyEqnsWrtErasure: Erasure -> TcGlobals -> TType -> TType

    /// See through F# exception abbreviations
    val stripExnEqns: TyconRef -> Tycon

    val primDestForallTy: TcGlobals -> TType -> Typars * TType

    val destFunTy: TcGlobals -> TType -> TType * TType

    val destAnyTupleTy: TcGlobals -> TType -> TupInfo * TTypes

    val destRefTupleTy: TcGlobals -> TType -> TTypes

    val destStructTupleTy: TcGlobals -> TType -> TTypes

    val destTyparTy: TcGlobals -> TType -> Typar

    val destAnyParTy: TcGlobals -> TType -> Typar

    val destMeasureTy: TcGlobals -> TType -> Measure

    val destAnonRecdTy: TcGlobals -> TType -> AnonRecdTypeInfo * TTypes

    val destStructAnonRecdTy: TcGlobals -> TType -> TTypes

    val isFunTy: TcGlobals -> TType -> bool

    val isForallTy: TcGlobals -> TType -> bool

    val isAnyTupleTy: TcGlobals -> TType -> bool

    val isRefTupleTy: TcGlobals -> TType -> bool

    val isStructTupleTy: TcGlobals -> TType -> bool

    val isAnonRecdTy: TcGlobals -> TType -> bool

    val isStructAnonRecdTy: TcGlobals -> TType -> bool

    val isUnionTy: TcGlobals -> TType -> bool

    val isStructUnionTy: TcGlobals -> TType -> bool

    val isReprHiddenTy: TcGlobals -> TType -> bool

    val isFSharpObjModelTy: TcGlobals -> TType -> bool

    val isRecdTy: TcGlobals -> TType -> bool

    val isFSharpStructOrEnumTy: TcGlobals -> TType -> bool

    val isFSharpEnumTy: TcGlobals -> TType -> bool

    val isTyparTy: TcGlobals -> TType -> bool

    val isAnyParTy: TcGlobals -> TType -> bool

    val isMeasureTy: TcGlobals -> TType -> bool

    val isProvenUnionCaseTy: TType -> bool

    val mkWoNullAppTy: TyconRef -> TypeInst -> TType

    val mkProvenUnionCaseTy: UnionCaseRef -> TypeInst -> TType

    val isAppTy: TcGlobals -> TType -> bool

    val tryAppTy: TcGlobals -> TType -> (TyconRef * TypeInst) voption

    val destAppTy: TcGlobals -> TType -> TyconRef * TypeInst

    val tcrefOfAppTy: TcGlobals -> TType -> TyconRef

    val argsOfAppTy: TcGlobals -> TType -> TypeInst

    val tryTcrefOfAppTy: TcGlobals -> TType -> TyconRef voption

    /// Returns ValueSome if this type is a type variable, even after abbreviations are expanded and
    /// variables have been solved through unification.
    val tryDestTyparTy: TcGlobals -> TType -> Typar voption

    val tryDestFunTy: TcGlobals -> TType -> (TType * TType) voption

    val tryDestAnonRecdTy: TcGlobals -> TType -> (AnonRecdTypeInfo * TType list) voption

    val tryAnyParTy: TcGlobals -> TType -> Typar voption

    val tryAnyParTyOption: TcGlobals -> TType -> Typar option

    [<return: Struct>]
    val (|AppTy|_|): TcGlobals -> TType -> (TyconRef * TypeInst) voption

    [<return: Struct>]
    val (|RefTupleTy|_|): TcGlobals -> TType -> TTypes voption

    [<return: Struct>]
    val (|FunTy|_|): TcGlobals -> TType -> (TType * TType) voption

    /// Try to get a TyconRef for a type without erasing type abbreviations
    val tryNiceEntityRefOfTy: TType -> TyconRef voption

    val tryNiceEntityRefOfTyOption: TType -> TyconRef option

    val mkInstForAppTy: TcGlobals -> TType -> TyparInstantiation

    val domainOfFunTy: TcGlobals -> TType -> TType

    val rangeOfFunTy: TcGlobals -> TType -> TType

    /// If it is a tuple type, ensure it's outermost type is a .NET tuple type, otherwise leave unchanged
    val convertToTypeWithMetadataIfPossible: TcGlobals -> TType -> TType

    val stripMeasuresFromTy: TcGlobals -> TType -> TType

    val mkAnyTupledTy: TcGlobals -> TupInfo -> TType list -> TType

    val mkAnyAnonRecdTy: TcGlobals -> AnonRecdTypeInfo -> TType list -> TType

    val mkRefTupledTy: TcGlobals -> TType list -> TType

    val mkRefTupledVarsTy: TcGlobals -> Val list -> TType

    val mkMethodTy: TcGlobals -> TType list list -> TType -> TType

    /// Build a single-dimensional array type
    val mkArrayType: TcGlobals -> TType -> TType

    val mkByteArrayTy: TcGlobals -> TType

    val isQuotedExprTy: TcGlobals -> TType -> bool

    val destQuotedExprTy: TcGlobals -> TType -> TType

    val mkQuotedExprTy: TcGlobals -> TType -> TType

    val mkRawQuotedExprTy: TcGlobals -> TType

    val mkIEventType: TcGlobals -> TType -> TType -> TType

    val mkIObservableType: TcGlobals -> TType -> TType

    val mkIObserverType: TcGlobals -> TType -> TType

    val mkSeqTy: TcGlobals -> TType -> TType

    val mkIEnumeratorTy: TcGlobals -> TType -> TType

[<AutoOpen>]
module internal TypeEquivalence =

    [<NoEquality; NoComparison>]
    type TypeEquivEnv =
        { EquivTypars: TyparMap<TType>
          EquivTycons: TyconRefRemap
          NullnessMustEqual: bool }

        static member EmptyIgnoreNulls: TypeEquivEnv
        static member EmptyWithNullChecks: TcGlobals -> TypeEquivEnv

        member BindTyparsToTypes: Typars -> TType list -> TypeEquivEnv

        member BindEquivTypars: Typars -> Typars -> TypeEquivEnv

        member FromTyparInst: TyparInstantiation -> TypeEquivEnv

        member FromEquivTypars: Typars -> Typars -> TypeEquivEnv

        member ResetEquiv: TypeEquivEnv

    val traitsAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TraitConstraintInfo -> TraitConstraintInfo -> bool

    val traitKeysAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TraitWitnessInfo -> TraitWitnessInfo -> bool

    val returnTypesAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TType option -> TType option -> bool

    val typarConstraintsAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TyparConstraint -> TyparConstraint -> bool

    val typarConstraintSetsAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> Typar -> Typar -> bool

    val typarsAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> Typars -> Typars -> bool

    val tcrefAEquiv: TcGlobals -> TypeEquivEnv -> TyconRef -> TyconRef -> bool

    val typeAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TType -> TType -> bool

    val anonInfoEquiv: AnonRecdTypeInfo -> AnonRecdTypeInfo -> bool

    val structnessAEquiv: TupInfo -> TupInfo -> bool

    val measureAEquiv: TcGlobals -> TypeEquivEnv -> Measure -> Measure -> bool

    val typesAEquivAux: Erasure -> TcGlobals -> TypeEquivEnv -> TType list -> TType list -> bool

    /// Check the equivalence of two types up to an erasure flag
    val typeEquivAux: Erasure -> TcGlobals -> TType -> TType -> bool

    val typeAEquiv: TcGlobals -> TypeEquivEnv -> TType -> TType -> bool

    /// Check the equivalence of two types
    val typeEquiv: TcGlobals -> TType -> TType -> bool

    val traitsAEquiv: TcGlobals -> TypeEquivEnv -> TraitConstraintInfo -> TraitConstraintInfo -> bool

    val traitKeysAEquiv: TcGlobals -> TypeEquivEnv -> TraitWitnessInfo -> TraitWitnessInfo -> bool

    val typarConstraintsAEquiv: TcGlobals -> TypeEquivEnv -> TyparConstraint -> TyparConstraint -> bool

    val typarsAEquiv: TcGlobals -> TypeEquivEnv -> Typars -> Typars -> bool

    /// Constraints that may be present in an implementation/extension but not required by a signature/base type.
    val isConstraintAllowedAsExtra: TyparConstraint -> bool

    /// Check if declaredTypars are compatible with reqTypars for a type extension.
    /// Allows declaredTypars to have extra NotSupportsNull constraints.
    val typarsAEquivWithAddedNotNullConstraintsAllowed: TcGlobals -> TypeEquivEnv -> Typars -> Typars -> bool

    val returnTypesAEquiv: TcGlobals -> TypeEquivEnv -> TType option -> TType option -> bool

    /// Check the equivalence of two units-of-measure
    val measureEquiv: TcGlobals -> Measure -> Measure -> bool

    /// An immutable mapping from witnesses to some data.
    ///
    /// Note: this uses an immutable HashMap/Dictionary with an IEqualityComparer that captures TcGlobals, see EmptyTraitWitnessInfoHashMap
    type TraitWitnessInfoHashMap<'T> = ImmutableDictionary<TraitWitnessInfo, 'T>

    /// Create an empty immutable mapping from witnesses to some data
    val EmptyTraitWitnessInfoHashMap: TcGlobals -> TraitWitnessInfoHashMap<'T>
