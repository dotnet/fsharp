// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics

module internal Activity =

    type ActivityFacade =
        interface IDisposable
        new: Activity option -> ActivityFacade
        member AddTag: string -> #obj -> unit
        member Perform: (Activity -> unit) -> unit
        member Dispose: unit -> unit

    type ActivitySourceFacade =
        interface IDisposable
        new: ActivitySource -> ActivitySourceFacade
        member Start: string -> (string * #obj) seq -> ActivityFacade
        member StartNoTags: string -> ActivityFacade
        member Name: string
        member Dispose: unit -> unit

    val instance: ActivitySourceFacade

type internal LogCompilerFunctionId =
    | Service_ParseAndCheckFileInProject = 1
    | Service_CheckOneFile = 2
    | Service_IncrementalBuildersCache_BuildingNewCache = 3
    | Service_IncrementalBuildersCache_GettingCache = 4
    | CompileOps_TypeCheckOneInputAndFinishEventually = 5
    | IncrementalBuild_CreateItemKeyStoreAndSemanticClassification = 6
    | IncrementalBuild_TypeCheck = 7

[<RequireQualifiedAccess>]
module internal Logger =

    val Log: LogCompilerFunctionId -> unit

    val LogMessage: message: string -> LogCompilerFunctionId -> unit

    val LogBlockStart: LogCompilerFunctionId -> unit

    val LogBlockStop: LogCompilerFunctionId -> unit

    val LogBlockMessageStart: message: string -> LogCompilerFunctionId -> unit

    val LogBlockMessageStop: message: string -> LogCompilerFunctionId -> unit

    val LogBlock: LogCompilerFunctionId -> IDisposable

    val LogBlockMessage: message: string -> LogCompilerFunctionId -> IDisposable
