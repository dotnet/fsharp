// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Named =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternOutsideMatch01.fs"|])>]
    let ``Named - ActivePatternOutsideMatch01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternOutsideMatch02.fs"|])>]
    let ``Named - ActivePatternOutsideMatch02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns01.fs"|])>]
    let ``Named - activePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns02.fs"|])>]
    let ``Named - activePatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns03.fs"|])>]
    let ``Named - activePatterns03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns05.fs"|])>]
    let ``Named - activePatterns05_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns06.fs"|])>]
    let ``Named - activePatterns06_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns07.fs"|])>]
    let ``Named - activePatterns07_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"activePatterns08.fs"|])>]
    let ``Named - activePatterns08_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActivePatternUnconstrained01.fs"|])>]
    let ``Named - ActivePatternUnconstrained01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> ignoreWarnings
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsHighOrderFunc01.fs"|])>]
    let ``Named - AsHighOrderFunc01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"discUnion01.fs"|])>]
    let ``Named - discUnion01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"discUnion02.fs"|])>]
    let ``Named - _DiscUnion01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatternHasNoFields.fs"|])>]
    let ``Named - E_ActivePatternHasNoFields_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 3174, Line 10, Col 24, Line 10, Col 25, "Active patterns do not have fields. This syntax is invalid.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatternNotAFuncion.fs"|])>]
    let ``Named - E_ActivePatternNotAFuncion_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1209, Line 5, Col 6, Line 5, Col 11, "Active pattern '|A|B|' is not a function")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatterns01.fs"|])>]
    let ``Named - E_ActivePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatterns02.fs"|])>]
    let ``Named - E_ActivePatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3210, Line 6, Col 15, Line 6, Col 24, "A is an active pattern and cannot be treated as a discriminated union case with named fields.")
            (Warning 20, Line 6, Col 1, Line 6, Col 38, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ActivePatternUnconstrained01.fs"|])>]
    let ``Named - E_ActivePatternUnconstrained01_fs - --test:ErrorRanges`` compilation =
        compilation
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_LetRec01.fs"|])>]
    let ``Named - E_Error_LetRec01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 5, Col 9, Line 5, Col 30, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_LetRec02.fs"|])>]
    let ``Named - E_Error_LetRec02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 827, Line 4, Col 9, Line 4, Col 34, "This is not a valid name for an active pattern")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_LetRec03.fs"|])>]
    let ``Named - E_Error_LetRec03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 827, Line 4, Col 10, Line 4, Col 43, "This is not a valid name for an active pattern")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_LetRec04.fs"|])>]
    let ``Named - E_Error_LetRec04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 9, Line 4, Col 29, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_NonParam01.fs"|])>]
    let ``Named - E_Error_NonParam01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 5, Line 4, Col 18, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_NonParam02.fs"|])>]
    let ``Named - E_Error_NonParam02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 827, Line 4, Col 5, Line 4, Col 22, "This is not a valid name for an active pattern")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_NonParam03.fs"|])>]
    let ``Named - E_Error_NonParam03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 827, Line 4, Col 5, Line 4, Col 30, "This is not a valid name for an active pattern")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_NonParam04.fs"|])>]
    let ``Named - E_Error_NonParam04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 5, Line 4, Col 17, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_Param01.fs"|])>]
    let ``Named - E_Error_Param01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 5, Line 4, Col 26, "This expression was expected to have type
    'Choice<'a,'b>'    
but here has type
    'string'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_Param02.fs"|])>]
    let ``Named - E_Error_Param02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 827, Line 4, Col 5, Line 4, Col 31, "This is not a valid name for an active pattern")
    
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_Param03.fs"|])>]
    let ``Named - E_Error_Param03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 827, Line 4, Col 5, Line 4, Col 38, "This is not a valid name for an active pattern")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Error_Param04.fs"|])>]
    let ``Named - E_Error_Param04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 5, Line 4, Col 25, "This expression was expected to have type
    ''a option'    
but here has type
    'string'    ")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_LargeActivePat01.fs"|])>]
    let ``Named - E_LargeActivePat01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 265, Line 6, Col 53, Line 6, Col 56, "Active patterns cannot return more than 7 possibilities")
        
    
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MulticasePartialNotAllowed01.fs"|])>]
    let ``Named - E_MulticasePartialNotAllowed01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 827, Line 8, Col 5, Line 8, Col 64, "This is not a valid name for an active pattern")
            (Error 39, Line 20, Col 7, Line 20, Col 15, "The pattern discriminator 'Sentence' is not defined.")
            (Error 72, Line 20, Col 25, Line 20, Col 37, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Error 39, Line 21, Col 7, Line 21, Col 11, "The pattern discriminator 'Word' is not defined.")
            (Error 72, Line 21, Col 20, Line 21, Col 31, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
            (Warning 49, Line 22, Col 7, Line 22, Col 17, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Error 39, Line 23, Col 7, Line 23, Col 18, "The pattern discriminator 'Punctuation' is not defined.")
            (Warning 26, Line 23, Col 7, Line 23, Col 20, "This rule will never be matched")
            (Warning 26, Line 24, Col 7, Line 24, Col 8, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ParameterRestrictions01.fs"|])>]
    let ``Named - E_ParameterRestrictions01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 722, Line 15, Col 23, Line 15, Col 34, "Only active patterns returning exactly one result may accept arguments")
    
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PatternMatchRegressions02.fs"|])>]
    let ``Named - E_PatternMatchRegressions02_fs - --test:ErrorRanges`` compilation =
        compilation
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MultiActivePatterns01.fs"|])>]
    let ``Named - MultiActivePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedLiteral01.fs"|])>]
    let ``Named - NamedLiteral01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedLiteral02.fs"|])>]
    let ``Named - NamedLiteral02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ParamertizedPartialActivePattern01.fs"|])>]
    let ``Named - ParamertizedPartialActivePattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PatternMatchRegressions01.fs"|])>]
    let ``Named - PatternMatchRegressions01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 22, Col 11, Line 22, Col 12, "Incomplete pattern matches on this expression.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PatternMatchRegressions02.fs"|])>]
    let ``Named - PatternMatchRegressions02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 20, Col 11, Line 20, Col 12, "Incomplete pattern matches on this expression.")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveActivePats.fs"|])>]
    let ``Named - RecursiveActivePats_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed