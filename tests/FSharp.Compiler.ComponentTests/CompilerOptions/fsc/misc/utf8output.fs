// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System

[<Collection(nameof NotThreadSafeResourceCollection)>]
module utf8output =

    [<Fact>]
    let ``OutputEncoding is restored after executing compilation`` () =
        let currentEncoding = Console.OutputEncoding
        use restoreCurrentEncodingAfterTest = { new IDisposable with member _.Dispose() = Console.OutputEncoding <- currentEncoding }

        // UTF16
        let encoding = Text.Encoding.Unicode

        Console.OutputEncoding <- encoding

        Fs """printfn "Hello world" """
        |> asExe
        |> withOptionsString "--utf8output"
        |> compile
        |> shouldSucceed
        |> ignore

        Console.OutputEncoding.BodyName |> Assert.shouldBe encoding.BodyName

    [<Fact>]
    let ``OutputEncoding is restored after running script`` () =
        let currentEncoding = Console.OutputEncoding
        use restoreCurrentEncodingAfterTest = { new IDisposable with member _.Dispose() = Console.OutputEncoding <- currentEncoding }

        // UTF16
        let encoding = Text.Encoding.Unicode

        Console.OutputEncoding <- encoding

        Fsx """printfn "Hello world" """
        |> withOptionsString "--utf8output"
        |> runFsi
        |> shouldSucceed
        |> ignore

        Console.OutputEncoding.BodyName |> Assert.shouldBe encoding.BodyName
