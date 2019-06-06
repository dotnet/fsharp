// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Solves constraints using a mutable constraint-solver state
module internal FSharp.Compiler.ConstraintSolver

open FSharp.Compiler 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Tast
open FSharp.Compiler.Range
open FSharp.Compiler.Import
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.InfoReader

/// Create a type variable representing the use of a "_" in F# code
val NewAnonTypar : TyparKind * range * TyparRigidity * TyparStaticReq * TyparDynamicReq -> Typar

/// Create an inference type variable 
val NewInferenceType : unit -> TType

/// Create an inference type variable for the kind of a byref pointer
val NewByRefKindInferenceType : TcGlobals -> range -> TType

/// Create an inference type variable representing an error condition when checking an expression
val NewErrorType : unit -> TType

/// Create an inference type variable representing an error condition when checking a measure
val NewErrorMeasure : unit -> Measure

/// Create a list of inference type variables, one for each element in the input list
val NewInferenceTypes : 'a list -> TType list

/// Given a set of formal type parameters and their constraints, make new inference type variables for
/// each and ensure that the constraints on the new type variables are adjusted to refer to these.
val FreshenAndFixupTypars : range -> TyparRigidity -> Typars -> TType list -> Typars -> Typars * TyparInst * TType list

val FreshenTypeInst : range -> Typars -> Typars * TyparInst * TType list

val FreshenTypars : range -> Typars -> TType list

val FreshenMethInfo : range -> MethInfo -> TType list

[<RequireQualifiedAccess>] 
/// Information about the context of a type equation.
type ContextInfo =
/// No context was given.
| NoContext
/// The type equation comes from an IF expression.
| IfExpression of range
/// The type equation comes from an omitted else branch.
| OmittedElseBranch of range
/// The type equation comes from a type check of the result of an else branch.
| ElseBranchResult of range
/// The type equation comes from the verification of record fields.
| RecordFields
/// The type equation comes from the verification of a tuple in record fields.
| TupleInRecordFields
/// The type equation comes from a list or array constructor
| CollectionElement of bool * range
/// The type equation comes from a return in a computation expression.
| ReturnInComputationExpression
/// The type equation comes from a yield in a computation expression.
| YieldInComputationExpression
/// The type equation comes from a runtime type test.
| RuntimeTypeTest of bool
/// The type equation comes from an downcast where a upcast could be used.
| DowncastUsedInsteadOfUpcast of bool
/// The type equation comes from a return type of a pattern match clause (not the first clause).
| FollowingPatternMatchClause of range
/// The type equation comes from a pattern match guard.
| PatternMatchGuard of range
/// The type equation comes from a sequence expression.
| SequenceExpression of TType

exception ConstraintSolverTupleDiffLengths              of displayEnv: DisplayEnv * TType list * TType list * range * range
exception ConstraintSolverInfiniteTypes                 of displayEnv: DisplayEnv * contextInfo: ContextInfo * TType * TType * range * range
exception ConstraintSolverTypesNotInEqualityRelation    of displayEnv: DisplayEnv * TType * TType * range * range * ContextInfo
exception ConstraintSolverTypesNotInSubsumptionRelation of displayEnv: DisplayEnv * TType * TType * range * range
exception ConstraintSolverMissingConstraint             of displayEnv: DisplayEnv * Typar * TyparConstraint * range * range
exception ConstraintSolverError                         of string * range * range
exception ConstraintSolverRelatedInformation            of string option * range * exn

exception ErrorFromApplyingDefault              of tcGlobals: TcGlobals * displayEnv: DisplayEnv * Typar * TType * exn * range
exception ErrorFromAddingTypeEquation           of tcGlobals: TcGlobals * displayEnv: DisplayEnv * TType * TType * exn * range
exception ErrorsFromAddingSubsumptionConstraint of tcGlobals: TcGlobals * displayEnv: DisplayEnv * TType * TType * exn * ContextInfo * range
exception ErrorFromAddingConstraint             of displayEnv: DisplayEnv * exn * range
exception UnresolvedConversionOperator          of displayEnv: DisplayEnv * TType * TType * range
exception PossibleOverload                      of displayEnv: DisplayEnv * string * exn * range
exception UnresolvedOverloading                 of displayEnv: DisplayEnv * exn list * string * range
exception NonRigidTypar                         of displayEnv: DisplayEnv * string option * range * TType * TType * range

/// A function that denotes captured tcVal, Used in constraint solver and elsewhere to get appropriate expressions for a ValRef.
type TcValF = (ValRef -> ValUseFlag -> TType list -> range -> Expr * TType)

[<Sealed>]
type ConstraintSolverState =
    static member New: TcGlobals * Import.ImportMap * InfoReader * TcValF -> ConstraintSolverState

type ConstraintSolverEnv 

val BakedInTraitConstraintNames : Set<string>

val MakeConstraintSolverEnv : ContextInfo -> ConstraintSolverState -> range -> DisplayEnv -> ConstraintSolverEnv

[<Sealed; NoEquality; NoComparison>]
type Trace 

type OptionalTrace =
| NoTrace
| WithTrace of Trace

val SimplifyMeasuresInTypeScheme             : TcGlobals -> bool -> Typars -> TType -> TyparConstraint list -> Typars
val SolveTyparEqualsType                     : ConstraintSolverEnv -> int -> range -> OptionalTrace -> TType -> TType -> OperationResult<unit>
val SolveTypeEqualsTypeKeepAbbrevs           : ConstraintSolverEnv -> int -> range -> OptionalTrace -> TType -> TType -> OperationResult<unit>
val CanonicalizeRelevantMemberConstraints    : ConstraintSolverEnv -> int -> OptionalTrace -> Typars -> OperationResult<unit>
val ResolveOverloading                       : ConstraintSolverEnv -> OptionalTrace -> string -> ndeep: int -> TraitConstraintInfo option -> int * int -> AccessorDomain -> CalledMeth<Expr> list ->  bool -> TType option -> CalledMeth<Expr> option * OperationResult<unit>
val UnifyUniqueOverloading                   : ConstraintSolverEnv -> int * int -> string -> AccessorDomain -> CalledMeth<SynExpr> list -> TType -> OperationResult<bool> 
val EliminateConstraintsForGeneralizedTypars : ConstraintSolverEnv -> OptionalTrace -> Typars -> unit 

val CheckDeclaredTypars                       : DisplayEnv -> ConstraintSolverState -> range -> Typars -> Typars -> unit 

val AddConstraint                             : ConstraintSolverEnv -> int -> Range.range -> OptionalTrace -> Typar -> TyparConstraint -> OperationResult<unit>
val AddCxTypeEqualsType                       : ContextInfo -> DisplayEnv -> ConstraintSolverState -> range -> TType -> TType -> unit
val AddCxTypeEqualsTypeUndoIfFailed           : DisplayEnv -> ConstraintSolverState -> range -> TType -> TType -> bool
val AddCxTypeEqualsTypeMatchingOnlyUndoIfFailed : DisplayEnv -> ConstraintSolverState -> range -> TType -> TType -> bool
val AddCxTypeMustSubsumeType                  : ContextInfo -> DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> TType -> unit
val AddCxTypeMustSubsumeTypeUndoIfFailed      : DisplayEnv -> ConstraintSolverState -> range -> TType -> TType -> bool
val AddCxTypeMustSubsumeTypeMatchingOnlyUndoIfFailed : DisplayEnv -> ConstraintSolverState -> range -> TType -> TType -> bool
val AddCxMethodConstraint                     : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TraitConstraintInfo -> unit
val AddCxTypeMustSupportNull                  : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeMustSupportComparison            : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeMustSupportEquality              : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeMustSupportDefaultCtor           : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeIsReferenceType                  : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeIsValueType                      : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeIsUnmanaged                      : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> unit
val AddCxTypeIsEnum                           : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> TType -> unit
val AddCxTypeIsDelegate                       : DisplayEnv -> ConstraintSolverState -> range -> OptionalTrace -> TType -> TType -> TType -> unit

val CodegenWitnessThatTypeSupportsTraitConstraint : TcValF -> TcGlobals -> ImportMap -> range -> TraitConstraintInfo -> Expr list -> OperationResult<Expr option>

val ChooseTyparSolutionAndSolve : ConstraintSolverState -> DisplayEnv -> Typar -> unit

val IsApplicableMethApprox : TcGlobals -> ImportMap -> range -> MethInfo -> TType -> bool
