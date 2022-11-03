namespace global
open NUnit.Framework
open ParallelTypeCheckingTests.Utils

[<SetUpFixture>]
type SetUp() =
    let mutable tracerProvider = None

    [<OneTimeSetUp>]
    member this.SetUp() =
        tracerProvider <- setupOtel() |> Some
        
    [<OneTimeTearDown>]
    member this.TearDown() =
        tracerProvider |> Option.iter (fun x -> x.Dispose())
        tracerProvider <- None

