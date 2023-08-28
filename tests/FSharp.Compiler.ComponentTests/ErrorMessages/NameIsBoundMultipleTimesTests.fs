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
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 10, Line 2, Col 11, "'a' is bound twice in this pattern");
            (Error 38, Line 3, Col 20, Line 3, Col 21, "'c' is bound twice in this pattern");
            (Error 38, Line 4, Col 20, Line 4, Col 21, "'a' is bound twice in this pattern")
        ]