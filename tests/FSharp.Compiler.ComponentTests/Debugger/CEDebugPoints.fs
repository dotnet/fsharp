// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler

module CEDebugPoints =

    [<Fact>]
    let ``Return in async CE - debug point covers full expression`` () =
        FSharp """
module TestModule

let a =
    async {
        return 1
    }
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints("Invoke", [ (Line 6, Col 9, Line 6, Col 17) ]) ]

    [<Fact>]
    let ``Yield in seq CE - debug point on yield value`` () =
        FSharp """
module TestModule

let a =
    seq {
        yield 42
    }
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints("GenerateNext", [ (Line 6, Col 15, Line 6, Col 17) ]) ]

    [<Fact>]
    let ``Use in async CE - no extra out-of-order sequence point`` () =
        FSharp """
module TestModule

open System

type Disposable() =
    interface IDisposable with
        member _.Dispose() = ()

let t =
    async {
        let i = 1
        use d = new Disposable()
        return i
    }
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints("Invoke", [ (Line 14, Col 9, Line 14, Col 17); (Line 13, Col 9, Line 13, Col 33); (Line 12, Col 9, Line 12, Col 18); (Line 13, Col 9, Line 13, Col 12) ]) ]
