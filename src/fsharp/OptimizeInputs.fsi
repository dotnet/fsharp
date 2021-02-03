// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.IlxGen
open FSharp.Compiler.Import
open FSharp.Compiler.Optimizer
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.CheckDeclarations

val GetGeneratedILModuleName : CompilerTarget -> string -> string

val GetInitialOptimizationEnv : TcImports * TcGlobals -> IncrementalOptimizationEnv

val AddExternalCcuToOptimizationEnv : TcGlobals -> IncrementalOptimizationEnv -> ImportedAssembly -> IncrementalOptimizationEnv

val ApplyAllOptimizations : TcConfig * TcGlobals * ConstraintSolver.TcValF * string * ImportMap * bool * IncrementalOptimizationEnv * CcuThunk * TypedImplFile list -> TypedAssemblyAfterOptimization * Optimizer.LazyModuleInfo * IncrementalOptimizationEnv 

val CreateIlxAssemblyGenerator : TcConfig * TcImports * TcGlobals * ConstraintSolver.TcValF * CcuThunk -> IlxAssemblyGenerator

val GenerateIlxCode : IlxGen.IlxGenBackend * isInteractiveItExpr:bool * isInteractiveOnMono:bool * TcConfig * TopAttribs * TypedAssemblyAfterOptimization * fragName:string * IlxGen.IlxAssemblyGenerator -> IlxGenResults

// Used during static linking
val NormalizeAssemblyRefs : CompilationThreadToken * ILGlobals * TcImports -> (ILScopeRef -> ILScopeRef)

val GetGeneratedILModuleName : CompilerTarget -> string -> string
