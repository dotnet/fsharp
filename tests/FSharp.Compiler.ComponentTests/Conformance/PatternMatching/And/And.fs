// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module And =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"andPattern01.fs"|])>]
    let ``And - andPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"andPattern02.fs"|])>]
    let ``And - andPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"andPattern03.fs"|])>]
    let ``And - andPattern03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IdentBoundTwice.fs"|])>]
    let ``And - E_IdentBoundTwice_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 38, Line 9, Col 20, Line 9, Col 21, "'x' is bound twice in this pattern")