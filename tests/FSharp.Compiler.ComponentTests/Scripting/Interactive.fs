// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Scripting

open Xunit

open System
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers

module ``Interactive tests`` =

    [<Fact>]
    let ``Eval object value``() =
        Fsx "1+1"
        |> eval
        |> shouldSucceed
        |> withEvalTypeEquals typeof<int>
        |> withEvalValueEquals 2

    [<Fact>]
    let ``Pretty print void pointer``() =
        Fsx "System.IntPtr.Zero.ToPointer()"
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "val it: voidptr = 0n"

    [<Fact>]
    let ``EntryPoint attribute in FSI should produce a compiler warning`` () =
        Fsx "[<EntryPoint>] let myFunc _ = 0"
        |> eval
        |> shouldFail
        |> withDiagnostics [
            (Warning 2304, Line 1, Col 3, Line 1, Col 13, "Functions with [<EntryPoint>] are not invoked in FSI. 'myFunc' was not invoked. Execute 'myFunc <args>' in order to invoke 'myFunc' with the appropriate string array of command line arguments.")
        ]

    [<Theory>]
    [<InlineData(true)>]
    [<InlineData(false)>]
    let ``Evaluation of multiple sessions should succeed`` (useMultiEmit) =

        let args : string array = [| if useMultiEmit then "--multiemit+" else "--multiemit-"|]
        use sessionOne = new FSharpScript(additionalArgs=args)
        use sessionTwo = new FSharpScript(additionalArgs=args)

        sessionOne.Eval("""
module Test1 =

    let test1 obj = sprintf "Execute - Test1.test1 - %A" obj""") |> ignore

        let result1 = sessionOne.Eval("""Test1.test1 18""") |> getValue
        let value1 = result1.Value
        Assert.Equal(typeof<string>, value1.ReflectionType)
        Assert.Equal("Execute - Test1.test1 - 18", value1.ReflectionValue :?> string)

        sessionTwo.Eval("""
module Test2 =

    let test2 obj = sprintf "Execute - Test2.test2 - %A" obj""") |> ignore

        let result2 = sessionTwo.Eval("""Test2.test2 27""") |> getValue
        let value2 = result2.Value
        Assert.Equal(typeof<string>, value2.ReflectionType)
        Assert.Equal("Execute - Test2.test2 - 27", value2.ReflectionValue :?> string)

module ``External FSI tests`` =
    [<Fact>]
    let ``Eval object value``() =
        Fsx "1+1"
        |> runFsi
        |> shouldSucceed

    [<Fact>]
    let ``Invalid expression should fail``() =
        Fsx "1+a"
        |> runFsi
        |> shouldFail


    [<Fact>]
    let ``Internals visible over a large number of submissions``() =
        let submission =
            let lines = [|
                yield """let internal original_submission = "From the first submission";;""" + Environment.NewLine
                for _ in 1 .. 200 do yield """if original_submission <> "From the first submission" then failwith $"Failed to read an internal at line: {__LINE__}";;""" + Environment.NewLine
                |]
            lines |> Array.fold(fun acc line -> acc + line) ""
        Fsx submission
        |> runFsi
        |> shouldSucceed


module MultiEmit =

    [<Theory>]
    [<InlineData(true)>]
    [<InlineData(false)>]
    let ``FSharp record in script`` (useMultiEmit) =

        let args : string array = [| if useMultiEmit then "--multiemit+" else "--multiemit-"|]
        use session = new FSharpScript(additionalArgs=args)

        let scriptIt submission =

            let result, errors = session.Eval(submission)
            Assert.Empty(errors)
            match result with
            | Ok _ -> ()
            | _ -> Assert.True(false, $"Failed in line: {submission}")

        [|
            """type R = { x: int }"""
            """let a = { x = 7 } """
            """if a.x <> 7 then failwith $"1: Failed {a.x} <> 7" """
            """if a.x <> 7 then failwith $"2: Failed {a.x} <> 7" """
            """if a.x <> 7 then failwith $"3: Failed {a.x} <> 7" """
            """if a.x <> 7 then failwith $"4: Failed {a.x} <> 7" """
            """let b = { x = 9 }"""
            """if a.x <> 7 then failwith $"5: Failed {a.x} <> 7" """
            """if b.x <> 9 then failwith $"6: Failed {b.x} <> 9" """
            """let A = {| v = 7.2 |}"""
            """if A.v <> 7.2 then failwith $"7: Failed {A.v} <> 7.2" """
            """let B = {| v = 9.3 |}"""
            """if A.v <> 7.2 then failwith $"8: Failed {A.v} <> 7.2" """
            """if B.v <> 9.3 then failwith $"9: Failed {A.v} <> 9.3" """
        |] |> Seq.iter(fun item -> item |> scriptIt)
