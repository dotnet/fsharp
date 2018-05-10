// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

// FIXME: We cannot make this internal yet until F# gets a compiler switch to make cases public when the type itself is internal.
// https://github.com/Microsoft/visualfsharp/issues/4821
type (* internal *) LogEditorFunctionId =
    | SemanticClassification = 1
    | SyntacticClassification = 2
    | HandleCommandLineArgs = 3
    | Completion_ShouldTrigger = 4
    | Completion_ProvideCompletionsAsync = 5
    | Completion_GetDescriptionAsync = 6
    | Completion_GetChangeAsync = 7

[<RequireQualifiedAccess>]
module internal Logger =

    val Log : LogEditorFunctionId -> unit

    val LogBlockStart : LogEditorFunctionId -> unit

    val LogBlockStop : LogEditorFunctionId -> unit

    val LogBlock : LogEditorFunctionId -> IDisposable

    val LogBlockMessage : message: string -> LogEditorFunctionId -> IDisposable
