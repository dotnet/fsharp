// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TyperelatedExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_rigidtypeannotation02.fs"|])>]
    let ``E_rigidtypeannotation02_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 7, Col 1, Line 7, Col 10, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 16, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 9, Col 1, Line 9, Col 41, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_rigidtypeannotation02b.fs"|])>]
    let ``E_rigidtypeannotation02b_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 7, Col 1, Line 7, Col 15, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 50, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float<s>>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"rigidtypeannotation01.fs"|])>]
    let ``rigidtypeannotation01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticcoercion01.fs"|])>]
    let ``staticcoercion01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 4, Col 1, Line 4, Col 11, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 5, Col 1, Line 5, Col 17, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 6, Col 1, Line 6, Col 38, "The result of this expression has type 'System.Collections.Generic.IEnumerator<int>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 7, Col 1, Line 7, Col 17, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"staticcoercion01b.fs"|])>]
    let ``staticcoercion01b_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 6, Col 1, Line 6, Col 16, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 7, Col 1, Line 7, Col 17, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 51, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float<s>>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 9, Col 1, Line 9, Col 22, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]


