// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework

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
            failwith "Not Implemented"

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
