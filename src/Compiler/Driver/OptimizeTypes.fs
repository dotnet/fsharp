module internal FSharp.Compiler.Service.Driver.OptimizeTypes

open FSharp.Compiler
open FSharp.Compiler.Optimizer
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

type OptimizeDuringCodeGen = bool -> Expr -> Expr
type OptimizeRes = (IncrementalOptimizationEnv * CheckedImplFile * ImplFileOptimizationInfo * SignatureHidingInfo) * OptimizeDuringCodeGen

type Optimize =
    OptimizationSettings * CcuThunk * TcGlobals * ConstraintSolver.TcValF * Import.ImportMap * IncrementalOptimizationEnv * bool * bool * bool * SignatureHidingInfo * CheckedImplFile -> OptimizeRes

type PhaseInputs = IncrementalOptimizationEnv * SignatureHidingInfo * CheckedImplFile

type Phase1Inputs = PhaseInputs
type Phase1Res = OptimizeRes
type Phase1Fun = Phase1Inputs -> Phase1Res

type Phase2Inputs = IncrementalOptimizationEnv * SignatureHidingInfo * ImplFileOptimizationInfo * CheckedImplFile
type Phase2Res = IncrementalOptimizationEnv * CheckedImplFile
type Phase2Fun = Phase2Inputs -> Phase2Res

type Phase3Inputs = PhaseInputs
type Phase3Res = IncrementalOptimizationEnv * CheckedImplFile
type Phase3Fun = Phase3Inputs -> Phase3Res

type Phase =
    | Phase1
    | Phase2
    | Phase3

module Phase =
    let all = [| Phase1; Phase2; Phase3 |]

    let prev (phase: Phase) =
        match phase with
        | Phase1 -> None
        | Phase2 -> Some Phase1
        | Phase3 -> Some Phase2

    let next (phase: Phase) =
        match phase with
        | Phase1 -> Some Phase2
        | Phase2 -> Some Phase3
        | Phase3 -> None

type PhaseRes =
    | Phase1 of Phase1Res
    | Phase2 of Phase2Res
    | Phase3 of Phase3Res

    member x.Which =
        match x with
        | Phase1 _ -> Phase.Phase1
        | Phase2 _ -> Phase.Phase2
        | Phase3 _ -> Phase.Phase3

    member x.Get1() =
        match x with
        | Phase1 x -> x
        | Phase2 _
        | Phase3 _ -> failwith $"Called {nameof (x.Get1)} but this is {x.Which}"

    member x.Get2() =
        match x with
        | Phase2 x -> x
        | Phase1 _
        | Phase3 _ -> failwith $"Called {nameof (x.Get2)} but this is {x.Which}"

type FileResultsComplete =
    {
        Phase1: Phase1Res
        Phase2: Phase2Res
        Phase3: Phase3Res
    }

type CollectorInputs = FileResultsComplete[]
type CollectorOutputs = (CheckedImplFileAfterOptimization * ImplFileOptimizationInfo)[] * IncrementalOptimizationEnv
