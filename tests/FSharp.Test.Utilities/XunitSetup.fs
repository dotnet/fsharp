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
        logConfig initialConfig

module XUnitSetup =

    [<assembly: AssemblyFixture(typeof<FSharpTestAssemblyFixture>); CaptureConsole; CaptureTrace>]
    do ()

/// Modules/Types included in this Collection (via `[<Collection(nameof NotThreadSafeResourceCollection>`)):
/// 1. do not run concurrently with other tests or modules in the collection (typical behavior)
/// 2. run entirely isolated from all other tests in a given test run (including ones not included in a Collection) due to `DisableParallelization = true`
/// see https://github.com/xunit/xunit/issues/1999#issuecomment-522635397
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection() = class end

namespace Xunit

#nowarn "1182" // the DisableParallelization properties are unused as these are shims waiting for us to move to xunit3 >= 4

// Shim to be deleted when xunit dependency updates to >= 4
[<System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)>]
type TestClassAttribute(DisableParallelization: bool) =
    inherit System.Attribute()

// Shim to be removed and replaced with direct usage of FactAttribute from xunit >= 4 when xunit dependency updates to >= 4
[<System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)>]
type Fact4Attribute(DisableParallelization: bool) =
    inherit FactAttribute()
