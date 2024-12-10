// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Decimal =

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"literal01.fs"|])>]
    let ``Decimal - literal01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"incompleteMatchesLiteral01.fs"|])>]
    let ``Decimal - incompleteMatchesLiteral01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 7, Col 11, Line 7, Col 13, "Incomplete pattern matches on this expression. For example, the value '3M' may indicate a case not covered by the pattern(s).")