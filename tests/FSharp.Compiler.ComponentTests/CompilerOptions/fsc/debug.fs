// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module debug =

    [<Fact>]
    let ``fsc debug``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--debug"]
        |> compile
        |> shouldSucceed
        |> verifyHasPdb

    [<Fact>]
    let ``fsc debug plus``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--debug+"]
        |> compile
        |> shouldSucceed
        |> verifyHasPdb

    [<Fact>]
    let ``fsc debug minus``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--debug-"]
        |> compile
        |> shouldSucceed
        |> verifyNoPdb
