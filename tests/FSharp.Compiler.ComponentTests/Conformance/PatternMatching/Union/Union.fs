// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Union =
     // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_CapturesDiffVal01.fs"|])>]
    let ``Union - E_CapturesDiffVal01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 18, Line 10, Col 7, Line 10, Col 28, "The two sides of this 'or' pattern bind different sets of variables")
            (Error 18, Line 10, Col 7, Line 10, Col 40, "The two sides of this 'or' pattern bind different sets of variables")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_NotAllCaptureSameVal01.fs"|])>]
    let ``Union - E_NotAllCaptureSameVal01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 18, Line 10, Col 7, Line 10, Col 31, "The two sides of this 'or' pattern bind different sets of variables")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionCapturesDiffType01.fs"|])>]
    let ``Union - E_UnionCapturesDiffType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 9, Col 10, Line 9, Col 11, "This expression was expected to have type
    'int'    
but here has type
    'float'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern01.fs"|])>]
    let ``Union - unionPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern02.fs"|])>]
    let ``Union - unionPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern03.fs"|])>]
    let ``Union - unionPattern03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 12, Col 7, Line 12, Col 12, "This rule will never be matched")
            (Warning 26, Line 21, Col 7, Line 21, Col 12, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern04.fs"|])>]
    let ``Union - unionPattern04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern01.fs"|])>]
    let ``Union - E_UnionPattern1_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 9, Line 6, Col 10, "Incomplete pattern matches on this expression. For example, the value 'C' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 9, Col 5, Line 9, Col 22, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern02.fs"|])>]
    let ``Union - E_UnionPattern2_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 9, Line 6, Col 10, "Incomplete pattern matches on this expression. For example, the value 'C' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 9, Col 5, Line 9, Col 27, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern03.fs"|])>]
    let ``Union - E_UnionPattern3_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 12, Line 9, Col 17, "All branches of a pattern match expression must return values implicitly convertible to the type of the first branch, which here is 'string'. This branch returns a value of type 'bool'.")
            (Warning 25, Line 7, Col 11, Line 7, Col 12, "Incomplete pattern matches on this expression. For example, the value 'B' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 9, Col 7, Line 9, Col 8, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern04.fs"|])>]
    let ``Union - E_UnionPattern4_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 4, Col 7, Line 4, Col 13, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern05.fs"|])>]
    let ``Union - E_UnionPattern5_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 8, Col 7, Line 8, Col 23, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern06.fs"|])>]
    let ``Union - E_UnionPattern6_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 11, Line 6, Col 12, "Incomplete pattern matches on this expression. For example, the value 'B' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 8, Col 7, Line 8, Col 15, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern07.fs"|])>]
    let ``Union - E_UnionPattern7_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 11, Line 6, Col 12, "Incomplete pattern matches on this expression. For example, the value 'B' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 8, Col 7, Line 8, Col 26, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern08.fs"|])>]
    let ``Union - E_UnionPattern8_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 11, Line 6, Col 12, "Incomplete pattern matches on this expression. For example, the value 'B' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 8, Col 7, Line 8, Col 8, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern09.fs"|])>]
    let ``Union - E_UnionPattern9_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 8, Col 7, Line 8, Col 39, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern10.fs"|])>]
    let ``Union - E_UnionPattern10_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 8, Col 7, Line 8, Col 52, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionPattern11.fs"|])>]
    let ``Union - E_UnionPattern11_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 8, Col 7, Line 8, Col 55, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"UpperUnionCasePattern.fs"|])>]
    let ``Union - UpperUnionCasePattern_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"; "--nowarn:026"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 49, Line 15, Col 7, Line 15, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 16, Col 7, Line 16, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 20, Col 7, Line 20, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 24, Col 3, Line 24, Col 6, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 35, Col 14, Line 35, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 41, Col 12, Line 41, Col 15, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 45, Col 20, Line 45, Col 23, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 50, Col 14, Line 50, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 50, Col 21, Line 50, Col 24, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 51, Col 14, Line 51, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 52, Col 14, Line 52, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"UpperUnionCasePattern.fs"|])>]
    let ``Union - UpperUnionCasePattern_fs preview - --test:ErrorRanges`` compilation =
        compilation
        |> withLangVersionPreview
        |> asFs
        |> withOptions ["--test:ErrorRanges"; "--nowarn:026"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 49, Line 3, Col 7, Line 3, Col 9, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 4, Col 7, Line 4, Col 9, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 5, Col 7, Line 5, Col 8, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 9, Col 7, Line 9, Col 9, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 10, Col 7, Line 10, Col 9, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 11, Col 7, Line 11, Col 8, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 15, Col 7, Line 15, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 16, Col 7, Line 16, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 20, Col 7, Line 20, Col 10, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 24, Col 3, Line 24, Col 6, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 28, Col 3, Line 28, Col 5, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 35, Col 14, Line 35, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
        ]