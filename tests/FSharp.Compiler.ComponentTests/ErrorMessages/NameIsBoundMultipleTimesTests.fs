// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module NameIsBoundMultipleTimes =
    [<Fact>]
    let ``Name is bound multiple times is not reported in 'as' pattern``() =
        Fsx """
let f1 a a = ()
let f2 (a, b as c) c = ()
let f3 (a, b as c) a = ()
let f4 (a, b, c as d) a c = ()
let f5 (a, b, c as d) a d = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 10, Line 2, Col 11, "'a' is bound twice in this pattern")
            (Error 38, Line 3, Col 20, Line 3, Col 21, "'c' is bound twice in this pattern")
            (Error 38, Line 6, Col 25, Line 6, Col 26, "'d' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``CI Failure`` () =
        Fsx """
let (++) e1 e2 = if e1 then e2 else false
"""
        |> typecheck
        |> shouldSucceed
    
    [<Fact>]
    let ``CI Failure 2`` () =
        Fsx """
type CustomOperationAttribute(name:string) =
    let mutable allowInto = false
    member _.AllowIntoPattern with get() = allowInto and set v = allowInto <- v
"""
        |> typecheck
        |> shouldSucceed