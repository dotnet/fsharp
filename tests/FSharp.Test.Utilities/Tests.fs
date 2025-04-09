module FSharp.Test.UtilitiesTests

open System
open System.Threading
open Xunit
open FSharp.Test

type RunOrFail(name) =
    let mutable count = 0
    member _.Run(shouldFail: bool) =
        let count = Interlocked.Increment &count
        if shouldFail && count = 42 then
            failwith $"{name}, failed as expected on {count}"
        else
            printfn $"{name}, iteration {count} passed"
            count

let passing = RunOrFail "Passing"
let failing = RunOrFail "Failing"

[<Theory(Skip = "Explicit, unskip to run"); Stress(true, Count = 50)>]
let ``Stress attribute should catch intermittent failure`` shouldFail _ =
    failing.Run shouldFail

[<Theory; Stress(Count = 5)>]
let ``Stress attribute works`` _ =
    passing.Run false

[<Fact>]
let ``TestConsole captures output`` () =
    let rnd = Random()
    let task n =
        async {
            use console = new TestConsole.ExecutionCapture()
            do! Async.Sleep (rnd.Next 50)
            printf $"Hello, world! {n}"
            do! Async.Sleep (rnd.Next 50)
            eprintf $"Some error {n}"
            return console.OutText, console.ErrorText
        }

    let expected = [ for n in 0 .. 9 -> $"Hello, world! {n}", $"Some error {n}" ]

    let results = Seq.init 10 task |> Async.Parallel |> Async.RunSynchronously
    Assert.Equal(expected, results)
