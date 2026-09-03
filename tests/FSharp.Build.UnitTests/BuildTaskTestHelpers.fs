// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open FSharp.Test.ReflectionHelper

#nowarn "1182" //Unused arguments

/// A minimal IBuildEngine shared across FSharp.Build task tests, capturing logged errors,
/// warnings, messages, and custom events without needing a real MSBuild engine.
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

    /// Creates a TaskEnvironment rooted at a freshly created temporary directory (via
    /// TestFramework.createTemporaryDirectory) and returns it together with that directory.
    let createTaskEnvironmentInTemporaryDirectory () =
        let directory = TestFramework.createTemporaryDirectory ()
        let environment = TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(directory.FullName)
        environment, directory

    /// Assigns a TaskEnvironment to any task participating in the multi-threaded task execution
    /// model, mirroring how MSBuild wires up IMultiThreadableTask.TaskEnvironment before Execute().
    let assignTaskEnvironment (task: IMultiThreadableTask) (environment: TaskEnvironment) =
        task.TaskEnvironment <- environment

    /// TaskEnvironment.Dispose(), which releases the thread-local working-directory override held by
    /// a TaskEnvironment created via CreateWithProjectDirectoryAndEnvironment, is `internal` in
    /// Microsoft.Build.Framework: TaskEnvironment does not implement IDisposable, so `use`/`Dispose()`
    /// aren't available from this assembly. Reflection is the only way to invoke it deterministically
    /// rather than leaving cleanup to the finalizer.
    let private taskEnvironmentDisposeMethod =
        getPrivateInstanceMethod "Dispose" typeof<TaskEnvironment>

    /// Deterministically disposes a TaskEnvironment created by createTaskEnvironmentInTemporaryDirectory.
    /// Callers must invoke this (typically in a `finally` block) for every such environment.
    let disposeTaskEnvironment (environment: TaskEnvironment) =
        taskEnvironmentDisposeMethod.Invoke(environment, null) |> ignore

    /// Creates a temporary-directory-rooted TaskEnvironment, runs `body` against it, and deterministically
    /// disposes the environment afterwards (even if `body` throws). Scoped narrowly to the environments
    /// produced by createTaskEnvironmentInTemporaryDirectory; not a general-purpose disposable combinator.
    let withTaskEnvironment body =
        let environment, directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            body environment directory
        finally
            disposeTaskEnvironment environment

    /// Creates two temporary-directory-rooted TaskEnvironments, runs `body` against both, and disposes
    /// them afterwards. Nested try/finally guarantees the first environment is disposed even if the second
    /// fails to construct, and that both are disposed (second then first) once `body` completes. Scoped
    /// narrowly to createTaskEnvironmentInTemporaryDirectory environments; not a general disposable combinator.
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
