// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Optimizer

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 

type OptimizationSettings = 
    { abstractBigTargets : bool
      jitOptUser : bool option
      localOptUser : bool option
      crossModuleOptUser : bool option
      bigTargetSize : int
      veryBigExprSize : int 
      lambdaInlineThreshold : int
      reportingPhase : bool;
      reportNoNeedToTailcall: bool
      reportFunctionSizes : bool
      reportHasEffect : bool
      reportTotalSizes : bool }

    member jitOpt : unit -> bool 
    member localOpt : unit -> bool 
    static member Defaults : OptimizationSettings

/// Optimization information 
type ModuleInfo
type LazyModuleInfo = Lazy<ModuleInfo>
type ImplFileOptimizationInfo = LazyModuleInfo
type CcuOptimizationInfo = LazyModuleInfo

#if NO_COMPILER_BACKEND
#else
[<Sealed>]
type IncrementalOptimizationEnv =
    static member Empty : IncrementalOptimizationEnv

/// For building optimization environments incrementally 
val internal BindCcu : CcuThunk -> CcuOptimizationInfo -> IncrementalOptimizationEnv -> TcGlobals -> IncrementalOptimizationEnv

/// Optimize one implementation file in the given environment
val internal OptimizeImplFile : OptimizationSettings *  CcuThunk * TcGlobals * ConstraintSolver.TcValF * Import.ImportMap * IncrementalOptimizationEnv * isIncrementalFragment: bool * emitTaicalls: bool * SignatureHidingInfo * TypedImplFile -> IncrementalOptimizationEnv * TypedImplFile * ImplFileOptimizationInfo * SignatureHidingInfo

#if DEBUG
/// Displaying optimization data
val internal moduleInfoL : TcGlobals -> LazyModuleInfo -> Layout.layout
#endif

/// Saving and re-reading optimization information 
val p_CcuOptimizationInfo : CcuOptimizationInfo -> TastPickle.WriterState -> unit 

/// Rewrite the module info using the export remapping 
val RemapOptimizationInfo : TcGlobals -> Tastops.Remap -> (CcuOptimizationInfo -> CcuOptimizationInfo)

/// Ensure that 'internal' items are not exported in the optimization info
val AbstractOptimizationInfoToEssentials : (CcuOptimizationInfo -> CcuOptimizationInfo)

/// Combine optimization infos
val UnionOptimizationInfos: seq<ImplFileOptimizationInfo> -> CcuOptimizationInfo

/// Check if an expression has an effect
val ExprHasEffect: TcGlobals -> Expr -> bool
#endif

val internal u_CcuOptimizationInfo : TastPickle.ReaderState -> CcuOptimizationInfo
