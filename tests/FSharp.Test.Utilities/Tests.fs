module FSharp.Test.StressAttributeTests

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

[<Theory; Stress(Count = 50)>]
let ``Stress attribute works`` _ =
    passing.Run false