// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Scripting

open Xunit

open System
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Diagnostics
open FSharp.Test.CompilerAssertHelpers

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
    [<InlineData(true, true)>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    let ``Evaluation of multiple sessions should succeed`` (useMultiEmit1, useMultiEmit2) =

    // This test is to check that we can have multiple sessions and that they can access each other

        let AssertCanWeAccessTheOtherSession (session: FSharpScript) (script: string) (moduleName: string) (errorChoice: string) =

            let result: Result<FsiValue option, exn> * FSharpDiagnostic[] =  session.Eval(script)
            let expected =
                [ (Error 39, Line 1, Col 1, Line 1, Col 6, $"The value, namespace, type or module '{moduleName}' is not defined. Maybe you want one of the following:{errorChoice}") ]

            match result with
            | Result.Error _, diagnostics -> Assert.WithDiagnostics(0, diagnostics, expected)
            | _ -> ()

        // Initialize
        let args useMultiEmit : string array = [| if useMultiEmit then "--multiemit+" else "--multiemit-"|]
        use sessionOne = new FSharpScript(additionalArgs = args useMultiEmit1)
        use sessionTwo = new FSharpScript(additionalArgs = args useMultiEmit2)
        use sessionThree = new FSharpScript(additionalArgs = args useMultiEmit2)

        // First session
        sessionOne.Eval("""module Test1 = let test1 obj = sprintf "Execute - Test1.test1 - %A" obj""") |> ignore
        let result1 = sessionOne.Eval("Test1.test1 18") |> getValue
        let value1 = result1.Value
        Assert.Equal(typeof<string>, value1.ReflectionType)
        Assert.Equal("Execute - Test1.test1 - 18", value1.ReflectionValue :?> string)

        // Second session
        sessionTwo.Eval("""module Test2 = let test2 obj = sprintf "Execute - Test2.test2 - %A" obj""") |> ignore
        let result2 = sessionTwo.Eval("""Test2.test2 27""") |> getValue
        let value2 = result2.Value
        Assert.Equal(typeof<string>, value2.ReflectionType)
        Assert.Equal("Execute - Test2.test2 - 27", value2.ReflectionValue :?> string)

        // Can I access the other session
        AssertCanWeAccessTheOtherSession sessionOne "Test2.test2 19" "Test2" "\nTest1\nText"            // Session 1 can't access values from session 2
        AssertCanWeAccessTheOtherSession sessionTwo "Test1.test1 13" "Test1" "\nTest2\nText"            // Session 2 can't access values from session 1

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
