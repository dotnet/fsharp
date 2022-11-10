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