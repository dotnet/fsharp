// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Wildcard =
        // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Wildcard)
    [<Theory; FileInlineData("wildCardPatterns01.fs")>]
    let ``Wildcard - wildCardPatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed