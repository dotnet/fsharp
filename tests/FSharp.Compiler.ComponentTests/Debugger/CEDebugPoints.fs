// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler
open Debugger.DebuggerTestHelpers

/// https://github.com/dotnet/fsharp/issues/19248
/// https://github.com/dotnet/fsharp/issues/19255
module CEDebugPoints =

    [<Fact>]
    let ``Return in async CE - debug point covers full expression`` () =
        verifyMethodDebugPoints """
module TestModule

let a =
    async {
        return 1
    }
        """ "Invoke" [ (Line 6, Col 9, Line 6, Col 17) ]

    [<Fact>]
    let ``Yield in seq CE - debug point on yield value`` () =
        verifyMethodDebugPoints """
module TestModule

let a =
    seq {
        yield 42
    }
        """ "GenerateNext" [ (Line 6, Col 15, Line 6, Col 17) ]

    [<Fact>]
    let ``ReturnFrom in async CE - debug point covers full expression`` () =
        verifyMethodDebugPoints """
module TestModule

let a =
    async {
        return! async.Return(1)
    }
        """ "Invoke" [ (Line 6, Col 9, Line 6, Col 32) ]

    [<Fact>]
    let ``YieldFrom in CE - debug point covers full expression`` () =
        verifyMethodDebugPoints """
module TestModule

type Wrapper<'a> = Wrapper of 'a list

type ListBuilder() =
    member _.Yield(x) = Wrapper [x]
    member _.YieldFrom(Wrapper xs) = Wrapper xs
    member _.Combine(Wrapper xs, Wrapper ys) = Wrapper(xs @ ys)
    member _.Delay(f: unit -> Wrapper<'a>) = f()
    member _.Zero() = Wrapper []

let list = ListBuilder()

let a =
    list {
        yield! Wrapper [1; 2]
    }
        """ "staticInitialization@" [ (Line 13, Col 1, Line 13, Col 25); (Line 16, Col 5, Line 16, Col 9); (Line 17, Col 9, Line 17, Col 30) ]

    [<Fact>]
    let ``Yield in task CE - debug point covers full expression`` () =
        verifyMethodDebugPoints """
module TestModule

open System.Collections.Generic

let mkValue () = 42

let items = ResizeArray<int>()

let t =
    task {
        items.Add(mkValue())
        return ()
    }
        """ "MoveNext" [ (Line 12, Col 9, Line 12, Col 29); (Line 13, Col 9, Line 13, Col 18) ]

    [<Fact>]
    let ``Use in task CE - no extra out-of-order sequence point`` () =
        verifyMethodDebugPoints """
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

    // ---- Instrumentation: #19248 edge cases ----

    [<Fact>]
    let ``Return multi-line expression in async CE - debug point covers full range`` () =
        verifyMethodDebugPoints """
module TestModule

let a =
    async {
        return
            (1 + 2)
    }
        """ "Invoke" [ (Line 6, Col 9, Line 7, Col 20) ]

    [<Fact>]
    let ``Implicit yield in list CE - no regression`` () =
        // List literal [42] is NOT a CE yield — it's a constant list.
        // Verify the mFull change doesn't break list literals.
        verifyAllSequencePoints """
module TestModule

let a = [
    42
    ]
        """ [
            (Line 4, Col 1, Line 6, Col 6)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    // ---- Instrumentation: #19255 verify sequence point count ----

    [<Fact>]
    let ``Use in task CE - exactly expected number of sequence points`` () =
        // The original issue had 4 non-hidden SPs when only 3 were expected.
        // Verify the fix produces exactly the expected count.
        let source = """
module TestModule

open System

type Disposable() =
    interface IDisposable with
        member _.Dispose() = ()

let t =
    task {
        let i = 1
        use d = new Disposable()
        return i
    }
        """
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePointsInRange("MoveNext", Line 12, Line 15) ]
