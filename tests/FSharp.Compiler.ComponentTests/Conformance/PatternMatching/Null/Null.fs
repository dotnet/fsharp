// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Null =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Null)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_notNullCompatible01.fs"|])>]
    let ``Null - E_notNullCompatible01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 43, Line 14, Col 7, Line 14, Col 11, "The type 'Foo' does not have 'null' as a proper value")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Null)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"matchNull01.fs"|])>]
    let ``Null - matchNull01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed