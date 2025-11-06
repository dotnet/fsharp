// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ConsList =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    [<Theory; FileInlineData("consPattern01.fs")>]
    let ``ConsList - consPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    [<Theory; FileInlineData("E_consOnNonList.fs")>]
    let ``ConsList - E_consOnNonList_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 1, Line 4, Col 21, Line 4, Col 28, "This expression was expected to have type
    'int'    
but here has type
    ''a list'    "
            Error 1, Line 5, Col 21, Line 5, Col 33, "This expression was expected to have type
    'int'    
but here has type
    ''a list'    "
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    [<Theory; FileInlineData("E_consPattern01.fs")>]
    let ``ConsList - E_consPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 15, Col 22, Line 15, Col 24, "This expression was expected to have type\n    'int'    \nbut here has type\n    ''a list'    ")
            (Error 1, Line 16, Col 14, Line 16, Col 16, "This expression was expected to have type\n    'int'    \nbut here has type\n    ''a list'    ")
            (Warning 25, Line 13, Col 11, Line 13, Col 12, "Incomplete pattern matches on this expression. For example, the value '[_]' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 15, Col 7, Line 15, Col 18, "This rule will never be matched")
            (Warning 26, Line 16, Col 7, Line 16, Col 24, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    [<Theory; FileInlineData("OutsideMatch01.fs")>]
    let ``ConsList - OutsideMatch01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 6, Col 5, Line 6, Col 16, "Incomplete pattern matches on this expression. For example, the value '[_]' may indicate a case not covered by the pattern(s).")