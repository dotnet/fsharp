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
        |> ignore
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"andPattern02.fs"|])>]
    let ``And - andPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"andPattern03.fs"|])>]
    let ``And - andPattern03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 41, Line 7, Col 28, Line 7, Col 45, "A unique overload for method 'TryParse' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a

Candidates:
 - Int32.TryParse(s: ReadOnlySpan<char>, result: byref<int>) : bool
 - Int32.TryParse(s: string, result: byref<int>) : bool")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IdentBoundTwice.fs"|])>]
    let ``And - E_IdentBoundTwice_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 38, Line 9, Col 20, Line 9, Col 21, "'x' is bound twice in this pattern")