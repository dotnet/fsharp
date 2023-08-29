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
            (Error 38, Line 2, Col 8, Line 2, Col 9, "'a' is bound twice in this pattern")
            (Error 38, Line 3, Col 17, Line 3, Col 18, "'c' is bound twice in this pattern")
            (Error 38, Line 4, Col 17, Line 4, Col 18, "'c' is bound twice in this pattern")
            (Error 38, Line 5, Col 20, Line 5, Col 21, "'d' is bound twice in this pattern")
            (Error 38, Line 6, Col 20, Line 6, Col 21, "'d' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``Test me`` () =
        Fsx """
let f2 (a, b as c) c = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 17, Line 2, Col 18, "'c' is bound twice in this pattern")
        ]