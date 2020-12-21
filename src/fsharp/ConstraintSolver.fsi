// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Solves constraints using a mutable constraint-solver state
module internal FSharp.Compiler.ConstraintSolver

open FSharp.Compiler 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// Create a type variable representing the use of a "_" in F# code
val NewAnonTypar: TyparKind * Range * TyparRigidity * TyparStaticReq * TyparDynamicReq -> Typar

/// Create an inference type variable 
val NewInferenceType: unit -> TType

/// Create an inference type variable for the kind of a byref pointer
val NewByRefKindInferenceType: TcGlobals -> Range -> TType

/// Create an inference type variable representing an error condition when checking an expression
val NewErrorType: unit -> TType

/// Create an inference type variable representing an error condition when checking a measure
val NewErrorMeasure: unit -> Measure

/// Create a list of inference type variables, one for each element in the input list
val NewInferenceTypes: 'a list -> TType list

/// Given a set of formal type parameters and their constraints, make new inference type variables for
/// each and ensure that the constraints on the new type variables are adjusted to refer to these.
val FreshenAndFixupTypars: Range -> TyparRigidity -> Typars -> TType list -> Typars -> Typars * TyparInst * TType list

val FreshenTypeInst: Range -> Typars -> Typars * TyparInst * TType list

val FreshenTypars: Range -> Typars -> TType list

val FreshenMethInfo: Range -> MethInfo -> TType list

[<RequireQualifiedAccess>] 
/// Information about the context of a type equation.
type ContextInfo =

    /// No context was given.
    | NoContext

    /// The type equation comes from an IF expression.
    | IfExpression of Range

    /// The type equation comes from an omitted else branch.
    | OmittedElseBranch of Range

    /// The type equation comes from a type check of the result of an else branch.
    | ElseBranchResult of Range

    /// The type equation comes from the verification of record fields.
    | RecordFields

    /// The type equation comes from the verification of a tuple in record fields.
    | TupleInRecordFields

    /// The type equation comes from a list or array constructor
    | CollectionElement of bool * Range

    /// The type equation comes from a return in a computation expression.
    | ReturnInComputationExpression

    /// The type equation comes from a yield in a computation expression.
    | YieldInComputationExpression

    /// The type equation comes from a runtime type test.
    | RuntimeTypeTest of bool

    /// The type equation comes from an downcast where a upcast could be used.
    | DowncastUsedInsteadOfUpcast of bool

    /// The type equation comes from a return type of a pattern match clause (not the first clause).
    | FollowingPatternMatchClause of Range

    /// The type equation comes from a pattern match guard.
    | PatternMatchGuard of Range

    /// The type equation comes from a sequence expression.
    | SequenceExpression of TType

/// Captures relevant information for a particular failed overload resolution.
type OverloadInformation = 
    {
        methodSlot: CalledMeth<Expr>
        amap : ImportMap
        error: exn
    }

/// Cases for overload resolution failure that exists in the implementation of the compiler.
type OverloadResolutionFailure =
  | NoOverloadsFound   of methodName: string
                        * candidates: OverloadInformation list 
                        * cx: TraitConstraintInfo option
  | PossibleCandidates of methodName: string 
                        * candidates: OverloadInformation list // methodNames may be different (with operators?), this is refactored from original logic to assemble overload failure message
                        * cx: TraitConstraintInfo option

exception ConstraintSolverTupleDiffLengths              of displayEnv: DisplayEnv * TType list * TType list * Range * Range
exception ConstraintSolverInfiniteTypes                 of displayEnv: DisplayEnv * contextInfo: ContextInfo * TType * TType * Range * Range
exception ConstraintSolverTypesNotInEqualityRelation    of displayEnv: DisplayEnv * TType * TType * Range * Range * ContextInfo
exception ConstraintSolverTypesNotInSubsumptionRelation of displayEnv: DisplayEnv * argTy: TType * paramTy: TType * callRange: Range * parameterRange: Range
exception ConstraintSolverMissingConstraint             of displayEnv: DisplayEnv * Typar * TyparConstraint * Range * Range
exception ConstraintSolverError                         of string * Range * Range
exception ConstraintSolverRelatedInformation            of string option * Range * exn

exception ErrorFromApplyingDefault              of tcGlobals: TcGlobals * displayEnv: DisplayEnv * Typar * TType * exn * Range
exception ErrorFromAddingTypeEquation           of tcGlobals: TcGlobals * displayEnv: DisplayEnv * actualTy: TType * expectedTy: TType * exn * Range
exception ErrorsFromAddingSubsumptionConstraint of tcGlobals: TcGlobals * displayEnv: DisplayEnv * actualTy: TType * expectedTy: TType * exn * ContextInfo * parameterRange: Range
exception ErrorFromAddingConstraint             of displayEnv: DisplayEnv * exn * Range
exception UnresolvedConversionOperator          of displayEnv: DisplayEnv * TType * TType * Range
exception UnresolvedOverloading                 of displayEnv: DisplayEnv * callerArgs: CallerArgs<Expr> * failure: OverloadResolutionFailure * Range
exception NonRigidTypar                         of displayEnv: DisplayEnv * string option * Range * TType * TType * Range

exception ArgDoesNotMatchError                  of error: ErrorsFromAddingSubsumptionConstraint * calledMeth: CalledMeth<Expr> * calledArg: CalledArg * callerArg: CallerArg<Expr>
/// A function that denotes captured tcVal, Used in constraint solver and elsewhere to get appropriate expressions for a ValRef.
type TcValF = (ValRef -> ValUseFlag -> TType list -> Range -> Expr * TType)

[<Sealed>]
type ConstraintSolverState =
    static member New: TcGlobals * Import.ImportMap * InfoReader * TcValF -> ConstraintSolverState

val BakedInTraitConstraintNames: Set<string>

[<Sealed; NoEquality; NoComparison>]
type Trace 

type OptionalTrace =
    | NoTrace
    | WithTrace of Trace

val SimplifyMeasuresInTypeScheme: TcGlobals -> bool -> Typars -> TType -> TyparConstraint list -> Typars

val ResolveOverloadingForCall: DisplayEnv -> ConstraintSolverState -> Range -> methodName: string -> ndeep: int -> cx: TraitConstraintInfo option -> callerArgs: CallerArgs<Expr> -> AccessorDomain -> calledMethGroup: CalledMeth<Expr> list -> permitOptArgs: bool -> reqdRetTyOpt: TType option -> CalledMeth<Expr> option * OperationResult<unit>

val UnifyUniqueOverloading: DisplayEnv -> ConstraintSolverState -> Range -> int * int -> string -> AccessorDomain -> CalledMeth<SynExpr> list -> TType -> OperationResult<bool> 

/// Remove the global constraints where these type variables appear in the support of the constraint 
val EliminateConstraintsForGeneralizedTypars: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> Typars -> unit 

val CheckDeclaredTypars: DisplayEnv -> ConstraintSolverState -> Range -> Typars -> Typars -> unit 

val AddCxTypeEqualsType: ContextInfo -> DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> unit

val AddCxTypeEqualsTypeUndoIfFailed: DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> bool

val AddCxTypeEqualsTypeUndoIfFailedOrWarnings: DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> bool

val AddCxTypeEqualsTypeMatchingOnlyUndoIfFailed: DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> bool

val AddCxTypeMustSubsumeType: ContextInfo -> DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> TType -> unit

val AddCxTypeMustSubsumeTypeUndoIfFailed: DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> bool

val AddCxTypeMustSubsumeTypeMatchingOnlyUndoIfFailed: DisplayEnv -> ConstraintSolverState -> Range -> TType -> TType -> bool

val AddCxMethodConstraint: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TraitConstraintInfo -> unit

val AddCxTypeMustSupportNull: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeMustSupportComparison: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeMustSupportEquality: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeMustSupportDefaultCtor: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeIsReferenceType: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeIsValueType: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeIsUnmanaged: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> unit

val AddCxTypeIsEnum: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> TType -> unit

val AddCxTypeIsDelegate: DisplayEnv -> ConstraintSolverState -> Range -> OptionalTrace -> TType -> TType -> TType -> unit

val AddCxTyparDefaultsTo:  DisplayEnv -> ConstraintSolverState -> Range -> ContextInfo -> Typar -> int -> TType -> unit

val SolveTypeAsError: DisplayEnv -> ConstraintSolverState -> Range -> TType -> unit

val ApplyTyparDefaultAtPriority: DisplayEnv -> ConstraintSolverState -> priority: int -> Typar -> unit

/// Generate a witness expression if none is otherwise available, e.g. in legacy non-witness-passing code
val CodegenWitnessExprForTraitConstraint : TcValF -> TcGlobals -> ImportMap -> Range -> TraitConstraintInfo -> Expr list -> OperationResult<Expr option>

/// Generate the arguments passed when using a generic construct that accepts traits witnesses
val CodegenWitnessesForTyparInst : TcValF -> TcGlobals -> ImportMap -> Range -> Typars -> TType list -> OperationResult<Choice<TraitConstraintInfo, Expr> list>

/// Generate the lambda argument passed for a use of a generic construct that accepts trait witnesses
val CodegenWitnessArgForTraitConstraint : TcValF -> TcGlobals -> ImportMap -> Range -> TraitConstraintInfo -> OperationResult<Choice<TraitConstraintInfo, Expr>>

/// For some code like "let f() = ([] = [])", a free choice is made for a type parameter
/// for an interior type variable.  This chooses a solution for a type parameter subject
/// to its constraints and applies that solution by using a constraint.
val ChooseTyparSolutionAndSolve : ConstraintSolverState -> DisplayEnv -> Typar -> unit

val IsApplicableMethApprox: TcGlobals -> ImportMap -> Range -> MethInfo -> TType -> bool

val CanonicalizePartialInferenceProblem:  ConstraintSolverState -> DisplayEnv -> Range -> Typars -> unit
