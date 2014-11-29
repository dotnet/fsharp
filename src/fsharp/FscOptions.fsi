// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.FscOptions

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Tast
#if NO_COMPILER_BACKEND
#else
open Microsoft.FSharp.Compiler.IlxGen
#endif
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Optimizer
open Microsoft.FSharp.Compiler.TcGlobals

val DisplayBannerText : TcConfigBuilder -> unit

//val GetCompilerOptions : TcConfigBuilder -> CompilerOption list -> CompilerOption list
val GetCoreFscCompilerOptions     : TcConfigBuilder -> CompilerOptionBlock list
val GetCoreFsiCompilerOptions     : TcConfigBuilder -> CompilerOptionBlock list
val GetCoreServiceCompilerOptions : TcConfigBuilder -> CompilerOptionBlock list

// Expose the "setters" for some user switches, to enable setting of defaults
val SetOptimizeSwitch : TcConfigBuilder -> OptionSwitch -> unit
val SetTailcallSwitch : TcConfigBuilder -> OptionSwitch -> unit
val SetDebugSwitch    : TcConfigBuilder -> string option -> OptionSwitch -> unit
val PrintOptionInfo   : TcConfigBuilder -> unit

val fsharpModuleName : CompilerTarget -> string -> string

#if NO_COMPILER_BACKEND
#else
val InitialOptimizationEnv : TcImports -> TcGlobals -> IncrementalOptimizationEnv
val AddExternalCcuToOpimizationEnv : TcGlobals -> IncrementalOptimizationEnv -> ImportedAssembly -> IncrementalOptimizationEnv
val ApplyAllOptimizations : TcConfig * TcGlobals * ConstraintSolver.TcValF * string * ImportMap * bool * IncrementalOptimizationEnv * CcuThunk * TypedAssembly -> TypedAssembly * Optimizer.LazyModuleInfo * IncrementalOptimizationEnv

val CreateIlxAssemblyGenerator : TcConfig * TcImports * TcGlobals * ConstraintSolver.TcValF * CcuThunk -> IlxAssemblyGenerator

val GenerateIlxCode : IlxGenBackend * bool * bool * TcConfig * TypeChecker.TopAttribs * TypedAssembly * string * bool * IlxAssemblyGenerator -> IlxGenResults
#endif

// Used during static linking
val NormalizeAssemblyRefs : TcImports -> (AbstractIL.IL.ILScopeRef -> AbstractIL.IL.ILScopeRef)

// Miscellany
val ignoreFailureOnMono1_1_16 : (unit -> unit) -> unit
val mutable enableConsoleColoring : bool
val DoWithErrorColor : bool -> (unit -> 'a) -> 'a
val ReportTime : TcConfig -> string -> unit
val abbrevFlagSet : TcConfigBuilder -> bool -> Set<string>
val PostProcessCompilerArgs : string Set -> string [] -> string list
