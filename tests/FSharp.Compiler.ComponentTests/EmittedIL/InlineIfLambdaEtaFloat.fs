// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module InlineIfLambdaEtaFloat =

    // No-closure facts assert the closure *type* is absent; the negative controls assert it is invoked.
    let private intIntFunc = "FSharpFunc`2<int32,int32>"
    let private intIntInvoke = intIntFunc + "::Invoke"

    let private compileOpt source =
        FSharp source |> withOptimize |> asLibrary |> compile |> shouldSucceed

    [<Fact>]
    let ``Module-level function capture allocates no closure`` () =
        compileOpt """
module Test
type Box(v: int) = member _.Value = v
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let shapeB (b: Box) (o: int option) = o |> Option.map (f4 1 b.Value 3)
"""
        |> verifyILNotPresent [ intIntFunc ]

    [<Fact>]
    let ``Generic module-level function capture allocates no closure`` () =
        compileOpt """
module Test
type Box(v: int) = member _.Value = v
let gpick (a:'T) (b:'T) (c:'T) (x:'T) : 'T = a
let shapeG (b: Box) (o: int option) = o |> Option.map (gpick 1 b.Value 3)
"""
        |> verifyILNotPresent [ intIntFunc ]

    // A first-class function argument has no known arity, so there is nothing to eta-expand.
    [<Fact>]
    let ``First-class function argument keeps its closure`` () =
        compileOpt """
module Test
let shapeD (g: int -> int) (o: int option) = o |> Option.map g
"""
        |> verifyILPresent [ intIntInvoke ]

    // A static member lacks the module-level known-arity shape the transform keys on.
    [<Fact>]
    let ``Static-member callee keeps its closure`` () =
        compileOpt """
module Test
type Box(v: int) = member _.Value = v
type H = static member SF (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let shapeM (b: Box) (o: int option) = o |> Option.map (H.SF 1 b.Value 3)
"""
        |> verifyILPresent [ intIntInvoke ]
