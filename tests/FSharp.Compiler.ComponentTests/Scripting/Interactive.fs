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