// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.IlxGen
open FSharp.Compiler.Import
open FSharp.Compiler.Optimizer
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val GetGeneratedILModuleName: CompilerTarget -> string -> string

val GetInitialOptimizationEnv: TcImports * TcGlobals -> IncrementalOptimizationEnv

val AddExternalCcuToOptimizationEnv:
    TcGlobals -> IncrementalOptimizationEnv -> ImportedAssembly -> IncrementalOptimizationEnv

val ApplyAllOptimizations:
    tcConfig: TcConfig *
    tcGlobals: TcGlobals *
    tcVal: ConstraintSolver.TcValF *
    outfile: string *
    importMap: ImportMap *
    isIncrementalFragment: bool *
    optEnv: IncrementalOptimizationEnv *
    ccu: CcuThunk *
    implFiles: CheckedImplFile list ->
        CheckedAssemblyAfterOptimization * LazyModuleInfo * IncrementalOptimizationEnv

val CreateIlxAssemblyGenerator:
    TcConfig * TcImports * TcGlobals * ConstraintSolver.TcValF * CcuThunk -> IlxAssemblyGenerator

val GenerateIlxCode:
    ilxBackend: IlxGenBackend *
    isInteractiveItExpr: bool *
    isInteractiveOnMono: bool *
    tcConfig: TcConfig *
    topAttrs: TopAttribs *
    optimizedImpls: CheckedAssemblyAfterOptimization *
    fragName: string *
    ilxGenerator: IlxAssemblyGenerator ->
        IlxGenResults

// Used during static linking
val NormalizeAssemblyRefs: CompilationThreadToken * ILGlobals * TcImports -> (ILScopeRef -> ILScopeRef)

val GetGeneratedILModuleName: CompilerTarget -> string -> string
