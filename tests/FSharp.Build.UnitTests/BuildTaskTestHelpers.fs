// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Xunit
open FSharp.Test.ReflectionHelper

#nowarn "1182"

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

    interface IBuildEngine2 with
        member _.IsRunningMultipleNodes = false
        member _.BuildProjectFile(_, _, _, _, _) = failwith "Not Implemented"
        member _.BuildProjectFilesInParallel(_, _, _, _, _, _, _) = failwith "Not Implemented"

    interface IBuildEngine3 with
        member _.BuildProjectFilesInParallel(_, _, _, _, _, _) = failwith "Not Implemented"
        member _.Yield() = ()
        member _.Reacquire() = ()

module BuildTaskTestHelpers =

    let createTaskEnvironmentInTemporaryDirectory () =
        let directory = TestFramework.createTemporaryDirectory ()
        let environment = TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(directory.FullName)
        environment, directory

    // TaskEnvironment.Dispose (which releases the thread-local working-directory override) is internal
    // and TaskEnvironment is not IDisposable, so reflection is the only deterministic way to invoke it.
    let private taskEnvironmentDisposeMethod =
        getPrivateInstanceMethod "Dispose" typeof<TaskEnvironment>

    let disposeTaskEnvironment (environment: TaskEnvironment) =
        taskEnvironmentDisposeMethod.Invoke(environment, null) |> ignore

    let assignTaskEnvironment environment (task: #IMultiThreadableTask) =
        (task :> IMultiThreadableTask).TaskEnvironment <- environment
        task

    let withTaskEnvironmentUsing create body =
        let environment, state = create ()

        try
            body environment state
        finally
            disposeTaskEnvironment environment

    let withTaskEnvironment body =
        withTaskEnvironmentUsing createTaskEnvironmentInTemporaryDirectory body

    let withTaskEnvironmentPairUsing create body =
        withTaskEnvironmentUsing create (fun environmentA stateA ->
            withTaskEnvironmentUsing create (fun environmentB stateB -> body environmentA stateA environmentB stateB))

    let runConcurrentlyWithBarrier scenario (actions: ((unit -> unit) -> 'T) list) =
        use barrier = new Barrier(List.length actions)
        let release () =
            Assert.True(barrier.SignalAndWait(TimeSpan.FromSeconds 10.0), $"{scenario}: barrier timed out")
        let tasks = [| for action in actions -> Task.Run(fun () -> action release) |]

        Assert.True(
            Task.WaitAll([| for task in tasks -> task :> Task |], TimeSpan.FromSeconds 30.0),
            $"{scenario}: concurrent executions timed out; possible deadlock."
        )

        [| for task in tasks -> task.Result |]
