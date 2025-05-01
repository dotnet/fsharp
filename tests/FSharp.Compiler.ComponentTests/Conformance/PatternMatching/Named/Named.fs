// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Named =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("ActivePatternOutsideMatch01.fs")>]
    let ``Named - ActivePatternOutsideMatch01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("ActivePatternOutsideMatch02.fs")>]
    let ``Named - ActivePatternOutsideMatch02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns01.fs")>]
    let ``Named - activePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns02.fs")>]
    let ``Named - activePatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns03.fs")>]
    let ``Named - activePatterns03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns05.fs")>]
    let ``Named - activePatterns05_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns06.fs")>]
    let ``Named - activePatterns06_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns07.fs")>]
    let ``Named - activePatterns07_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("activePatterns08.fs")>]
    let ``Named - activePatterns08_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<Theory; FileInlineData("activePatterns09.fs")>]
    let ``Named - activePatterns09_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> typecheck
        |> shouldSucceed

    [<Theory; FileInlineData("activePatterns10.fs")>]
    let ``Named - activePatterns10_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 5, Line 6, Col 26, "Incomplete pattern matches on this expression.")
            (Warning 25, Line 7, Col 5, Line 7, Col 28, "Incomplete pattern matches on this expression.")
            (Warning 25, Line 8, Col 5, Line 8, Col 28, "Incomplete pattern matches on this expression.")
            (Warning 25, Line 9, Col 5, Line 9, Col 30, "Incomplete pattern matches on this expression.")
            (Warning 25, Line 13, Col 5, Line 13, Col 22, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 14, Col 5, Line 14, Col 24, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 15, Col 5, Line 15, Col 24, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 16, Col 5, Line 16, Col 26, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("ActivePatternUnconstrained01.fs")>]
    let ``Named - ActivePatternUnconstrained01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> ignoreWarnings
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("AsHighOrderFunc01.fs")>]
    let ``Named - AsHighOrderFunc01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("discUnion01.fs")>]
    let ``Named - discUnion01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("discUnion02.fs")>]
    let ``Named - _DiscUnion01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ActivePatternHasNoFields.fs")>]
    let ``Named - E_ActivePatternHasNoFields_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 3174, Line 10, Col 24, Line 10, Col 25, "Active patterns do not have fields. This syntax is invalid.")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ActivePatternNotAFunction.fs")>]
    let ``Named - E_ActivePatternNotAFunction_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1209, Line 5, Col 6, Line 5, Col 11, "Active pattern '|A|B|' is not a function")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ActivePatterns01.fs")>]
    let ``Named - E_ActivePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 623, Line 12, Col 7, Line 12, Col 15, "Active pattern case identifiers must begin with an uppercase letter")
            (Error 623, Line 12, Col 16, Line 12, Col 24, "Active pattern case identifiers must begin with an uppercase letter")
            (Error 623, Line 13, Col 7, Line 13, Col 13, "Active pattern case identifiers must begin with an uppercase letter")
            (Error 623, Line 14, Col 10, Line 14, Col 17, "Active pattern case identifiers must begin with an uppercase letter")
            (Error 623, Line 15, Col 7, Line 15, Col 13, "Active pattern case identifiers must begin with an uppercase letter")
            (Error 624, Line 16, Col 7, Line 16, Col 14, "The '|' character is not permitted in active pattern case identifiers")
            (Error 624, Line 17, Col 9, Line 17, Col 17, "The '|' character is not permitted in active pattern case identifiers")
            (Error 623, Line 18, Col 7, Line 18, Col 9, "Active pattern case identifiers must begin with an uppercase letter")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ActivePatterns02.fs")>]
    let ``Named - E_ActivePatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3210, Line 6, Col 15, Line 6, Col 24, "A is an active pattern and cannot be treated as a discriminated union case with named fields.")
            (Warning 20, Line 6, Col 1, Line 6, Col 38, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    [<Theory; FileInlineData("E_ActivePatterns03.fs")>]
    let ``Named - E_ActivePatterns03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 8, Line 3, Col 17, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
            (Error 3872, Line 4, Col 8, Line 4, Col 21, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 4, Col 8, Line 4, Col 21, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
            (Error 3872, Line 5, Col 8, Line 5, Col 29, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 5, Col 8, Line 5, Col 29, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
            (Error 1, Line 6, Col 8, Line 6, Col 16, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
            (Error 1, Line 11, Col 8, Line 11, Col 17, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
            (Error 3872, Line 12, Col 8, Line 12, Col 21, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 12, Col 8, Line 12, Col 21, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
            (Error 3872, Line 13, Col 8, Line 13, Col 29, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 13, Col 8, Line 13, Col 29, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
            (Error 1, Line 14, Col 8, Line 14, Col 16, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
            (Error 1, Line 19, Col 12, Line 19, Col 21, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
            (Error 3872, Line 20, Col 12, Line 20, Col 25, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 20, Col 12, Line 20, Col 25, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
            (Error 3872, Line 21, Col 13, Line 21, Col 34, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.");
            (Error 1, Line 21, Col 13, Line 21, Col 34, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
            (Error 1, Line 22, Col 12, Line 22, Col 20, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
            (Error 39, Line 29, Col 8, Line 29, Col 18, "The pattern discriminator 'FooA++' is not defined.")
            (Warning 25, Line 29, Col 7, Line 29, Col 22, "Incomplete pattern matches on this expression.")
            (Error 39, Line 31, Col 50, Line 31, Col 54, "The value or constructor 'OneA' is not defined.")
            (Error 39, Line 31, Col 60, Line 31, Col 69, "The value or constructor 'TwoA+' is not defined.")
            (Error 39, Line 34, Col 8, Line 34, Col 18, "The pattern discriminator 'FooB++' is not defined.")
            (Warning 25, Line 34, Col 7, Line 34, Col 22, "Incomplete pattern matches on this expression.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ActivePatternUnconstrained01.fs")>]
    let ``Named - E_ActivePatternUnconstrained01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 12, Col 18, Line 12, Col 19, "Incomplete pattern matches on this expression.")
            (Warning 20, Line 14, Col 1, Line 14, Col 5, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Error 1210, Line 7, Col 6, Line 7, Col 16, "Active pattern '|A1|A2|A3|' has a result type containing type variables that are not determined by the input. The common cause is a when a result case is not mentioned, e.g. 'let (|A|B|) (x:int) = A x'. This can be fixed with a type constraint, e.g. 'let (|A|B|) (x:int) : Choice<int,unit> = A x'")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_LetRec01.fs")>]
    let ``Named - E_Error_LetRec01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 5, Col 10, Line 5, Col 19, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_LetRec02.fs")>]
    let ``Named - E_Error_LetRec02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 10, Line 4, Col 23, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 10, Line 4, Col 23, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_LetRec03.fs")>]
    let ``Named - E_Error_LetRec03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 11, Line 4, Col 32, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 11, Line 4, Col 32, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_LetRec04.fs")>]
    let ``Named - E_Error_LetRec04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 10, Line 4, Col 18, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_NonParam01.fs")>]
    let ``Named - E_Error_NonParam01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 6, Line 4, Col 15, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_NonParam02.fs")>]
    let ``Named - E_Error_NonParam02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 6, Line 4, Col 19, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 6, Line 4, Col 19, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_NonParam03.fs")>]
    let ``Named - E_Error_NonParam03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 6, Line 4, Col 27, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 6, Line 4, Col 27, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_NonParam04.fs")>]
    let ``Named - E_Error_NonParam04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 6, Line 4, Col 14, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_Param01.fs")>]
    let ``Named - E_Error_Param01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 6, Line 4, Col 15, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_Param02.fs")>]
    let ``Named - E_Error_Param02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 6, Line 4, Col 19, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 6, Line 4, Col 19, "This expression was expected to have type
    'Choice<'a,'b> option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_Param03.fs")>]
    let ``Named - E_Error_Param03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 4, Col 6, Line 4, Col 27, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 1, Line 4, Col 6, Line 4, Col 27, "This expression was expected to have type
    'Choice<'a,'b,'c> option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_Error_Param04.fs")>]
    let ``Named - E_Error_Param04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 6, Line 4, Col 14, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_LargeActivePat01.fs")>]
    let ``Named - E_LargeActivePat01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 265, Line 6, Col 6, Line 6, Col 47, "Active patterns cannot return more than 7 possibilities")
            (Error 265, Line 8, Col 6, Line 8, Col 23, "Active patterns cannot return more than 7 possibilities")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_MulticasePartialNotAllowed01.fs")>]
    let ``Named - E_MulticasePartialNotAllowed01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3872, Line 8, Col 6, Line 8, Col 46, "Multi-case partial active patterns are not supported. Consider using a single-case partial active pattern or a full active pattern.")
            (Error 3868, Line 22, Col 7, Line 22, Col 17, "This active pattern expects exactly one pattern argument, e.g., 'WhiteSpace pat'.");
            (Error 1107, Line 20, Col 7, Line 20, Col 21, "Partial active patterns may only generate one result")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_ParameterRestrictions01.fs")>]
    let ``Named - E_ParameterRestrictions01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 722, Line 15, Col 23, Line 15, Col 34, "Only active patterns returning exactly one result may accept arguments")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("E_PatternMatchRegressions02.fs")>]
    let ``Named - E_PatternMatchRegressions02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 23, Col 11, Line 23, Col 12, "Incomplete pattern matches on this expression.")
            (Warning 25, Line 30, Col 11, Line 30, Col 12, "Incomplete pattern matches on this expression.")
            (Error 1210, Line 22, Col 6, Line 22, Col 38, "Active pattern '|ClientExternalTypeUse|WillFail|' has a result type containing type variables that are not determined by the input. The common cause is a when a result case is not mentioned, e.g. 'let (|A|B|) (x:int) = A x'. This can be fixed with a type constraint, e.g. 'let (|A|B|) (x:int) : Choice<int,unit> = A x'")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("MultiActivePatterns01.fs")>]
    let ``Named - MultiActivePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("NamedLiteral01.fs")>]
    let ``Named - NamedLiteral01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("NamedLiteral02.fs")>]
    let ``Named - NamedLiteral02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("ParameterizedPartialActivePattern01.fs")>]
    let ``Named - ParameterizedPartialActivePattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("PatternMatchRegressions01.fs")>]
    let ``Named - PatternMatchRegressions01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 22, Col 11, Line 22, Col 12, "Incomplete pattern matches on this expression.")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("PatternMatchRegressions02.fs")>]
    let ``Named - PatternMatchRegressions02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 20, Col 11, Line 20, Col 12, "Incomplete pattern matches on this expression.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; FileInlineData("RecursiveActivePats.fs")>]
    let ``Named - RecursiveActivePats_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed