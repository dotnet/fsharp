// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler

open System

// FIXME: We cannot make this internal yet until F# gets a compiler switch to make cases public when the type itself is internal.
// https://github.com/Microsoft/visualfsharp/issues/4821
type (* internal *) LogCompilerFunctionId =
    | Service_ParseAndCheckFileInProject = 1
    | Service_CheckOneFile = 2
    | Service_IncrementalBuildersCache_BuildingNewCache = 3
    | Service_IncrementalBuildersCache_GettingCache = 4
    | CompileOps_TypeCheckOneInputAndFinishEventually = 5

[<RequireQualifiedAccess>]
module internal Logger =

    val Log : LogCompilerFunctionId -> unit

    val LogMessage : message: string -> LogCompilerFunctionId -> unit

    val LogBlockStart : LogCompilerFunctionId -> unit

    val LogBlockStop : LogCompilerFunctionId -> unit

    val LogBlockMessageStart : message: string -> LogCompilerFunctionId -> unit

    val LogBlockMessageStop : message: string -> LogCompilerFunctionId -> unit

    val LogBlock : LogCompilerFunctionId -> IDisposable

    val LogBlockMessage : message: string -> LogCompilerFunctionId -> IDisposable
