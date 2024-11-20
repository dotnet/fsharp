namespace FSharp.Test

open System
open Xunit.Sdk
open Xunit.Abstractions

open TestFramework

/// Installs console support for parallel test runs.
type FSharpXunitFramework(sink: IMessageSink) =
    inherit XunitTestFramework(sink)
    do
        // Because xUnit v2 lacks assembly fixture, the next best place to ensure things get called
        // right at the start of the test run is here in the constructor.
        // This gets executed once per test assembly.
        MessageSink.sinkWriter |> ignore
        TestConsole.install()

    interface IDisposable with
        member _.Dispose() =
            cleanUpTemporaryDirectoryOfThisTestRun ()
            base.Dispose()       

