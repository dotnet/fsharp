// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler

open System

// FIXME: We cannot make this internal yet until F# gets a compiler switch to make cases public when the type itself is internal.
// https://github.com/Microsoft/visualfsharp/issues/4821
type (* internal *) LogCompilerFunctionId =
    | ParseAndCheckFileInProject = 1

[<RequireQualifiedAccess>]
module internal Logger =

    val Log : LogCompilerFunctionId -> unit

    val LogBlockStart : LogCompilerFunctionId -> unit

    val LogBlockStop : LogCompilerFunctionId -> unit

    val LogBlock : LogCompilerFunctionId -> IDisposable

    val LogBlockMessage : message: string -> LogCompilerFunctionId -> IDisposable
