// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Array =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"arrayMatch01.fs"|])>]
    let ``Array - arrayMatch01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"arrayMatch02.fs"|])>]
    let ``Array - arrayMatch02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"arrayMatch03.fs"|])>]
    let ``Array - arrayMatch03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TrailingSemi01.fs"|])>]
    let ``Array - TrailingSemi01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
