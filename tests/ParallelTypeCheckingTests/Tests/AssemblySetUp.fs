namespace global
open NUnit.Framework
open ParallelTypeCheckingTests
open OpenTelemetry.Trace

/// One-time Otel setup for NUnit tests
[<SetUpFixture>]
type AssemblySetUp() =
    let mutable tracerProvider = None

    [<OneTimeSetUp>]
    member this.SetUp() =
        tracerProvider <- TestUtils.setupOtel() |> Some
        
    [<OneTimeTearDown>]
    member this.TearDown() =
        tracerProvider
        |> Option.iter (fun x ->
            x.ForceFlush() |> ignore
            x.Dispose()
        )
        tracerProvider <- None
