// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

type internal LogEditorFunctionId =
    | Classification_Semantic = 1
    | Classification_Syntactic = 2
    | LanguageService_HandleCommandLineArgs = 3
    | LanguageService_UpdateProjectInfo = 4
    | Completion_ShouldTrigger = 5
    | Completion_ProvideCompletionsAsync = 6
    | Completion_GetDescriptionAsync = 7
    | Completion_GetChangeAsync = 9

[<RequireQualifiedAccess>]
module internal Logger =

    val Log: LogEditorFunctionId -> unit

    val LogMessage: message: string -> LogEditorFunctionId -> unit

    val LogBlockStart: LogEditorFunctionId -> unit

    val LogBlockStop: LogEditorFunctionId -> unit

    val LogBlockMessageStart: message: string -> LogEditorFunctionId -> unit

    val LogBlockMessageStop: message: string -> LogEditorFunctionId -> unit

    val LogBlock: LogEditorFunctionId -> IDisposable

    val LogBlockMessage: message: string -> LogEditorFunctionId -> IDisposable
