// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Simple =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("W_Incomplete01.fs")>]
    let ``Simple - W_Incomplete01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 92, Col 13, Line 92, Col 14, "Incomplete pattern matches on this expression. For example, the value 'Result (_)' may indicate a case not covered by the pattern(s).")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("W_Incomplete02.fs")>]
    let ``Simple - W_Incomplete02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 14, Col 15, Line 14, Col 16, "Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 21, Col 31, Line 21, Col 39, "Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s).")
            (Warning 26, Line 31, Col 11, Line 31, Col 12, "This rule will never be matched")
            (Warning 26, Line 32, Col 11, Line 32, Col 13, "This rule will never be matched")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("W_BindCapitalIdent.fs")>]
    let ``Simple - W_BindCapitalIdent_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 49, Line 9, Col 16, Line 9, Col 19, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 10, Col 16, Line 10, Col 19, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
        ]

    [<Theory; FileInlineData("W_BindCapitalIdent.fs")>]
    let ``Simple - W_BindCapitalIdent_fs preview - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 49, Line 9, Col 16, Line 9, Col 19, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
            (Warning 49, Line 10, Col 16, Line 10, Col 19, "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("CodeGenReg01.fs")>]
    let ``Simple - CodeGenReg01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("E_constPattern01.fs")>]
    let ``Simple - E_constPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 104, Line 9, Col 11, Line 9, Col 14, "Enums may take values outside known cases. For example, the value 'enum<DayOfWeek> (7)' may indicate a case not covered by the pattern(s).")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("E_namedLiberal01.fs")>]
    let ``Simple - E_namedLiberal01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 104, Line 10, Col 11, Line 10, Col 14, "Enums may take values outside known cases. For example, the value 'enum<DayOfWeek> (7)' may indicate a case not covered by the pattern(s).")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("E_SyntaxError01.fs")>]
    let ``Simple - E_SyntaxError01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 10, Line 8, Col 14, Line 8, Col 15, "Unexpected symbol'[' in pattern matching. Expected '->' or other token.")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("E_ValueCapture01.fs")>]
    let ``Simple - E_ValueCapture01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 14, Col 8, Line 14, Col 14, "The value or constructor 'ident1' is not defined. Maybe you want one of the following:
   int8
   int");
            (Error 39, Line 15, Col 8, Line 15, Col 14, "The value or constructor 'ident2' is not defined. Maybe you want one of the following:
   int8
   int")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("MatchFailureExn01.fs")>]
    let ``Simple - MatchFailureExn01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 20, Col 28, Line 20, Col 29, "Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 24, Col 15, Line 24, Col 23, "Incomplete pattern matches on this expression.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns01.fs")>]
    let ``Simple - simplePatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns02.fs")>]
    let ``Simple - simplePatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns03.fs")>]
    let ``Simple - simplePatterns03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns04.fs")>]
    let ``Simple - simplePatterns04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns05.fs")>]
    let ``Simple - simplePatterns05_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns06.fs")>]
    let ``Simple - simplePatterns06_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns07.fs")>]
    let ``Simple - simplePatterns07_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns08.fs")>]
    let ``Simple - simplePatterns08_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns09.fs")>]
    let ``Simple - simplePatterns09_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns10.fs")>]
    let ``Simple - simplePatterns10_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns11.fs")>]
    let ``Simple - simplePatterns11_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 10, Col 7, Line 10, Col 8, "This rule will never be matched")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns12.fs")>]
    let ``Simple - simplePatterns12_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns13.fs")>]
    let ``Simple - simplePatterns13_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns14.fs")>]
    let ``Simple - simplePatterns14_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns15.fs")>]
    let ``Simple - simplePatterns15_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns16.fs")>]
    let ``Simple - simplePatterns16_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; FileInlineData("simplePatterns17.fs")>]
    let ``Simple - simplePatterns17_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 16, Col 13, Line 16, Col 17, "Incomplete pattern matches on this expression. For example, the value '``some-non-null-value``' may indicate a case not covered by the pattern(s).")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/AsPatterns)
    [<Theory; FileInlineData("simplePatterns18.fs")>]
    let ``Simple - simplePatterns18_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/AsPatterns)
    [<Theory; FileInlineData("simplePatterns19.fs")>]
    let ``Simple - simplePatterns19_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/AsPatterns)
    [<Theory; FileInlineData("simplePatterns20.fs")>]
    let ``Simple - simplePatterns20_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/AsPatterns)
    [<Theory; FileInlineData("ValueCapture01.fs")>]
    let ``Simple - ValueCapture01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/AsPatterns)
    [<Theory; FileInlineData("ValueCapture02.fs")>]
    let ``Simple - ValueCapture02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges";]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Enum incompleteness check should not hide an issue with outer DU pattern matching with nowarn:104 `` () = 
        Fsx """
type E = A = 0

type Ex =
    | ExA of int * E
    | ExB of int
    
let flub ex =
    match ex with
    | ExA(_, E.A) -> ()
    
flub (ExB 3)
        """
        |> withNoWarn 104        
        |> typecheck
        |> shouldFail
        |> withDiagnostics [Warning 25, Line 9, Col 11, Line 9, Col 13, "Incomplete pattern matches on this expression. For example, the value 'ExB (_)' may indicate a case not covered by the pattern(s)."]

    [<Fact>]
    let ``Enum incompleteness check in nested scenarios should report all warnings`` () = 
        Fsx """
type E =
    | FieldA = 1
    | FieldB = 2

type U =
    | CaseA
    | CaseB of E

match CaseA with
| CaseB E.FieldA -> ()
| CaseB E.FieldB -> ()
        """     
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                Warning 104, Line 10, Col 7, Line 10, Col 12, "Enums may take values outside known cases. For example, the value 'CaseB (enum<E> (0))' may indicate a case not covered by the pattern(s)."
                Warning 25, Line 10, Col 7, Line 10, Col 12, "Incomplete pattern matches on this expression. For example, the value 'CaseA' may indicate a case not covered by the pattern(s)."]
   