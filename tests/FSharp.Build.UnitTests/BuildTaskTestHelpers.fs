// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open FSharp.Test.ReflectionHelper

#nowarn "1182" //Unused arguments

/// A minimal IBuildEngine shared across FSharp.Build task tests, capturing logged events.
type MockEngine() =
    member val Errors = ResizeArray() with get
    member val Warnings = ResizeArray() with get
    member val Custom = ResizeArray() with get
    member val Messages = ResizeArray() with get

    interface IBuildEngine with

        member _.BuildProjectFile(projectFileName: string, targetNames: string [], globalProperties: System.Collections.IDictionary, targetOutputs: System.Collections.IDictionary): bool =
            failwith "Not Implemented"

        member _.ColumnNumberOfTaskNode: int = 0

        member _.ContinueOnError = true

        member _.LineNumberOfTaskNode: int = 0

        member this.LogCustomEvent(e: CustomBuildEventArgs): unit =
            this.Custom.Add e

        member this.LogErrorEvent(e: BuildErrorEventArgs): unit =
            this.Errors.Add e

        member this.LogMessageEvent(e: BuildMessageEventArgs): unit =
            this.Messages.Add e

        member this.LogWarningEvent(e: BuildWarningEventArgs): unit =
            this.Warnings.Add e

        member _.ProjectFileOfTaskNode: string = ""

module BuildTaskTestHelpers =

    /// Creates a TaskEnvironment rooted at a fresh temporary directory, returned alongside it.
    let createTaskEnvironmentInTemporaryDirectory () =
        let directory = TestFramework.createTemporaryDirectory ()
        let environment = TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(directory.FullName)
        environment, directory

    let assignTaskEnvironment (task: IMultiThreadableTask) (environment: TaskEnvironment) =
        task.TaskEnvironment <- environment

    // TaskEnvironment.Dispose (which releases the thread-local working-directory override) is internal
    // and TaskEnvironment is not IDisposable, so reflection is the only deterministic way to invoke it.
    let private taskEnvironmentDisposeMethod =
        getPrivateInstanceMethod "Dispose" typeof<TaskEnvironment>

    let disposeTaskEnvironment (environment: TaskEnvironment) =
        taskEnvironmentDisposeMethod.Invoke(environment, null) |> ignore

    /// Runs `body` against a fresh temporary-directory-rooted TaskEnvironment, disposing it afterwards
    /// even if `body` throws.
    let withTaskEnvironment body =
        let environment, directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            body environment directory
        finally
            disposeTaskEnvironment environment

    /// As withTaskEnvironment but with two independent environments; nested try/finally disposes both
    /// (second then first) even if the second fails to construct.
    let withTaskEnvironmentPair body =
        let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()

        try
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                body environmentA directoryA environmentB directoryB
            finally
                disposeTaskEnvironment environmentB
        finally
            disposeTaskEnvironment environmentA
