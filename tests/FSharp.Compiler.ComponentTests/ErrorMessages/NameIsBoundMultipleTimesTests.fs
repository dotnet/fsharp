// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module NameIsBoundMultipleTimes =
    [<Fact>]
    let ``Name is bound multiple times is reported``() =
        Fsx """
let f1 a a = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 10, Line 2, Col 11, "'a' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``Name is bound multiple times is reported in 'as' pattern 1``() =
        Fsx """
let f2 (a, b as c) c = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 20, Line 2, Col 21, "'c' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``Name is bound multiple times is reported in 'as' pattern 2``() =
        Fsx """
let f4 (a, b, c as d) a c = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 23, Line 2, Col 24, "'a' is bound twice in this pattern")
            (Error 38, Line 2, Col 25, Line 2, Col 26, "'c' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``Name is bound multiple times is reported in 'as' pattern 3``() =
        Fsx """
let f5 (a, b, c as d) a d = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 23, Line 2, Col 24, "'a' is bound twice in this pattern");
            (Error 38, Line 2, Col 25, Line 2, Col 26, "'d' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``Name is bound multiple times is reported 2`` () =
        Fsx """
let (++) e1 e1 = if e1 then e1 else false
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 13, Line 2, Col 15, "'e1' is bound twice in this pattern")
        ]