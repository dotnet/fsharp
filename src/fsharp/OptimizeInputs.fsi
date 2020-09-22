// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.Import
open FSharp.Compiler.Optimizer
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val GetGeneratedILModuleName : CompilerTarget -> string -> string

val GetInitialOptimizationEnv : TcImports * TcGlobals -> IncrementalOptimizationEnv

val AddExternalCcuToOptimizationEnv : TcGlobals -> IncrementalOptimizationEnv -> ImportedAssembly -> IncrementalOptimizationEnv

val ApplyAllOptimizations : TcConfig * TcGlobals * ConstraintSolver.TcValF * string * ImportMap * bool * IncrementalOptimizationEnv * CcuThunk * TypedImplFile list -> TypedAssemblyAfterOptimization * Optimizer.LazyModuleInfo * IncrementalOptimizationEnv 

val CreateIlxAssemblyGenerator : TcConfig * TcImports * TcGlobals * ConstraintSolver.TcValF * CcuThunk -> IlxGen.IlxAssemblyGenerator

val GenerateIlxCode : IlxGen.IlxGenBackend * isInteractiveItExpr:bool * isInteractiveOnMono:bool * TcConfig * TypeChecker.TopAttribs * TypedAssemblyAfterOptimization * fragName:string * IlxGen.IlxAssemblyGenerator -> IlxGen.IlxGenResults

// Used during static linking
val NormalizeAssemblyRefs : CompilationThreadToken * ILGlobals * TcImports -> (AbstractIL.IL.ILScopeRef -> AbstractIL.IL.ILScopeRef)

val GetGeneratedILModuleName : CompilerTarget -> string -> string
