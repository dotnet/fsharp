namespace FSharp.Compiler.ComponentTests.Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module WhileBangTests =

    [<Fact>]
    let ``while! works as expected`` () =
        FSharp """
        let mutable count = 0

        let asyncCondition = async {
            return count < 10
        }

        async {
            count <- 1

            while! asyncCondition do
                count <- count + 2

            count <- count + 1
            return count
        }
        |> Async.RunSynchronously
        |> printfn "%d"
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "12"

    [<Fact>]
    let ``Nested while! works as expected`` () =
        FSharp """
        let mutable total = 0
        let mutable outerCount = 0

        let outerCondition = async {
            outerCount <- outerCount + 1
            return outerCount <= 10
        }

        async {
            while! outerCondition do
                let mutable innerCount = 0

                let innerCondition = async {
                    innerCount <- innerCount + 1
                    return innerCount <= 3
                }

                while! innerCondition do
                    total <- total + 1

            return total
        }
        |> Async.RunSynchronously
        |> printfn "%d"
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "30"

    [<Fact>]
    let ``Nested while! works as expected2`` () =
        FSharp """
        let mutable total = 0
        let mutable outerCount = 0

        let outerCondition = async {
            outerCount <- outerCount + 1
            return outerCount <= 10
        }

        let mutable innerOnce = true

        let innerCondition = async {
            let c = innerOnce
            innerOnce <- false
            return c
        }

        async {
            while! outerCondition do
                while! innerCondition do
                    total <- total + 1

            return total
        }
        |> Async.RunSynchronously
        |> printfn "%d"
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"