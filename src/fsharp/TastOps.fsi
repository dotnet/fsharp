// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Defines derived expression manipulation and construction functions.
module internal Microsoft.FSharp.Compiler.Tastops 

open System.Text
open System.Collections.Generic
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Rational
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Lib

#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif

//-------------------------------------------------------------------------
// Type equivalence
//------------------------------------------------------------------------- 

type Erasure = EraseAll | EraseMeasures | EraseNone

val typeEquivAux    : Erasure -> TcGlobals  -> TType          -> TType         -> bool
val typeEquiv       :            TcGlobals  -> TType          -> TType         -> bool
val measureEquiv    :            TcGlobals  -> Measure  -> Measure -> bool
val stripTyEqnsWrtErasure: Erasure -> TcGlobals -> TType -> TType

//-------------------------------------------------------------------------
// Build common types
//------------------------------------------------------------------------- 

val mkFunTy : TType -> TType -> TType
val ( --> ) : TType -> TType -> TType
val tryMkForallTy : Typars -> TType -> TType
val ( +-> ) : Typars -> TType -> TType
val mkIteratedFunTy : TTypes -> TType -> TType
val typeOfLambdaArg : range -> Val list -> TType
val mkMultiLambdaTy : range -> Val list -> TType -> TType
val mkLambdaTy : Typars -> TTypes -> TType -> TType

//-------------------------------------------------------------------------
// Module publication, used while compiling fslib.
//------------------------------------------------------------------------- 

val ensureCcuHasModuleOrNamespaceAtPath : CcuThunk -> Ident list -> CompilationPath -> XmlDoc -> unit 

//-------------------------------------------------------------------------
// Miscellaneous accessors on terms
//------------------------------------------------------------------------- 

val stripExpr : Expr -> Expr

val valsOfBinds : Bindings -> Vals 
val (|ExprValWithPossibleTypeInst|_|) : Expr -> (ValRef * ValUseFlag * TType list * range) option

//-------------------------------------------------------------------------
// Build decision trees imperatively
//------------------------------------------------------------------------- 

type MatchBuilder =
    new : SequencePointInfoForBinding * range -> MatchBuilder
    member AddTarget : DecisionTreeTarget -> int
    member AddResultTarget : Expr * SequencePointInfoForTarget -> DecisionTree
    member CloseTargets : unit -> DecisionTreeTarget list
    member Close : DecisionTree * range * TType -> Expr

//-------------------------------------------------------------------------
// Make some special decision graphs
//------------------------------------------------------------------------- 

val mkBoolSwitch : range -> Expr -> DecisionTree -> DecisionTree -> DecisionTree
val primMkCond : SequencePointInfoForBinding -> SequencePointInfoForTarget -> SequencePointInfoForTarget -> range -> TType -> Expr -> Expr -> Expr -> Expr
val mkCond : SequencePointInfoForBinding -> SequencePointInfoForTarget -> range -> TType -> Expr -> Expr -> Expr -> Expr
val mkNonNullCond : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr
val mkIfThen : TcGlobals -> range -> Expr -> Expr -> Expr 

//-------------------------------------------------------------------------
// Generate new locals
//------------------------------------------------------------------------- 

/// Note: try to use exprForValRef or the expression returned from mkLocal instead of this. 
val exprForVal : range -> Val -> Expr
val exprForValRef : range -> ValRef -> Expr

/// Return the local and an expression to reference it
val mkLocal : range -> string -> TType -> Val * Expr
val mkCompGenLocal : range -> string -> TType -> Val * Expr
val mkMutableCompGenLocal : range -> string -> TType -> Val * Expr
val mkCompGenLocalAndInvisbleBind : TcGlobals -> string -> range -> Expr -> Val * Expr * Binding 

//-------------------------------------------------------------------------
// Make lambdas
//------------------------------------------------------------------------- 

val mkMultiLambda : range -> Val list -> Expr * TType -> Expr
val rebuildLambda : range -> Val option -> Val option -> Val list -> Expr * TType -> Expr
val mkLambda : range -> Val -> Expr * TType -> Expr
val mkTypeLambda : range -> Typars -> Expr * TType -> Expr
val mkObjExpr : TType * Val option * Expr * ObjExprMethod list * (TType * ObjExprMethod list) list * Range.range -> Expr
val mkTypeChoose : range -> Typars -> Expr -> Expr
val mkLambdas : range -> Typars -> Val list -> Expr * TType -> Expr
val mkMultiLambdasCore : range -> Val list list -> Expr * TType -> Expr * TType
val mkMultiLambdas : range -> Typars -> Val list list -> Expr * TType -> Expr
val mkMemberLambdas : range -> Typars -> Val option -> Val option -> Val list list -> Expr * TType -> Expr

val mkWhile      : TcGlobals -> SequencePointInfoForWhileLoop * SpecialWhileLoopMarker * Expr * Expr * range                          -> Expr
val mkFor        : TcGlobals -> SequencePointInfoForForLoop * Val * Expr * ForLoopStyle * Expr * Expr * range -> Expr
val mkTryWith  : TcGlobals -> Expr * (* filter val *) Val * (* filter expr *) Expr * (* handler val *) Val * (* handler expr *) Expr * range * TType * SequencePointInfoForTry * SequencePointInfoForWith -> Expr
val mkTryFinally: TcGlobals -> Expr * Expr * range * TType * SequencePointInfoForTry * SequencePointInfoForFinally -> Expr

//-------------------------------------------------------------------------
// Make let/letrec
//------------------------------------------------------------------------- 
 

// Generate a user-level let-bindings
val mkBind : SequencePointInfoForBinding -> Val -> Expr -> Binding
val mkLetBind : range -> Binding -> Expr -> Expr
val mkLetsBind : range -> Binding list -> Expr -> Expr
val mkLetsFromBindings : range -> Bindings -> Expr -> Expr
val mkLet : SequencePointInfoForBinding -> range -> Val -> Expr -> Expr -> Expr
val mkMultiLambdaBind : Val -> SequencePointInfoForBinding -> range -> Typars -> Val list list -> Expr * TType -> Binding

// Compiler generated bindings may involve a user variable.
// Compiler generated bindings may give rise to a sequence point if they are part of
// an SPAlways expression. Compiler generated bindings can arise from for example, inlining.
val mkCompGenBind : Val -> Expr -> Binding
val mkCompGenBinds : Val list -> Exprs -> Bindings
val mkCompGenLet : range -> Val -> Expr -> Expr -> Expr

// Invisible bindings are never given a sequence point and should never have side effects
val mkInvisibleLet : range -> Val -> Expr -> Expr -> Expr
val mkInvisibleBind : Val -> Expr -> Binding
val mkInvisibleBinds : Vals -> Exprs -> Bindings
val mkLetRecBinds : range -> Bindings -> Expr -> Expr
 
//-------------------------------------------------------------------------
// Generalization/inference helpers
//------------------------------------------------------------------------- 
 
/// TypeScheme (generalizedTypars, tauTy)
///
///    generalizedTypars -- the truly generalized type parameters 
///    tauTy  --  the body of the generalized type. A 'tau' type is one with its type parameters stripped off.
type TypeScheme = TypeScheme of Typars  * TType    

val mkGenericBindRhs : TcGlobals -> range -> Typars -> TypeScheme -> Expr -> Expr
val isBeingGeneralized : Typar -> TypeScheme -> bool

//-------------------------------------------------------------------------
// Make lazy and/or
//------------------------------------------------------------------------- 

val mkLazyAnd  : TcGlobals -> range -> Expr -> Expr -> Expr
val mkLazyOr   : TcGlobals -> range -> Expr -> Expr -> Expr
val mkByrefTy  : TcGlobals -> TType -> TType

//-------------------------------------------------------------------------
// Make construction operations
//------------------------------------------------------------------------- 

val mkUnionCaseExpr : UnionCaseRef * TypeInst * Exprs * range -> Expr
val mkExnExpr : TyconRef * Exprs * range -> Expr
val mkAsmExpr : ILInstr list * TypeInst * Exprs * TTypes * range -> Expr
val mkCoerceExpr : Expr * TType * range * TType -> Expr
val mkReraise : range -> TType -> Expr
val mkReraiseLibCall : TcGlobals -> TType -> range -> Expr


//-------------------------------------------------------------------------
// Make projection operations
//------------------------------------------------------------------------- 
 
val mkTupleFieldGet                : TcGlobals -> TupInfo * Expr * TypeInst * int * range -> Expr
val mkRecdFieldGetViaExprAddr      : Expr * RecdFieldRef   * TypeInst               * range -> Expr
val mkRecdFieldGetAddrViaExprAddr  : Expr * RecdFieldRef   * TypeInst               * range -> Expr
val mkStaticRecdFieldGet           :        RecdFieldRef   * TypeInst               * range -> Expr
val mkStaticRecdFieldSet           :        RecdFieldRef   * TypeInst * Expr        * range -> Expr
val mkStaticRecdFieldGetAddr       :        RecdFieldRef   * TypeInst               * range -> Expr
val mkRecdFieldSetViaExprAddr      : Expr * RecdFieldRef   * TypeInst * Expr        * range -> Expr
val mkUnionCaseTagGetViaExprAddr   : Expr * TyconRef       * TypeInst               * range -> Expr

/// Make a 'TOp.UnionCaseProof' expression, which proves a union value is over a particular case (used only for ref-unions, not struct-unions)
val mkUnionCaseProof               : Expr * UnionCaseRef   * TypeInst               * range -> Expr

/// Build a 'TOp.UnionCaseFieldGet' expression for something we've already determined to be a particular union case. For ref-unions,
/// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
/// the input should be the address of the expression.
val mkUnionCaseFieldGetProvenViaExprAddr : Expr * UnionCaseRef   * TypeInst * int         * range -> Expr

/// Build a 'TOp.UnionCaseFieldGetAddr' expression for a field of a union when we've already determined the value to be a particular union case. For ref-unions,
/// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
/// the input should be the address of the expression.
val mkUnionCaseFieldGetAddrProvenViaExprAddr  : Expr * UnionCaseRef   * TypeInst * int         * range -> Expr

/// Build a 'TOp.UnionCaseFieldGetAddr' expression for a field of a union when we've already determined the value to be a particular union case. For ref-unions,
/// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
/// the input should be the address of the expression.
val mkUnionCaseFieldGetUnprovenViaExprAddr : Expr * UnionCaseRef   * TypeInst * int         * range -> Expr

/// Build a 'TOp.UnionCaseFieldSet' expression. For ref-unions, the input expression has 'TType_ucase', which is 
/// an F# compiler internal "type" corresponding to the union case. For struct-unions,
/// the input should be the address of the expression.
val mkUnionCaseFieldSet            : Expr * UnionCaseRef   * TypeInst * int  * Expr * range -> Expr

/// Like mkUnionCaseFieldGetUnprovenViaExprAddr, but for struct-unions, the input should be a copy of the expression.
val mkUnionCaseFieldGetUnproven    : TcGlobals -> Expr * UnionCaseRef   * TypeInst * int         * range -> Expr

val mkExnCaseFieldGet              : Expr * TyconRef               * int         * range -> Expr
val mkExnCaseFieldSet              : Expr * TyconRef               * int  * Expr * range -> Expr

val mkArrayElemAddress : TcGlobals -> ILReadonly * bool * ILArrayShape * TType * Expr * Expr * range -> Expr

//-------------------------------------------------------------------------
// Compiled view of tuples
//------------------------------------------------------------------------- 
 
val maxTuple : int
val goodTupleFields : int
val isCompiledTupleTyconRef : TcGlobals -> TyconRef -> bool
val mkCompiledTupleTyconRef : TcGlobals -> bool -> int -> TyconRef
val mkCompiledTupleTy : TcGlobals -> bool -> TTypes -> TType
val mkCompiledTuple : TcGlobals -> bool -> TTypes * Exprs * range -> TyconRef * TTypes * Exprs * range
val mkGetTupleItemN : TcGlobals -> range -> int -> ILType -> bool -> Expr -> TType -> Expr

val evalTupInfoIsStruct : TupInfo -> bool

//-------------------------------------------------------------------------
// Take the address of an expression, or force it into a mutable local. Any allocated
// mutable local may need to be kept alive over a larger expression, hence we return
// a wrapping function that wraps "let mutable loc = Expr in ..." around a larger
// expression.
//------------------------------------------------------------------------- 

exception DefensiveCopyWarning of string * range 
type Mutates = DefinitelyMutates | PossiblyMutates | NeverMutates
val mkExprAddrOfExprAux : TcGlobals -> bool -> bool -> Mutates -> Expr -> ValRef option -> range -> (Val * Expr) option * Expr
val mkExprAddrOfExpr : TcGlobals -> bool -> bool -> Mutates -> Expr -> ValRef option -> range -> (Expr -> Expr) * Expr

//-------------------------------------------------------------------------
// Tables keyed on values and/or type parameters
//------------------------------------------------------------------------- 

/// Maps Val to T, based on stamps
[<Struct;NoEquality; NoComparison>]
type ValMap<'T> = 
    member Contents : StampMap<'T>
    member Item : Val -> 'T with get
    member TryFind : Val -> 'T option
    member ContainsVal : Val -> bool
    member Add : Val -> 'T -> ValMap<'T>
    member Remove : Val -> ValMap<'T>
    member IsEmpty : bool
    static member Empty : ValMap<'T>
    static member OfList : (Val * 'T) list -> ValMap<'T>

/// Mutable data structure mapping Val's to T based on stamp keys
[<Sealed; NoEquality; NoComparison>]
type ValHash<'T> =
    member Values : seq<'T>
    member TryFind : Val -> 'T option
    member Add : Val * 'T -> unit
    static member Create : unit -> ValHash<'T>


/// Maps Val's to list of T based on stamp keys
[<Struct; NoEquality; NoComparison>]
type ValMultiMap<'T> =
    member Find : Val -> 'T list
    member Add : Val * 'T -> ValMultiMap<'T>
    member Remove : Val -> ValMultiMap<'T>
    member Contents : StampMap<'T list>
    static member Empty : ValMultiMap<'T>

[<Sealed>]
/// Maps Typar to T based on stamp keys
type TyparMap<'T>  =
    member Item : Typar -> 'T with get
    member ContainsKey : Typar -> bool
    member Add : Typar * 'T -> TyparMap<'T> 
    static member Empty : TyparMap<'T> 

[<NoEquality; NoComparison;Sealed>]
/// Maps TyconRef to T based on stamp keys
type TyconRefMap<'T> =
    member Item : TyconRef -> 'T with get
    member TryFind : TyconRef -> 'T option
    member ContainsKey : TyconRef -> bool
    member Add : TyconRef -> 'T -> TyconRefMap<'T>
    member Remove : TyconRef -> TyconRefMap<'T>
    member IsEmpty : bool
    static member Empty : TyconRefMap<'T>
    static member OfList : (TyconRef * 'T) list -> TyconRefMap<'T>

/// Maps TyconRef to list of T based on stamp keys
[<Struct; NoEquality; NoComparison>]
type TyconRefMultiMap<'T> =
    member Find : TyconRef -> 'T list
    member Add : TyconRef * 'T -> TyconRefMultiMap<'T>
    static member Empty : TyconRefMultiMap<'T>
    static member OfList : (TyconRef * 'T) list -> TyconRefMultiMap<'T>


//-------------------------------------------------------------------------
// Orderings on Tycon, Val, RecdFieldRef, Typar
//------------------------------------------------------------------------- 

val valOrder          : IComparer<Val>
val tyconOrder        : IComparer<Tycon>
val recdFieldRefOrder : IComparer<RecdFieldRef>
val typarOrder        : IComparer<Typar>

//-------------------------------------------------------------------------
// Equality on Tycon and Val
//------------------------------------------------------------------------- 

val tyconRefEq : TcGlobals -> TyconRef -> TyconRef -> bool
val valRefEq : TcGlobals -> ValRef -> ValRef -> bool

//-------------------------------------------------------------------------
// Operations on types: substitution
//------------------------------------------------------------------------- 

type TyparInst = (Typar * TType) list

type TyconRefRemap = TyconRefMap<TyconRef>
type ValRemap = ValMap<ValRef>

[<NoEquality; NoComparison>]
type Remap =
    { tpinst : TyparInst;
      valRemap: ValRemap;
      tyconRefRemap : TyconRefRemap;
      removeTraitSolutions: bool }

    static member Empty : Remap

val addTyconRefRemap : TyconRef -> TyconRef -> Remap -> Remap
val addValRemap : Val -> Val -> Remap -> Remap


val mkTyparInst : Typars -> TTypes -> TyparInst
val mkTyconRefInst : TyconRef -> TypeInst -> TyparInst
val emptyTyparInst : TyparInst

val instType               : TyparInst -> TType -> TType
val instTypes              : TyparInst -> TypeInst -> TypeInst
val instTyparConstraints  : TyparInst -> TyparConstraint list -> TyparConstraint list 
val instTrait              : TyparInst -> TraitConstraintInfo -> TraitConstraintInfo 

//-------------------------------------------------------------------------
// From typars to types 
//------------------------------------------------------------------------- 

val generalizeTypars : Typars -> TypeInst
val generalizeTyconRef : TyconRef -> TTypes * TType
val generalizedTyconRef : TyconRef -> TType
val mkTyparToTyparRenaming : Typars -> Typars -> TyparInst * TTypes

//-------------------------------------------------------------------------
// See through typar equations from inference and/or type abbreviation equations.
//------------------------------------------------------------------------- 

val reduceTyconRefAbbrev : TyconRef -> TypeInst -> TType
val reduceTyconRefMeasureableOrProvided : TcGlobals -> TyconRef -> TypeInst -> TType
val reduceTyconRefAbbrevMeasureable : TyconRef -> Measure

/// set bool to 'true' to allow shortcutting of type parameter equation chains during stripping 
val stripTyEqnsA : TcGlobals -> bool -> TType -> TType 
val stripTyEqns : TcGlobals -> TType -> TType
val stripTyEqnsAndMeasureEqns : TcGlobals -> TType -> TType

val tryNormalizeMeasureInType : TcGlobals -> TType -> TType

//-------------------------------------------------------------------------
// 
//------------------------------------------------------------------------- 

/// See through F# exception abbreviations
val stripExnEqns : TyconRef -> Tycon
val recdFieldsOfExnDefRef : TyconRef -> RecdField list
val recdFieldTysOfExnDefRef : TyconRef -> TType list

//-------------------------------------------------------------------------
// Analyze types.  These all look through type abbreviations and 
// inference equations, i.e. are "stripped"
//------------------------------------------------------------------------- 

val destForallTy      : TcGlobals -> TType -> Typars * TType
val destFunTy         : TcGlobals -> TType -> TType * TType
val destAnyTupleTy    : TcGlobals -> TType -> TupInfo * TTypes
val destRefTupleTy    : TcGlobals -> TType -> TTypes
val destStructTupleTy : TcGlobals -> TType -> TTypes
val destTyparTy       : TcGlobals -> TType -> Typar
val destAnyParTy      : TcGlobals -> TType -> Typar
val destMeasureTy     : TcGlobals -> TType -> Measure
val tryDestForallTy   : TcGlobals -> TType -> Typars * TType

val isFunTy            : TcGlobals -> TType -> bool
val isForallTy         : TcGlobals -> TType -> bool
val isAnyTupleTy       : TcGlobals -> TType -> bool
val isRefTupleTy       : TcGlobals -> TType -> bool
val isStructTupleTy    : TcGlobals -> TType -> bool
val isUnionTy          : TcGlobals -> TType -> bool
val isReprHiddenTy     : TcGlobals -> TType -> bool
val isFSharpObjModelTy : TcGlobals -> TType -> bool
val isRecdTy           : TcGlobals -> TType -> bool
val isTyparTy          : TcGlobals -> TType -> bool
val isAnyParTy         : TcGlobals -> TType -> bool
val tryAnyParTy        : TcGlobals -> TType -> Typar option
val isMeasureTy        : TcGlobals -> TType -> bool

val mkAppTy : TyconRef -> TypeInst -> TType

val mkProvenUnionCaseTy : UnionCaseRef -> TypeInst -> TType
val isProvenUnionCaseTy : TType -> bool

val isAppTy        : TcGlobals -> TType -> bool
val destAppTy      : TcGlobals -> TType -> TyconRef * TypeInst
val tcrefOfAppTy   : TcGlobals -> TType -> TyconRef
val tyconOfAppTy   : TcGlobals -> TType -> Tycon
val tryDestAppTy   : TcGlobals -> TType -> TyconRef option
val tryDestTyparTy : TcGlobals -> TType -> Typar option
val tryDestFunTy : TcGlobals -> TType -> (TType * TType) option
val argsOfAppTy    : TcGlobals -> TType -> TypeInst
val mkInstForAppTy  : TcGlobals -> TType -> TyparInst

/// Try to get a TyconRef for a type without erasing type abbreviations
val tryNiceEntityRefOfTy : TType -> TyconRef option


val domainOfFunTy  : TcGlobals -> TType -> TType
val rangeOfFunTy   : TcGlobals -> TType -> TType
val stripFunTy     : TcGlobals -> TType -> TType list * TType
val stripFunTyN    : TcGlobals -> int -> TType -> TType list * TType

val applyForallTy : TcGlobals -> TType -> TypeInst -> TType

val tryDestAnyTupleTy : TcGlobals -> TType -> TupInfo * TType list
val tryDestRefTupleTy : TcGlobals -> TType -> TType list

//-------------------------------------------------------------------------
// Compute actual types of union cases and fields given an instantiation 
// of the generic type parameters of the enclosing type.
//------------------------------------------------------------------------- 

val actualResultTyOfUnionCase : TypeInst -> UnionCaseRef -> TType

val actualTysOfUnionCaseFields : TyparInst -> UnionCaseRef -> TType list

val actualTysOfInstanceRecdFields    : TyparInst -> TyconRef -> TType list

val actualTyOfRecdField            : TyparInst -> RecdField -> TType
val actualTyOfRecdFieldRef : RecdFieldRef -> TypeInst -> TType
val actualTyOfRecdFieldForTycon    : Tycon -> TypeInst -> RecdField -> TType

//-------------------------------------------------------------------------
// Top types: guaranteed to be compiled to .NET methods, and must be able to 
// have user-specified argument names (for stability w.r.t. reflection)
// and user-specified argument and return attributes.
//------------------------------------------------------------------------- 

type UncurriedArgInfos = (TType * ArgReprInfo) list 
type CurriedArgInfos = UncurriedArgInfos list

val destTopForallTy : TcGlobals -> ValReprInfo -> TType -> Typars * TType 
val GetTopTauTypeInFSharpForm     : TcGlobals -> ArgReprInfo list list -> TType -> range -> CurriedArgInfos * TType
val GetTopValTypeInFSharpForm     : TcGlobals -> ValReprInfo -> TType -> range -> Typars * CurriedArgInfos * TType * ArgReprInfo
val IsCompiledAsStaticProperty    : TcGlobals -> Val -> bool
val IsCompiledAsStaticPropertyWithField : TcGlobals -> Val -> bool
val GetTopValTypeInCompiledForm   : TcGlobals -> ValReprInfo -> TType -> range -> Typars * CurriedArgInfos * TType option * ArgReprInfo
val GetFSharpViewOfReturnType     : TcGlobals -> TType option -> TType

val NormalizeDeclaredTyparsForEquiRecursiveInference : TcGlobals -> Typars -> Typars

//-------------------------------------------------------------------------
// Compute the return type after an application
//------------------------------------------------------------------------- 
 
val applyTys : TcGlobals -> TType -> TType list * 'T list -> TType

//-------------------------------------------------------------------------
// Compute free variables in types
//------------------------------------------------------------------------- 
 
val emptyFreeTypars : FreeTypars
val unionFreeTypars : FreeTypars -> FreeTypars -> FreeTypars

val emptyFreeTycons : FreeTycons
val unionFreeTycons : FreeTycons -> FreeTycons -> FreeTycons

val emptyFreeTyvars : FreeTyvars
val unionFreeTyvars : FreeTyvars -> FreeTyvars -> FreeTyvars

val emptyFreeLocals : FreeLocals
val unionFreeLocals : FreeLocals -> FreeLocals -> FreeLocals

type FreeVarOptions 

val CollectLocalsNoCaching : FreeVarOptions
val CollectTyparsNoCaching : FreeVarOptions
val CollectTyparsAndLocalsNoCaching : FreeVarOptions
val CollectTyparsAndLocals : FreeVarOptions
val CollectLocals : FreeVarOptions
val CollectTypars : FreeVarOptions
val CollectAllNoCaching : FreeVarOptions
val CollectAll : FreeVarOptions

val accFreeInTypes : FreeVarOptions -> TType list -> FreeTyvars -> FreeTyvars
val accFreeInType : FreeVarOptions -> TType -> FreeTyvars -> FreeTyvars
val accFreeInTypars : FreeVarOptions -> Typars -> FreeTyvars -> FreeTyvars

val freeInType  : FreeVarOptions -> TType      -> FreeTyvars
val freeInTypes : FreeVarOptions -> TType list -> FreeTyvars
val freeInVal   : FreeVarOptions -> Val -> FreeTyvars

// This one puts free variables in canonical left-to-right order. 
val freeInTypeLeftToRight : TcGlobals -> bool -> TType -> Typars
val freeInTypesLeftToRight : TcGlobals -> bool -> TType list -> Typars
val freeInTypesLeftToRightSkippingConstraints : TcGlobals -> TType list -> Typars


val isDimensionless : TcGlobals -> TType -> bool

//-------------------------------------------------------------------------
// Equivalence of types (up to substitution of type variables in the left-hand type)
//------------------------------------------------------------------------- 

[<NoEquality; NoComparison>]
type TypeEquivEnv = 
    { EquivTypars: TyparMap<TType>;
      EquivTycons: TyconRefRemap }

    static member Empty : TypeEquivEnv
    member BindEquivTypars : Typars -> Typars -> TypeEquivEnv
    static member FromTyparInst : TyparInst -> TypeEquivEnv
    static member FromEquivTypars : Typars -> Typars -> TypeEquivEnv

val traitsAEquivAux           : Erasure -> TcGlobals -> TypeEquivEnv -> TraitConstraintInfo  -> TraitConstraintInfo  -> bool
val traitsAEquiv              :            TcGlobals -> TypeEquivEnv -> TraitConstraintInfo  -> TraitConstraintInfo  -> bool
val typarConstraintsAEquivAux : Erasure -> TcGlobals -> TypeEquivEnv -> TyparConstraint      -> TyparConstraint      -> bool
val typarConstraintsAEquiv    :            TcGlobals -> TypeEquivEnv -> TyparConstraint      -> TyparConstraint      -> bool
val typarsAEquiv              :            TcGlobals -> TypeEquivEnv -> Typars               -> Typars               -> bool
val typeAEquivAux             : Erasure -> TcGlobals -> TypeEquivEnv -> TType                  -> TType                  -> bool
val typeAEquiv                :            TcGlobals -> TypeEquivEnv -> TType                  -> TType                  -> bool
val returnTypesAEquivAux      : Erasure -> TcGlobals -> TypeEquivEnv -> TType option           -> TType option           -> bool
val returnTypesAEquiv         :            TcGlobals -> TypeEquivEnv -> TType option           -> TType option           -> bool
val tcrefAEquiv               :            TcGlobals -> TypeEquivEnv -> TyconRef             -> TyconRef             -> bool
val valLinkageAEquiv          :            TcGlobals -> TypeEquivEnv -> Val   -> Val -> bool

//-------------------------------------------------------------------------
// Erasure of types wrt units-of-measure and type providers
//-------------------------------------------------------------------------

// Return true if this type is a nominal type that is an erased provided type
val isErasedType              : TcGlobals -> TType -> bool

// Return all components (units-of-measure, and types) of this type that would be erased
val getErasedTypes            : TcGlobals -> TType -> TType list

//-------------------------------------------------------------------------
// Unit operations
//------------------------------------------------------------------------- 

val MeasurePower : Measure -> int -> Measure
val ListMeasureVarOccsWithNonZeroExponents : Measure -> (Typar * Rational) list
val ListMeasureConOccsWithNonZeroExponents : TcGlobals -> bool -> Measure -> (TyconRef * Rational) list
val ProdMeasures : Measure list -> Measure
val MeasureVarExponent : Typar -> Measure -> Rational
val MeasureExprConExponent : TcGlobals -> bool -> TyconRef -> Measure -> Rational
val normalizeMeasure : TcGlobals -> Measure -> Measure


//-------------------------------------------------------------------------
// Members 
//------------------------------------------------------------------------- 

val GetTypeOfMemberInFSharpForm : TcGlobals -> ValRef -> Typars * CurriedArgInfos * TType * ArgReprInfo
val GetTypeOfMemberInMemberForm : TcGlobals -> ValRef -> Typars * CurriedArgInfos * TType option * ArgReprInfo
val GetTypeOfIntrinsicMemberInCompiledForm : TcGlobals -> ValRef -> Typars * CurriedArgInfos * TType option * ArgReprInfo
val GetMemberTypeInMemberForm : TcGlobals -> MemberFlags -> ValReprInfo -> TType -> range -> Typars * CurriedArgInfos * TType option * ArgReprInfo

/// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
val PartitionValTyparsForApparentEnclosingType : TcGlobals -> Val -> (Typars * Typars * Typars * TyparInst * TType list) option
/// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
val PartitionValTypars : TcGlobals -> Val -> (Typars * Typars * Typars * TyparInst * TType list) option
/// Returns (parentTypars,memberParentTypars,memberMethodTypars,memberToParentInst,tinst)
val PartitionValRefTypars : TcGlobals -> ValRef -> (Typars * Typars * Typars * TyparInst * TType list) option

val ReturnTypeOfPropertyVal : TcGlobals -> Val -> TType
val ArgInfosOfPropertyVal : TcGlobals -> Val -> UncurriedArgInfos 
val ArgInfosOfMember: TcGlobals -> ValRef -> CurriedArgInfos 

val GetMemberCallInfo : TcGlobals -> ValRef * ValUseFlag -> int * bool * bool * bool * bool * bool * bool * bool

//-------------------------------------------------------------------------
// Printing
//------------------------------------------------------------------------- 
 
type TyparConstraintsWithTypars = (Typar * TyparConstraint) list


module PrettyTypes =
    val NeedsPrettyTyparName : Typar -> bool
    val NewPrettyTypars : TyparInst -> Typars -> string list -> Typars * TyparInst
    val PrettyTyparNames : (Typar -> bool) -> string list -> Typars -> string list
    val PrettifyType : TcGlobals -> TType -> TType * TyparConstraintsWithTypars
    val PrettifyInstAndTyparsAndType : TcGlobals -> TyparInst * Typars * TType -> (TyparInst * Typars * TType) * TyparConstraintsWithTypars
    val PrettifyTypePair : TcGlobals -> TType * TType -> (TType * TType) * TyparConstraintsWithTypars
    val PrettifyTypes : TcGlobals -> TTypes -> TTypes * TyparConstraintsWithTypars
    val PrettifyInst : TcGlobals -> TyparInst -> TyparInst * TyparConstraintsWithTypars
    val PrettifyInstAndType : TcGlobals -> TyparInst * TType -> (TyparInst * TType) * TyparConstraintsWithTypars
    val PrettifyInstAndTypes : TcGlobals -> TyparInst * TTypes -> (TyparInst * TTypes) * TyparConstraintsWithTypars
    val PrettifyInstAndSig : TcGlobals -> TyparInst * TTypes * TType -> (TyparInst * TTypes * TType) * TyparConstraintsWithTypars
    val PrettifyCurriedTypes : TcGlobals -> TType list list -> TType list list * TyparConstraintsWithTypars
    val PrettifyCurriedSigTypes : TcGlobals -> TType list list * TType -> (TType list list * TType) * TyparConstraintsWithTypars
    val PrettifyInstAndUncurriedSig : TcGlobals -> TyparInst * UncurriedArgInfos * TType -> (TyparInst * UncurriedArgInfos * TType) * TyparConstraintsWithTypars
    val PrettifyInstAndCurriedSig : TcGlobals -> TyparInst * TTypes * CurriedArgInfos * TType -> (TyparInst * TTypes * CurriedArgInfos * TType) * TyparConstraintsWithTypars

[<NoEquality; NoComparison>]
type DisplayEnv = 
    { includeStaticParametersInTypeNames : bool;
      openTopPathsSorted: Lazy<string list list>; 
      openTopPathsRaw: string list list; 
      shortTypeNames: bool;
      suppressNestedTypes: bool;
      maxMembers : int option;
      showObsoleteMembers: bool; 
      showHiddenMembers: bool; 
      showTyparBinding: bool;
      showImperativeTyparAnnotations: bool;
      suppressInlineKeyword:bool;
      suppressMutableKeyword:bool;
      showMemberContainers: bool;
      shortConstraints:bool;
      useColonForReturnType:bool;
      showAttributes: bool;
      showOverrides:bool;
      showConstraintTyparAnnotations:bool;
      abbreviateAdditionalConstraints: bool;
      showTyparDefaultConstraints: bool
      g: TcGlobals
      contextAccessibility: Accessibility
      generatedValueLayout:(Val -> layout option) }
    member SetOpenPaths: string list list -> DisplayEnv
    static member Empty: TcGlobals -> DisplayEnv

    member AddAccessibility : Accessibility -> DisplayEnv
    member AddOpenPath : string list  -> DisplayEnv
    member AddOpenModuleOrNamespace : ModuleOrNamespaceRef   -> DisplayEnv

val tagEntityRefName: xref: EntityRef -> name: string -> StructuredFormat.TaggedText

/// Return the full text for an item as we want it displayed to the user as a fully qualified entity
val fullDisplayTextOfModRef : ModuleOrNamespaceRef -> string
val fullDisplayTextOfParentOfModRef : ModuleOrNamespaceRef -> string option
val fullDisplayTextOfValRef   : ValRef -> string
val fullDisplayTextOfValRefAsLayout   : ValRef -> StructuredFormat.Layout
val fullDisplayTextOfTyconRef  : TyconRef -> string
val fullDisplayTextOfTyconRefAsLayout  : TyconRef -> StructuredFormat.Layout
val fullDisplayTextOfExnRef  : TyconRef -> string
val fullDisplayTextOfExnRefAsLayout  : TyconRef -> StructuredFormat.Layout
val fullDisplayTextOfUnionCaseRef  : UnionCaseRef -> string
val fullDisplayTextOfRecdFieldRef  : RecdFieldRef -> string

val ticksAndArgCountTextOfTyconRef : TyconRef -> string

/// A unique qualified name for each type definition, used to qualify the names of interface implementation methods
val qualifiedMangledNameOfTyconRef : TyconRef -> string -> string

val trimPathByDisplayEnv : DisplayEnv -> string list -> string

val prefixOfStaticReq : TyparStaticReq -> string
val prefixOfRigidTypar : Typar -> string

/// Utilities used in simplifying types for visual presentation
module SimplifyTypes = 
    type TypeSimplificationInfo =
        { singletons         : Typar Zset;
          inplaceConstraints :  Zmap<Typar,TType>;
          postfixConstraints : TyparConstraintsWithTypars; }
    val typeSimplificationInfo0 : TypeSimplificationInfo
    val CollectInfo : bool -> TType list -> TyparConstraintsWithTypars -> TypeSimplificationInfo

//-------------------------------------------------------------------------
// 
//------------------------------------------------------------------------- 

val superOfTycon : TcGlobals -> Tycon -> TType
val abstractSlotValsOfTycons : Tycon list -> Val list

//-------------------------------------------------------------------------
// Free variables in expressions etc.
//------------------------------------------------------------------------- 

val emptyFreeVars : FreeVars
val unionFreeVars : FreeVars -> FreeVars -> FreeVars

val accFreeInTargets      : FreeVarOptions -> DecisionTreeTarget array -> FreeVars -> FreeVars
val accFreeInExprs        : FreeVarOptions -> Exprs -> FreeVars -> FreeVars
val accFreeInSwitchCases : FreeVarOptions -> DecisionTreeCase list -> DecisionTree option -> FreeVars -> FreeVars
val accFreeInDecisionTree        : FreeVarOptions -> DecisionTree -> FreeVars -> FreeVars

/// Get the free variables in a module definition.
val freeInModuleOrNamespace : FreeVarOptions -> ModuleOrNamespaceExpr -> FreeVars

/// Get the free variables in an expression.
val freeInExpr  : FreeVarOptions -> Expr  -> FreeVars

/// Get the free variables in the right hand side of a binding.
val freeInBindingRhs   : FreeVarOptions -> Binding  -> FreeVars

val freeTyvarsAllPublic  : FreeTyvars -> bool
val freeVarsAllPublic     : FreeVars -> bool

//-------------------------------------------------------------------------
// Mark/range/position information from expressions
//------------------------------------------------------------------------- 

type Expr with 
    member Range : range

//-------------------------------------------------------------------------
// type-of operations on the expression tree
//------------------------------------------------------------------------- 

val tyOfExpr : TcGlobals -> Expr -> TType 

//-------------------------------------------------------------------------
// Top expressions to implement top types
//------------------------------------------------------------------------- 

val stripTopLambda : Expr * TType -> Typars * Val list list * Expr * TType
val InferArityOfExpr : TcGlobals -> TType -> Attribs list list -> Attribs -> Expr -> ValReprInfo
val InferArityOfExprBinding : TcGlobals -> Val -> Expr -> ValReprInfo

//-------------------------------------------------------------------------
//  Copy expressions and types
//------------------------------------------------------------------------- 
                   
// REVIEW: this mutation should not be needed 
val setValHasNoArity : Val -> Val

type ValCopyFlag = 
    | CloneAll
    | CloneAllAndMarkExprValsAsCompilerGenerated
    // OnlyCloneExprVals is a nasty setting to reuse the cloning logic in a mode where all 
    // Tycon and "module/member" Val objects keep their identity, but the Val objects for all Expr bindings
    // are cloned. This is used to 'fixup' the TAST created by tlr.fs 
    //
    // This is a fragile mode of use. It's not really clear why TLR needs to create a "bad" expression tree that
    // reuses Val objects as multiple value bindings, and its been the cause of several subtle bugs.
    | OnlyCloneExprVals

val remapTyconRef : TyconRefRemap -> TyconRef -> TyconRef
val remapUnionCaseRef : TyconRefRemap -> UnionCaseRef -> UnionCaseRef
val remapRecdFieldRef : TyconRefRemap -> RecdFieldRef -> RecdFieldRef
val remapValRef : Remap -> ValRef -> ValRef
val remapExpr : TcGlobals -> ValCopyFlag -> Remap -> Expr -> Expr
val remapAttrib : TcGlobals -> Remap -> Attrib -> Attrib
val remapPossibleForallTy : TcGlobals -> Remap -> TType -> TType
val copyModuleOrNamespaceType : TcGlobals -> ValCopyFlag -> ModuleOrNamespaceType -> ModuleOrNamespaceType
val copyExpr : TcGlobals -> ValCopyFlag -> Expr -> Expr
val copyImplFile : TcGlobals -> ValCopyFlag -> TypedImplFile -> TypedImplFile
val copySlotSig : SlotSig -> SlotSig
val instSlotSig : TyparInst -> SlotSig -> SlotSig
val instExpr : TcGlobals -> TyparInst -> Expr -> Expr

//-------------------------------------------------------------------------
// Build the remapping that corresponds to a module meeting its signature
// and also report the set of tycons, tycon representations and values hidden in the process.
//------------------------------------------------------------------------- 

type SignatureRepackageInfo = 
    { mrpiVals: (ValRef * ValRef) list;
      mrpiEntities: (TyconRef * TyconRef) list  }

    static member Empty : SignatureRepackageInfo
      
type SignatureHidingInfo = 
    { mhiTycons  : Zset<Tycon>; 
      mhiTyconReprs : Zset<Tycon>;  
      mhiVals       : Zset<Val>; 
      mhiRecdFields : Zset<RecdFieldRef>;
      mhiUnionCases : Zset<UnionCaseRef> }
    static member Empty : SignatureHidingInfo

val ComputeRemappingFromInferredSignatureToExplicitSignature : TcGlobals -> ModuleOrNamespaceType -> ModuleOrNamespaceType -> SignatureRepackageInfo * SignatureHidingInfo
val ComputeRemappingFromImplementationToSignature : TcGlobals -> ModuleOrNamespaceExpr -> ModuleOrNamespaceType -> SignatureRepackageInfo * SignatureHidingInfo
val ComputeHidingInfoAtAssemblyBoundary : ModuleOrNamespaceType -> SignatureHidingInfo -> SignatureHidingInfo
val mkRepackageRemapping : SignatureRepackageInfo -> Remap 

val wrapModuleOrNamespaceExprInNamespace : Ident -> CompilationPath -> ModuleOrNamespaceExpr -> ModuleOrNamespaceExpr
val wrapModuleOrNamespaceTypeInNamespace : Ident -> CompilationPath -> ModuleOrNamespaceType -> ModuleOrNamespaceType * ModuleOrNamespace  
val wrapModuleOrNamespaceType : Ident -> CompilationPath -> ModuleOrNamespaceType -> ModuleOrNamespace

val SigTypeOfImplFile : TypedImplFile -> ModuleOrNamespaceType

//-------------------------------------------------------------------------
// Given a list of top-most signatures that together constrain the public compilation units
// of an assembly, compute a remapping that converts local references to non-local references.
// This remapping must be applied to all pickled expressions and types 
// exported from the assembly.
//------------------------------------------------------------------------- 


val tryRescopeEntity : CcuThunk -> Entity -> EntityRef option
val tryRescopeVal    : CcuThunk -> Remap -> Val -> ValRef option

val MakeExportRemapping : CcuThunk -> ModuleOrNamespace -> Remap
val ApplyExportRemappingToEntity :  TcGlobals -> Remap -> ModuleOrNamespace -> ModuleOrNamespace 

/// Query SignatureRepackageInfo
val IsHiddenTycon     : (Remap * SignatureHidingInfo) list -> Tycon -> bool
val IsHiddenTyconRepr : (Remap * SignatureHidingInfo) list -> Tycon -> bool
val IsHiddenVal       : (Remap * SignatureHidingInfo) list -> Val -> bool
val IsHiddenRecdField : (Remap * SignatureHidingInfo) list -> RecdFieldRef -> bool

//-------------------------------------------------------------------------
//  Adjust marks in expressions
//------------------------------------------------------------------------- 

val remarkExpr : range -> Expr -> Expr

//-------------------------------------------------------------------------
// Make applications
//------------------------------------------------------------------------- 
 
val primMkApp : (Expr * TType) -> TypeInst -> Exprs -> range -> Expr
val mkApps : TcGlobals -> (Expr * TType) * TType list list * Exprs * range -> Expr
val mkTyAppExpr : range -> Expr * TType -> TType list -> Expr

///   localv <- e      
val mkValSet   : range -> ValRef -> Expr -> Expr
///  *localv_ptr = e   
val mkAddrSet  : range -> ValRef -> Expr -> Expr
/// *localv_ptr        
val mkAddrGet  : range -> ValRef -> Expr
/// &localv           
val mkValAddr  : range -> ValRef -> Expr

//-------------------------------------------------------------------------
// Note these take the address of the record expression if it is a struct, and
// apply a type instantiation if it is a first-class polymorphic record field.
//------------------------------------------------------------------------- 

val mkRecdFieldGet : TcGlobals -> Expr * RecdFieldRef * TypeInst * range -> Expr

//-------------------------------------------------------------------------
//  Get the targets used in a decision graph (for reporting warnings)
//------------------------------------------------------------------------- 

val accTargetsOfDecisionTree : DecisionTree -> int list -> int list

//-------------------------------------------------------------------------
//  Optimizations on decision graphs
//------------------------------------------------------------------------- 

val mkAndSimplifyMatch : SequencePointInfoForBinding  -> range -> range -> TType -> DecisionTree -> DecisionTreeTarget list -> Expr

val primMkMatch : SequencePointInfoForBinding * range * DecisionTree * DecisionTreeTarget array * range * TType -> Expr

//-------------------------------------------------------------------------
//  Work out what things on the r.h.s. of a let rec need to be fixed up
//------------------------------------------------------------------------- 

val IterateRecursiveFixups : 
   TcGlobals -> Val option  -> 
   (Val option -> Expr -> (Expr -> Expr) -> Expr -> unit) -> 
   Expr * (Expr -> Expr) -> Expr -> unit

//-------------------------------------------------------------------------
// From lambdas taking multiple variables to lambdas taking a single variable
// of tuple type. 
//------------------------------------------------------------------------- 

val MultiLambdaToTupledLambda : TcGlobals -> Val list -> Expr -> Val * Expr
val AdjustArityOfLambdaBody : TcGlobals -> int -> Val list -> Expr -> Val list * Expr

//-------------------------------------------------------------------------
// Make applications, doing beta reduction by introducing let-bindings
//------------------------------------------------------------------------- 

val MakeApplicationAndBetaReduce : TcGlobals -> Expr * TType * TypeInst list * Exprs * range -> Expr

val JoinTyparStaticReq : TyparStaticReq -> TyparStaticReq -> TyparStaticReq

//-------------------------------------------------------------------------
// More layout - this is for debugging
//------------------------------------------------------------------------- 
module DebugPrint =

    val layoutRanges : bool ref
    val showType : TType -> string
    val showExpr : Expr -> string

    val valRefL : ValRef -> layout
    val unionCaseRefL : UnionCaseRef -> layout
    val vspecAtBindL : Val -> layout
    val intL : int -> layout
    val valL : Val -> layout
    val typarDeclL : Typar -> layout
    val traitL : TraitConstraintInfo -> layout
    val typarL : Typar -> layout
    val typarsL : Typars -> layout
    val typeL : TType -> layout
    val slotSigL : SlotSig -> layout
    val entityTypeL : ModuleOrNamespaceType -> layout
    val entityL : ModuleOrNamespace -> layout
    val typeOfValL : Val -> layout
    val bindingL : Binding -> layout
    val exprL : Expr -> layout
    val tyconL : Tycon -> layout
    val decisionTreeL : DecisionTree -> layout
    val implFileL : TypedImplFile -> layout
    val implFilesL : TypedImplFile list -> layout
    val recdFieldRefL : RecdFieldRef -> layout

//-------------------------------------------------------------------------
// Fold on expressions
//------------------------------------------------------------------------- 

type ExprFolder<'State> =
    { exprIntercept            : ('State -> Expr -> 'State) -> 'State -> Expr -> 'State option;
      valBindingSiteIntercept  : 'State -> bool * Val -> 'State;
      nonRecBindingsIntercept  : 'State -> Binding -> 'State;         
      recBindingsIntercept     : 'State -> Bindings -> 'State;         
      dtreeIntercept           : 'State -> DecisionTree -> 'State;
      targetIntercept          : ('State -> Expr -> 'State) -> 'State -> DecisionTreeTarget -> 'State option;
      tmethodIntercept         : ('State -> Expr -> 'State) -> 'State -> ObjExprMethod -> 'State option;}
val ExprFolder0 : ExprFolder<'State>
val FoldImplFile: ExprFolder<'State> -> ('State -> TypedImplFile -> 'State) 
val FoldExpr : ExprFolder<'State> -> ('State -> Expr -> 'State) 

#if DEBUG
val ExprStats : Expr -> string
#endif

//-------------------------------------------------------------------------
// Make some common types
//------------------------------------------------------------------------- 

val mkNativePtrTy  : TcGlobals -> TType -> TType
val mkArrayType      : TcGlobals -> TType -> TType
val isOptionTy     : TcGlobals -> TType -> bool
val destOptionTy   : TcGlobals -> TType -> TType
val tryDestOptionTy : TcGlobals -> TType -> TType option

val isLinqExpressionTy     : TcGlobals -> TType -> bool
val destLinqExpressionTy   : TcGlobals -> TType -> TType
val tryDestLinqExpressionTy : TcGlobals -> TType -> TType option

(*
val isQuoteExprTy     : TcGlobals -> TType -> bool
val destQuoteExprTy   : TcGlobals -> TType -> TType
val tryDestQuoteExprTy : TcGlobals -> TType -> TType option
*)

//-------------------------------------------------------------------------
// Primitives associated with compiling the IEvent idiom to .NET events
//------------------------------------------------------------------------- 

val isIDelegateEventType   : TcGlobals -> TType -> bool
val destIDelegateEventType : TcGlobals -> TType -> TType 
val mkIEventType   : TcGlobals -> TType -> TType -> TType
val mkIObservableType   : TcGlobals -> TType -> TType
val mkIObserverType   : TcGlobals -> TType -> TType

//-------------------------------------------------------------------------
// Primitives associated with printf format string parsing
//------------------------------------------------------------------------- 

val mkLazyTy : TcGlobals -> TType -> TType
val mkPrintfFormatTy : TcGlobals -> TType -> TType -> TType -> TType -> TType -> TType

//-------------------------------------------------------------------------
// Classify types
//------------------------------------------------------------------------- 

type TypeDefMetadata = 
     | ILTypeMetadata of TILObjectReprData
     | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata 
#if EXTENSIONTYPING
     | ProvidedTypeMetadata of  TProvidedTypeInfo
#endif

val metadataOfTycon : Tycon -> TypeDefMetadata
val metadataOfTy : TcGlobals -> TType -> TypeDefMetadata

val isStringTy       : TcGlobals -> TType -> bool
val isListTy         : TcGlobals -> TType -> bool
val isILAppTy      : TcGlobals -> TType -> bool
val isArrayTy        : TcGlobals -> TType -> bool
val isArray1DTy       : TcGlobals -> TType -> bool
val destArrayTy     : TcGlobals -> TType -> TType
val destListTy      : TcGlobals -> TType -> TType

val mkArrayTy         : TcGlobals -> int -> TType -> range -> TType
val isArrayTyconRef      : TcGlobals -> TyconRef -> bool
val rankOfArrayTyconRef : TcGlobals -> TyconRef -> int

val isUnitTy          : TcGlobals -> TType -> bool
val isObjTy           : TcGlobals -> TType -> bool
val isVoidTy          : TcGlobals -> TType -> bool

/// Get the element type of an array type
val destArrayTy    : TcGlobals -> TType -> TType
/// Get the rank of an array type
val rankOfArrayTy : TcGlobals -> TType -> int

val isInterfaceTyconRef                 : TyconRef -> bool

val isDelegateTy                 : TcGlobals -> TType -> bool
val isInterfaceTy                : TcGlobals -> TType -> bool
val isRefTy                      : TcGlobals -> TType -> bool
val isSealedTy                   : TcGlobals -> TType -> bool
val isComInteropTy               : TcGlobals -> TType -> bool
val underlyingTypeOfEnumTy       : TcGlobals -> TType -> TType
val normalizeEnumTy              : TcGlobals -> TType -> TType
val isStructTy                   : TcGlobals -> TType -> bool
val isUnmanagedTy                : TcGlobals -> TType -> bool
val isClassTy                    : TcGlobals -> TType -> bool
val isEnumTy                     : TcGlobals -> TType -> bool
val isStructRecordOrUnionTyconTy : TcGlobals -> TType -> bool

/// For "type Class as self", 'self' is fixed up after initialization. To support this,
/// it is converted behind the scenes to a ref. This function strips off the ref and
/// returns the underlying type.
val StripSelfRefCell : TcGlobals * ValBaseOrThisInfo * TType -> TType

val (|AppTy|_|)   : TcGlobals -> TType -> (TyconRef * TType list) option
val (|NullableTy|_|)   : TcGlobals -> TType -> TType option
val (|StripNullableTy|)   : TcGlobals -> TType -> TType 
val (|ByrefTy|_|)   : TcGlobals -> TType -> TType option

//-------------------------------------------------------------------------
// Special semantic constraints
//------------------------------------------------------------------------- 

val IsUnionTypeWithNullAsTrueValue: TcGlobals -> Tycon -> bool
val TyconHasUseNullAsTrueValueAttribute : TcGlobals -> Tycon -> bool
val CanHaveUseNullAsTrueValueAttribute : TcGlobals -> Tycon -> bool
val MemberIsCompiledAsInstance : TcGlobals -> TyconRef -> bool -> ValMemberInfo -> Attribs -> bool
val ValSpecIsCompiledAsInstance : TcGlobals -> Val -> bool
val ValRefIsCompiledAsInstanceMember : TcGlobals -> ValRef -> bool
val ModuleNameIsMangled : TcGlobals -> Attribs -> bool

val CompileAsEvent : TcGlobals -> Attribs -> bool

val TypeNullIsExtraValue : TcGlobals -> range -> TType -> bool
val TypeNullIsTrueValue : TcGlobals -> TType -> bool
val TypeNullNotLiked : TcGlobals -> range -> TType -> bool
val TypeNullNever : TcGlobals -> TType -> bool

val TypeSatisfiesNullConstraint : TcGlobals -> range -> TType -> bool
val TypeHasDefaultValue : TcGlobals -> range -> TType -> bool

val isAbstractTycon : Tycon -> bool

val isUnionCaseRefAllocObservable : UnionCaseRef -> bool
val isRecdOrUnionOrStructTyconRefAllocObservable : TcGlobals -> TyconRef -> bool
val isExnAllocObservable : TyconRef -> bool 
val isUnionCaseFieldMutable : TcGlobals -> UnionCaseRef -> int -> bool
val isExnFieldMutable : TyconRef -> int -> bool

val useGenuineField : Tycon -> RecdField -> bool 
val ComputeFieldName : Tycon -> RecdField -> string

//-------------------------------------------------------------------------
// Destruct slotsigs etc.
//------------------------------------------------------------------------- 

val slotSigHasVoidReturnTy     : SlotSig -> bool
val actualReturnTyOfSlotSig    : TypeInst -> TypeInst -> SlotSig -> TType option

val returnTyOfMethod : TcGlobals -> ObjExprMethod -> TType option

//-------------------------------------------------------------------------
// Primitives associated with initialization graphs
//------------------------------------------------------------------------- 

val mkRefCell              : TcGlobals -> range -> TType -> Expr -> Expr
val mkRefCellGet          : TcGlobals -> range -> TType -> Expr -> Expr
val mkRefCellSet          : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkLazyDelayed         : TcGlobals -> range -> TType -> Expr -> Expr
val mkLazyForce           : TcGlobals -> range -> TType -> Expr -> Expr


val mkRefCellContentsRef : TcGlobals -> RecdFieldRef
val isRefCellTy   : TcGlobals -> TType -> bool
val destRefCellTy : TcGlobals -> TType -> TType
val mkRefCellTy   : TcGlobals -> TType -> TType

val mkSeqTy          : TcGlobals -> TType -> TType
val mkIEnumeratorTy  : TcGlobals -> TType -> TType
val mkListTy         : TcGlobals -> TType -> TType
val mkOptionTy       : TcGlobals -> TType -> TType
val mkNoneCase  : TcGlobals -> UnionCaseRef
val mkSomeCase  : TcGlobals -> UnionCaseRef

val mkNil  : TcGlobals -> range -> TType -> Expr
val mkCons : TcGlobals -> TType -> Expr -> Expr -> Expr

//-------------------------------------------------------------------------
// Make a few more expressions
//------------------------------------------------------------------------- 

val mkSequential  : SequencePointInfoForSeq -> range -> Expr -> Expr -> Expr
val mkCompGenSequential  : range -> Expr -> Expr -> Expr
val mkSequentials : SequencePointInfoForSeq -> TcGlobals -> range -> Exprs -> Expr   
val mkRecordExpr : TcGlobals -> RecordConstructionInfo * TyconRef * TypeInst * RecdFieldRef list * Exprs * range -> Expr
val mkUnbox : TType -> Expr -> range -> Expr
val mkBox : TType -> Expr -> range -> Expr
val mkIsInst : TType -> Expr -> range -> Expr
val mkNull : range -> TType -> Expr
val mkNullTest : TcGlobals -> range -> Expr -> Expr -> Expr -> Expr
val mkNonNullTest : TcGlobals -> range -> Expr -> Expr
val mkIsInstConditional : TcGlobals -> range -> TType -> Expr -> Val -> Expr -> Expr -> Expr
val mkThrow   : range -> TType -> Expr -> Expr
val mkGetArg0 : range -> TType -> Expr

val mkDefault : range * TType -> Expr

val isThrow : Expr -> bool

val mkString    : TcGlobals -> range -> string -> Expr
val mkBool      : TcGlobals -> range -> bool -> Expr
val mkByte      : TcGlobals -> range -> byte -> Expr
val mkUInt16    : TcGlobals -> range -> uint16 -> Expr
val mkTrue      : TcGlobals -> range -> Expr
val mkFalse     : TcGlobals -> range -> Expr
val mkUnit      : TcGlobals -> range -> Expr
val mkInt32     : TcGlobals -> range -> int32 -> Expr
val mkInt       : TcGlobals -> range -> int -> Expr
val mkZero      : TcGlobals -> range -> Expr
val mkOne       : TcGlobals -> range -> Expr
val mkTwo       : TcGlobals -> range -> Expr
val mkMinusOne : TcGlobals -> range -> Expr
val destInt32 : Expr -> int32 option

//-------------------------------------------------------------------------
// Primitives associated with quotations
//------------------------------------------------------------------------- 
 
val isQuotedExprTy : TcGlobals -> TType -> bool
val destQuotedExprTy : TcGlobals -> TType -> TType
val mkQuotedExprTy : TcGlobals -> TType -> TType
val mkRawQuotedExprTy : TcGlobals -> TType

//-------------------------------------------------------------------------
// Primitives associated with IL code gen
//------------------------------------------------------------------------- 

val mspec_Type_GetTypeFromHandle : TcGlobals ->  ILMethodSpec
val fspec_Missing_Value : TcGlobals ->  ILFieldSpec
val mkInitializeArrayMethSpec: TcGlobals -> ILMethodSpec 
val mkByteArrayTy : TcGlobals -> TType
val mkInvalidCastExnNewobj: TcGlobals -> ILInstr


//-------------------------------------------------------------------------
// Construct calls to some intrinsic functions
//------------------------------------------------------------------------- 

val mkCallNewFormat              : TcGlobals -> range -> TType -> TType -> TType -> TType -> TType -> Expr -> Expr

val mkCallUnbox       : TcGlobals -> range -> TType -> Expr -> Expr
val mkCallGetGenericComparer : TcGlobals -> range -> Expr
val mkCallGetGenericEREqualityComparer : TcGlobals -> range -> Expr
val mkCallGetGenericPEREqualityComparer : TcGlobals -> range -> Expr

val mkCallUnboxFast  : TcGlobals -> range -> TType -> Expr -> Expr
val canUseUnboxFast  : TcGlobals -> range -> TType -> bool

val mkCallDispose     : TcGlobals -> range -> TType -> Expr -> Expr
val mkCallSeq         : TcGlobals -> range -> TType -> Expr -> Expr

val mkCallTypeTest      : TcGlobals -> range -> TType -> Expr -> Expr
val canUseTypeTestFast : TcGlobals -> TType -> bool

val mkCallTypeOf      : TcGlobals -> range -> TType -> Expr
val mkCallTypeDefOf   : TcGlobals -> range -> TType -> Expr 

val mkCallCreateInstance     : TcGlobals -> range -> TType -> Expr
val mkCallCreateEvent        : TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr -> Expr
val mkCallArrayLength        : TcGlobals -> range -> TType -> Expr -> Expr
val mkCallArrayGet           : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallArray2DGet         : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr 
val mkCallArray3DGet         : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr
val mkCallArray4DGet         : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr
val mkCallRaise              : TcGlobals -> range -> TType -> Expr -> Expr

val mkCallGenericComparisonWithComparerOuter : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr
val mkCallGenericEqualityEROuter             : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallEqualsOperator                     : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallSubtractionOperator                : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallGenericEqualityWithComparerOuter   : TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr
val mkCallGenericHashWithComparerOuter       : TcGlobals -> range -> TType -> Expr -> Expr -> Expr

val mkCallDeserializeQuotationFSharp20Plus  : TcGlobals -> range -> Expr -> Expr -> Expr -> Expr -> Expr
val mkCallDeserializeQuotationFSharp40Plus : TcGlobals -> range -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr
val mkCallCastQuotation      : TcGlobals -> range -> TType -> Expr -> Expr 
val mkCallLiftValueWithName          : TcGlobals -> range -> TType -> string -> Expr -> Expr
val mkCallLiftValueWithDefn          : TcGlobals -> range -> TType -> Expr -> Expr
val mkCallSeqCollect         : TcGlobals -> range -> TType  -> TType -> Expr -> Expr -> Expr
val mkCallSeqUsing           : TcGlobals -> range -> TType  -> TType -> Expr -> Expr -> Expr
val mkCallSeqDelay           : TcGlobals -> range -> TType  -> Expr -> Expr
val mkCallSeqAppend          : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallSeqFinally         : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallSeqGenerated       : TcGlobals -> range -> TType -> Expr -> Expr -> Expr
val mkCallSeqOfFunctions    : TcGlobals -> range -> TType  -> TType -> Expr -> Expr -> Expr -> Expr
val mkCallSeqToArray        : TcGlobals -> range -> TType  -> Expr -> Expr 
val mkCallSeqToList         : TcGlobals -> range -> TType  -> Expr -> Expr 
val mkCallSeqMap             : TcGlobals -> range -> TType  -> TType -> Expr -> Expr -> Expr
val mkCallSeqSingleton       : TcGlobals -> range -> TType  -> Expr -> Expr
val mkCallSeqEmpty           : TcGlobals -> range -> TType  -> Expr
val mkILAsmCeq                   : TcGlobals -> range -> Expr -> Expr -> Expr
val mkILAsmClt                   : TcGlobals -> range -> Expr -> Expr -> Expr

val mkCallFailInit           : TcGlobals -> range -> Expr 
val mkCallFailStaticInit    : TcGlobals -> range -> Expr 
val mkCallCheckThis          : TcGlobals -> range -> TType -> Expr -> Expr 

val mkCase : DecisionTreeTest * DecisionTree -> DecisionTreeCase

val mkCallQuoteToLinqLambdaExpression : TcGlobals -> range -> TType -> Expr -> Expr

val mkCallGetQuerySourceAsEnumerable : TcGlobals -> range -> TType -> TType -> Expr -> Expr
val mkCallNewQuerySource : TcGlobals -> range -> TType -> TType -> Expr -> Expr

val mkArray : TType * Exprs * range -> Expr

//-------------------------------------------------------------------------
// operations primarily associated with the optimization to fix
// up loops to generate .NET code that does not include array bound checks
//------------------------------------------------------------------------- 

val mkDecr   : TcGlobals -> range -> Expr -> Expr
val mkIncr   : TcGlobals -> range -> Expr -> Expr
val mkLdlen  : TcGlobals -> range -> Expr -> Expr
val mkLdelem : TcGlobals -> range -> TType -> Expr -> Expr -> Expr

//-------------------------------------------------------------------------
// Analyze attribute sets 
//------------------------------------------------------------------------- 

val TryDecodeILAttribute   : TcGlobals -> ILTypeRef -> ILAttributes -> (ILAttribElem list * ILAttributeNamedArg list) option
val TryFindILAttribute : BuiltinAttribInfo -> ILAttributes -> bool
val TryFindILAttributeOpt : BuiltinAttribInfo option -> ILAttributes -> bool

val IsMatchingFSharpAttribute      : TcGlobals -> BuiltinAttribInfo -> Attrib -> bool
val IsMatchingFSharpAttributeOpt   : TcGlobals -> BuiltinAttribInfo option -> Attrib -> bool
val HasFSharpAttribute             : TcGlobals -> BuiltinAttribInfo -> Attribs -> bool
val HasFSharpAttributeOpt          : TcGlobals -> BuiltinAttribInfo option -> Attribs -> bool
val TryFindFSharpAttribute         : TcGlobals -> BuiltinAttribInfo -> Attribs -> Attrib option
val TryFindFSharpAttributeOpt      : TcGlobals -> BuiltinAttribInfo option -> Attribs -> Attrib option
val TryFindFSharpBoolAttribute     : TcGlobals -> BuiltinAttribInfo -> Attribs -> bool option
val TryFindFSharpBoolAttributeAssumeFalse : TcGlobals -> BuiltinAttribInfo -> Attribs -> bool option
val TryFindFSharpStringAttribute   : TcGlobals -> BuiltinAttribInfo -> Attribs -> string option
val TryFindFSharpInt32Attribute    : TcGlobals -> BuiltinAttribInfo -> Attribs -> int32 option

/// Try to find a specific attribute on a type definition, where the attribute accepts a string argument.
///
/// This is used to detect the 'DefaultMemberAttribute' and 'ConditionalAttribute' attributes (on type definitions)
val TryFindTyconRefStringAttribute : TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> string option

/// Try to find a specific attribute on a type definition, where the attribute accepts a bool argument.
val TryFindTyconRefBoolAttribute : TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> bool option

/// Try to find a specific attribute on a type definition
val TyconRefHasAttribute : TcGlobals -> range -> BuiltinAttribInfo -> TyconRef -> bool

/// Try to find the AttributeUsage attribute, looking for the value of the AllowMultiple named parameter
val TryFindAttributeUsageAttribute : TcGlobals -> range -> TyconRef -> bool option

#if EXTENSIONTYPING
/// returns Some(assemblyName) for success
val TryDecodeTypeProviderAssemblyAttr : ILGlobals -> ILAttribute -> string option
#endif
val IsSignatureDataVersionAttr  : ILAttribute -> bool
val ILThingHasExtensionAttribute : ILAttributes -> bool
val TryFindAutoOpenAttr           : IL.ILGlobals -> ILAttribute -> string option 
val TryFindInternalsVisibleToAttr : IL.ILGlobals -> ILAttribute -> string option 
val IsMatchingSignatureDataVersionAttr : IL.ILGlobals -> ILVersionInfo -> ILAttribute -> bool


val mkCompilationMappingAttr                         : TcGlobals -> int -> ILAttribute
val mkCompilationMappingAttrWithSeqNum               : TcGlobals -> int -> int -> ILAttribute
val mkCompilationMappingAttrWithVariantNumAndSeqNum  : TcGlobals -> int -> int -> int             -> ILAttribute
val mkCompilationMappingAttrForQuotationResource     : TcGlobals -> string * ILTypeRef list -> ILAttribute
val mkCompilationArgumentCountsAttr                  : TcGlobals -> int list -> ILAttribute
val mkCompilationSourceNameAttr                      : TcGlobals -> string -> ILAttribute
val mkSignatureDataVersionAttr                       : TcGlobals -> ILVersionInfo -> ILAttribute
val mkCompilerGeneratedAttr                          : TcGlobals -> int -> ILAttribute

//-------------------------------------------------------------------------
// More common type construction
//------------------------------------------------------------------------- 

val isByrefTy : TcGlobals -> TType -> bool
val isNativePtrTy : TcGlobals -> TType -> bool
val destByrefTy : TcGlobals -> TType -> TType
val destNativePtrTy : TcGlobals -> TType -> TType

val isByrefLikeTyconRef : TcGlobals -> TyconRef -> bool
val isByrefLikeTy : TcGlobals -> TType -> bool

//-------------------------------------------------------------------------
// Tuple constructors/destructors
//------------------------------------------------------------------------- 

val isRefTupleExpr : Expr -> bool
val tryDestRefTupleExpr : Expr -> Exprs

val mkAnyTupledTy : TcGlobals -> TupInfo -> TType list -> TType

val mkAnyTupled : TcGlobals -> range -> TupInfo -> Exprs -> TType list -> Expr 
val mkRefTupled : TcGlobals -> range -> Exprs -> TType list -> Expr 
val mkRefTupledNoTypes : TcGlobals -> range -> Exprs -> Expr 
val mkRefTupledTy : TcGlobals -> TType list -> TType
val mkRefTupledVarsTy : TcGlobals -> Val list -> TType
val mkRefTupledVars : TcGlobals -> range -> Val list -> Expr 
val mkMethodTy : TcGlobals -> TType list list -> TType -> TType

//-------------------------------------------------------------------------
// 
//------------------------------------------------------------------------- 

val AdjustValForExpectedArity : TcGlobals -> range -> ValRef -> ValUseFlag -> ValReprInfo -> Expr * TType
val AdjustValToTopVal : Val -> ParentRef -> ValReprInfo -> unit
val LinearizeTopMatch : TcGlobals -> ParentRef -> Expr -> Expr
val AdjustPossibleSubsumptionExpr : TcGlobals -> Expr -> Exprs -> (Expr * Exprs) option
val NormalizeAndAdjustPossibleSubsumptionExprs : TcGlobals -> Expr -> Expr

//-------------------------------------------------------------------------
// XmlDoc signatures, used by both VS mode and XML-help emit
//------------------------------------------------------------------------- 

val buildAccessPath : CompilationPath option -> string

val XmlDocArgsEnc : TcGlobals -> Typars * Typars -> TType list -> string
val XmlDocSigOfVal : TcGlobals -> string -> Val -> string
val XmlDocSigOfUnionCase : (string list -> string)
val XmlDocSigOfField : (string list -> string)
val XmlDocSigOfProperty : (string list -> string)
val XmlDocSigOfTycon : (string list -> string)
val XmlDocSigOfSubModul : (string list -> string)
val XmlDocSigOfEntity : EntityRef -> string

//---------------------------------------------------------------------------
// Resolve static optimizations
//------------------------------------------------------------------------- 
type StaticOptimizationAnswer = 
    | Yes = 1y
    | No = -1y
    | Unknown = 0y
val DecideStaticOptimizations : TcGlobals -> StaticOptimization list -> StaticOptimizationAnswer
val mkStaticOptimizationExpr     : TcGlobals -> StaticOptimization list * Expr * Expr * range -> Expr

//---------------------------------------------------------------------------
// Build for loops
//------------------------------------------------------------------------- 

val mkFastForLoop : TcGlobals -> SequencePointInfoForForLoop * range * Val * Expr * bool * Expr * Expr -> Expr

//---------------------------------------------------------------------------
// Active pattern helpers
//------------------------------------------------------------------------- 

type ActivePatternElemRef with 
    member Name : string

val TryGetActivePatternInfo  : ValRef -> PrettyNaming.ActivePatternInfo option
val mkChoiceCaseRef : TcGlobals -> range -> int -> int -> UnionCaseRef

type PrettyNaming.ActivePatternInfo with 
    member Names : string list 

    member ResultType : TcGlobals -> range -> TType list -> TType
    member OverallType : TcGlobals -> range -> TType -> TType list -> TType

val doesActivePatternHaveFreeTypars : TcGlobals -> ValRef -> bool

//---------------------------------------------------------------------------
// Structural rewrites
//------------------------------------------------------------------------- 

[<NoEquality; NoComparison>]
type ExprRewritingEnv = 
    {PreIntercept: ((Expr -> Expr) -> Expr -> Expr option) option;
     PostTransform: Expr -> Expr option;
     PreInterceptBinding: ((Expr -> Expr) -> Binding -> Binding option) option;
     IsUnderQuotations: bool }    

val RewriteExpr : ExprRewritingEnv -> Expr -> Expr
val RewriteImplFile : ExprRewritingEnv -> TypedImplFile -> TypedImplFile

val IsGenericValWithGenericContraints: TcGlobals -> Val -> bool

type Entity with 
    member HasInterface : TcGlobals -> TType -> bool
    member HasOverride : TcGlobals -> string -> TType list -> bool
    member HasMember : TcGlobals -> string -> TType list -> bool

type EntityRef with 
    member HasInterface : TcGlobals -> TType -> bool
    member HasOverride : TcGlobals -> string -> TType list -> bool
    member HasMember : TcGlobals -> string -> TType list -> bool

val (|AttribBitwiseOrExpr|_|) : TcGlobals -> Expr -> (Expr * Expr) option
val (|EnumExpr|_|) : TcGlobals -> Expr -> Expr option
val (|TypeOfExpr|_|) : TcGlobals -> Expr -> TType option
val (|TypeDefOfExpr|_|) : TcGlobals -> Expr -> TType option

val EvalLiteralExprOrAttribArg: TcGlobals -> Expr -> Expr
val EvaledAttribExprEquality : TcGlobals -> Expr -> Expr -> bool
val IsSimpleSyntacticConstantExpr: TcGlobals -> Expr -> bool

val (|ConstToILFieldInit|_|): Const -> ILFieldInit option

val (|ExtractAttribNamedArg|_|) : string -> AttribNamedArg list -> AttribExpr option 
val (|AttribInt32Arg|_|) : AttribExpr -> int32 option
val (|AttribInt16Arg|_|) : AttribExpr -> int16 option
val (|AttribBoolArg|_|) : AttribExpr -> bool option
val (|AttribStringArg|_|) : AttribExpr -> string option
val (|Int32Expr|_|) : Expr -> int32 option


/// Determines types that are potentially known to satisfy the 'comparable' constraint and returns
/// a set of residual types that must also satisfy the constraint
val (|SpecialComparableHeadType|_|) : TcGlobals -> TType -> TType list option
val (|SpecialEquatableHeadType|_|) : TcGlobals -> TType -> TType list option
val (|SpecialNotEquatableHeadType|_|) : TcGlobals -> TType -> unit option

type OptimizeForExpressionOptions = OptimizeIntRangesOnly | OptimizeAllForExpressions
val DetectAndOptimizeForExpression : TcGlobals -> OptimizeForExpressionOptions -> Expr -> Expr

val TryEliminateDesugaredConstants : TcGlobals -> range -> Const -> Expr option

val MemberIsExplicitImpl : TcGlobals -> ValMemberInfo -> bool
val ValIsExplicitImpl : TcGlobals -> Val -> bool
val ValRefIsExplicitImpl : TcGlobals -> ValRef -> bool

val (|LinearMatchExpr|_|) : Expr -> (SequencePointInfoForBinding * range * DecisionTree * DecisionTreeTarget * Expr * SequencePointInfoForTarget * range * TType) option
val rebuildLinearMatchExpr : (SequencePointInfoForBinding * range * DecisionTree * DecisionTreeTarget * Expr * SequencePointInfoForTarget * range * TType) -> Expr

val mkCoerceIfNeeded : TcGlobals -> tgtTy: TType -> srcTy: TType -> Expr -> Expr

val (|InnerExprPat|) : Expr -> Expr

val allValsOfModDef : ModuleOrNamespaceExpr -> seq<Val>

val BindUnitVars : TcGlobals -> (Val list * ArgReprInfo list * Expr) -> Val list * Expr
