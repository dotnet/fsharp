// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Scripting

open Xunit
open FSharp.Test.Compiler

module ``Interactive tests`` =
    [<Fact>]
    let ``Eval object value``() =
        Fsx "1+1"
        |> eval
        |> shouldSucceed
        |> withEvalTypeEquals typeof<int>
        |> withEvalValueEquals 2

    [<Fact>]
    let ``Using the EntryPoint attribute in FSI should produce a compiler warning indicating that it won't be invoked automatically.`` () =
        Fsx """
[<EntryPoint>]
let main argv =
    printfn "Hello, world!"   
    0
        """
        |> eval
        |> shouldFail
        |> withDiagnostics [
            (Warning 2304, Line 3, Col 5, Line 3, Col 9, "Function with '[<EntryPoint>]' are not invoked in FSI. 'main' was not invoked. Execute '['main' <args>]' in order to invoke 'main' with the appropriate string array of command line arguments.")
        ]

    [<Fact>]
    let ``Using the EntryPoint attribute in FSI should won't produce a compiler warning indicating that it won't be invoked automatically.`` () =
        Fsx """
let main argv =
    printfn "Hello, world!"   
    0
        """
        |> eval
        |> shouldSucceed


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