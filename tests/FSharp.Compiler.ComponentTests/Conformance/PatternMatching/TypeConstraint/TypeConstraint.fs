// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeConstraint =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/TypeConstraint)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_typeconstraint01.fs"|])>]
    let ``TypeConstraint - E_typecontraint01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 21, Col 8, Line 21, Col 10, "Unexpected symbol ':>' in pattern. Expected ')' or other token.")
            (Error 583, Line 21, Col 5, Line 21, Col 6, "Unmatched '('")
        ]
        