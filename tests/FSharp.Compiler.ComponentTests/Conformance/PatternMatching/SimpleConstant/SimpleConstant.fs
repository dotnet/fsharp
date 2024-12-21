// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SimpleConstant =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("DiffAssembly.fs")>]
    let ``SimpleConstant - DiffAssembly_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("E_NoRangeConst01.fs")>]
    let ``SimpleConstant - E_NoRangeConst01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 10, Line 9, Col 9, Line 9, Col 11, "Unexpected symbol '..' in pattern matching. Expected '->' or other token.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("E_type_bigint.fs")>]
    let ``SimpleConstant - E_type_bigint_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 720, Line 9, Col 7, Line 9, Col 9, "Non-primitive numeric literal constants cannot be used in pattern matches because they can be mapped to multiple different types through the use of a NumericLiteral module. Consider using replacing with a variable, and use 'when <variable> = <constant>' at the end of the match clause.")
            (Error 720, Line 10, Col 7, Line 10, Col 10, "Non-primitive numeric literal constants cannot be used in pattern matches because they can be mapped to multiple different types through the use of a NumericLiteral module. Consider using replacing with a variable, and use 'when <variable> = <constant>' at the end of the match clause.")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("E_type_bignum40.fs")>]
    let ``SimpleConstant - E_type_bignum40_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 720, Line 10, Col 7, Line 10, Col 13, "Non-primitive numeric literal constants cannot be used in pattern matches because they can be mapped to multiple different types through the use of a NumericLiteral module. Consider using replacing with a variable, and use 'when <variable> = <constant>' at the end of the match clause.")
            (Error 720, Line 11, Col 7, Line 11, Col 9, "Non-primitive numeric literal constants cannot be used in pattern matches because they can be mapped to multiple different types through the use of a NumericLiteral module. Consider using replacing with a variable, and use 'when <variable> = <constant>' at the end of the match clause.")
            (Error 784, Line 14, Col 17, Line 14, Col 19, "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("FullyQualify01.fs")>]
    let ``SimpleConstant - FullyQualify01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("matchConst01.fs")>]
    let ``SimpleConstant - matchConst01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("matchConst02.fs")>]
    let ``SimpleConstant - matchConst02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("matchConst03.fs")>]
    let ``SimpleConstant - matchConst03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
    
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("matchConst04.fs")>]
    let ``SimpleConstant - matchConst04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("MatchLiteral01.fs")>]
    let ``SimpleConstant - MatchLiteral01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3190, Line 13, Col 7, Line 13, Col 17, "Lowercase literal 'intLiteral' is being shadowed by a new pattern with the same name. Only uppercase and module-prefixed literals can be used as named patterns.")
            (Warning 26, Line 14, Col 7, Line 14, Col 8, "This rule will never be matched")
            (Warning 3190, Line 20, Col 7, Line 20, Col 17, "Lowercase literal 'strLiteral' is being shadowed by a new pattern with the same name. Only uppercase and module-prefixed literals can be used as named patterns.")
            (Warning 26, Line 21, Col 7, Line 21, Col 8, "This rule will never be matched")
            (Warning 3190, Line 27, Col 7, Line 27, Col 18, "Lowercase literal 'boolLiteral' is being shadowed by a new pattern with the same name. Only uppercase and module-prefixed literals can be used as named patterns.")
            (Warning 26, Line 28, Col 7, Line 28, Col 11, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("MatchNaN.fs")>]
    let ``SimpleConstant - MatchNaN_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 8, Col 11, Line 8, Col 16, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 14, Col 7, Line 14, Col 24, "This rule will never be matched")
            (Warning 26, Line 20, Col 7, Line 20, Col 24, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_bigint.fs")>]
    let ``SimpleConstant - type_bigint_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_bool.fs")>]
    let ``SimpleConstant - type_bool_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 10, Col 7, Line 10, Col 8, "This rule will never be matched")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_byte.fs")>]
    let ``SimpleConstant - type_byte_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_byteArr.fs")>]
    let ``SimpleConstant - type_byteArr_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_char.fs")>]
    let ``SimpleConstant - type_char_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_double.fs")>]
    let ``SimpleConstant - type_double_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_float32.fs")>]
    let ``SimpleConstant - type_float32_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_int.fs")>]
    let ``SimpleConstant - type_int_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_int16.fs")>]
    let ``SimpleConstant - type_int16_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; FileInlineData("type_int64.fs")>]
    let ``SimpleConstant - type_int64_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
    
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_nativenint.fs"|])>]
    let ``SimpleConstant - type_nativenint_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_sbyte.fs"|])>]
    let ``SimpleConstant - type_sbyte_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_string.fs"|])>]
    let ``SimpleConstant - type_string_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 10, Col 7, Line 10, Col 17, "This rule will never be matched")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_uint16.fs"|])>]
    let ``SimpleConstant - type_uint16_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_uint32.fs"|])>]
    let ``SimpleConstant - type_uint32_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_uint64.fs"|])>]
    let ``SimpleConstant - type_uint64_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_unativenint.fs"|])>]
    let ``SimpleConstant - type_unativenint_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"type_unit.fs"|])>]
    let ``SimpleConstant - type_unit_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 9, Col 7, Line 9, Col 8, "This rule will never be matched")