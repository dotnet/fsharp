// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler

/// https://github.com/dotnet/fsharp/issues/19248
/// https://github.com/dotnet/fsharp/issues/19255
module CEDebugPoints =

    let private verifyCEMethodDebugPoints source methodName expectedSequencePoints =
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints(methodName, expectedSequencePoints) ]

    [<Fact>]
    let ``Return in async CE - debug point covers full expression`` () =
        verifyCEMethodDebugPoints """
module TestModule

let a =
    async {
        return 1
    }
        """ "Invoke" [ (Line 6, Col 9, Line 6, Col 17) ]

    [<Fact>]
    let ``Yield in seq CE - debug point on yield value`` () =
        verifyCEMethodDebugPoints """
module TestModule

let a =
    seq {
        yield 42
    }
        """ "GenerateNext" [ (Line 6, Col 15, Line 6, Col 17) ]

    [<Fact>]
    let ``Use in task CE - no extra out-of-order sequence point`` () =
        verifyCEMethodDebugPoints """
module TestModule

open System
open System.Threading.Tasks

type Disposable() =
    interface IDisposable with
        member _.Dispose() = ()

let t =
    task {
        let i = 1
        use d = new Disposable()
        return i
    }
        """ "MoveNext" [ (Line 14, Col 9, Line 14, Col 33); (Line 13, Col 9, Line 13, Col 18); (Line 14, Col 13, Line 14, Col 14); (Line 15, Col 9, Line 15, Col 17) ]
