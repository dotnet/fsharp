namespace FSharp.Test

open System
open Xunit
open TestFramework

/// xUnit3 assembly fixture: performs one-time setup for the test assembly.
/// Registered via [<assembly: AssemblyFixture(typeof<FSharpTestAssemblyFixture>)>] below.
/// The constructor is called by xUnit once before any tests in the assembly run.
type FSharpTestAssemblyFixture() =
    do
#if !NETCOREAPP
        // We need AssemblyResolver already here, because OpenTelemetry loads some assemblies dynamically.
        log "Adding AssemblyResolver"
        AssemblyResolver.addResolver()
#endif
        log $"Server GC enabled: {Runtime.GCSettings.IsServerGC}"
        log "Installing TestConsole redirection"
        TestConsole.install()
        logConfig initialConfig

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
/// see https://github.com/xunit/xunit/issues/1999#issuecomment-522635397
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection() = class end

/// Mark test cases as not safe to run in parallel with other test cases of the same test collection.
/// In case Xunit 3 enables internal parallelization of test collections.
[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method, AllowMultiple = false)>]
type RunTestCasesInSequenceAttribute() = inherit Attribute()

module XUnitSetup =

    // NOTE: CaptureTrace is disabled because it conflicts with TestConsole.ExecutionCapture
    // which is used by FSI tests to capture console output. xUnit3's trace capture intercepts
    // console output before it can reach TestConsole's redirectors.
    // [<assembly: CaptureTrace>]

    [<assembly: AssemblyFixture(typeof<FSharpTestAssemblyFixture>)>]
    do ()
