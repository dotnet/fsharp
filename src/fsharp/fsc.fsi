// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Driver 

open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TypeChecker

/// the F# project system calls this to determine if there are type provider assemblies
val internal runFromCommandLineToImportingAssemblies : (string -> unit) * string[] * string * string * Exiter -> unit

#if NO_COMPILER_BACKEND
#else

[<Class>]
type ErrorLoggerThatAccumulatesErrors = 
    inherit ErrorLogger
    new : TcConfigBuilder -> ErrorLoggerThatAccumulatesErrors
    new : TcConfig -> ErrorLoggerThatAccumulatesErrors
    member GetMessages : unit -> (bool * string) list
    member ProcessMessage : PhasedError * bool -> (bool * string) option


/// fsc.exe calls this
val mainCompile : argv : string[] * bannerAlreadyPrinted : bool * exiter : Exiter -> unit

type CompilationOutput = 
    {
        Errors : seq<ErrorOrWarning>
        Warnings : seq<ErrorOrWarning>
    }

type InProcCompiler = 
    new : unit -> InProcCompiler
    member Compile : args : string[] -> bool * CompilationOutput

#endif
