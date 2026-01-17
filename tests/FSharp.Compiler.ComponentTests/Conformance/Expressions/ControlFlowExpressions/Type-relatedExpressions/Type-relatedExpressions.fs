// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TyperelatedExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; FileInlineData("E_rigidtypeannotation02.fs")>]
    let ``E_rigidtypeannotation02_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 7, Col 1, Line 7, Col 10, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 16, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 9, Col 1, Line 9, Col 41, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; FileInlineData("E_rigidtypeannotation02b.fs")>]
    let ``E_rigidtypeannotation02b_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 7, Col 1, Line 7, Col 15, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 50, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float<s>>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; FileInlineData("rigidtypeannotation01.fs")>]
    let ``rigidtypeannotation01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; FileInlineData("staticcoercion01.fs")>]
    let ``staticcoercion01_fs`` compilation =
        compilation
        |> getCompilation
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
    [<Theory; FileInlineData("staticcoercion01b.fs")>]
    let ``staticcoercion01b_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 6, Col 1, Line 6, Col 16, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 7, Col 1, Line 7, Col 17, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 8, Col 1, Line 8, Col 51, "The result of this expression has type 'System.Collections.Generic.IEnumerator<float<s>>' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Warning 20, Line 9, Col 1, Line 9, Col 22, "The result of this expression has type 'obj' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    [<Fact>]
    let ``Upcast 01`` () =
        FSharp """
module Module

type A() =
    class end

not true :> A |> ignore
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [(Error 193, Line 7, Col 1, Line 7, Col 9, "Type constraint mismatch. The type \n    'bool'    \nis not compatible with type\n    'A'    \n")]

    // Additional tests migrated from fsharpqa/Source/Conformance/Expressions/Type-relatedExpressions

    // SOURCE=E_RigidTypeAnnotation01.fsx SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_RigidTypeAnnotation01.fsx")>]
    let ``E_RigidTypeAnnotation01_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [1]
        |> ignore

    // SOURCE=E_RigidTypeAnnotation02.fsx SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_RigidTypeAnnotation02.fsx")>]
    let ``E_RigidTypeAnnotation02_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [1; 43]
        |> ignore

    // SOURCE=E_RigidTypeAnnotation02_5_0.fsx SCFLAGS="--langversion:5.0 --test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_RigidTypeAnnotation02_5_0.fsx")>]
    let ``E_RigidTypeAnnotation02_5_0_fsx`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion50
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [1; 43]
        |> ignore

    // SOURCE=E_RigidTypeAnnotation03.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_RigidTypeAnnotation03.fs")>]
    let ``E_RigidTypeAnnotation03_fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_StaticCoercion_class_not_impl_iface.fsx SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_StaticCoercion_class_not_impl_iface.fsx")>]
    let ``E_StaticCoercion_class_not_impl_iface_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 193
        |> ignore

    // SOURCE=E_StaticCoercion_class_not_subclass.fsx SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_StaticCoercion_class_not_subclass.fsx")>]
    let ``E_StaticCoercion_class_not_subclass_fsx`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 193
        |> ignore

    // SOURCE=RigidTypeAnnotation_null01.fsx
    [<Theory; FileInlineData("RigidTypeAnnotation_null01.fsx")>]
    let ``RigidTypeAnnotation_null01_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed

    // SOURCE=RigidTypeAnnotation01.fsx
    [<Theory; FileInlineData("RigidTypeAnnotation01.fsx")>]
    let ``RigidTypeAnnotation01_fsx`` compilation =
        compilation
        |> getCompilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=RigidTypeAnnotation02.fsx
    [<Theory; FileInlineData("RigidTypeAnnotation02.fsx")>]
    let ``RigidTypeAnnotation02_fsx`` compilation =
        compilation
        |> getCompilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=RigidTypeAnnotation03.fsx
    [<Theory; FileInlineData("RigidTypeAnnotation03.fsx")>]
    let ``RigidTypeAnnotation03_fsx`` compilation =
        compilation
        |> getCompilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=StaticCoercion_class01.fsx
    [<Theory; FileInlineData("StaticCoercion_class01.fsx")>]
    let ``StaticCoercion_class01_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed

    // SOURCE=StaticCoercion_int_to_obj.fsx
    [<Theory; FileInlineData("StaticCoercion_int_to_obj.fsx")>]
    let ``StaticCoercion_int_to_obj_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed

    // SOURCE=StaticCoercion_interface01.fsx
    [<Theory; FileInlineData("StaticCoercion_interface01.fsx")>]
    let ``StaticCoercion_interface01_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed

    // SOURCE=StaticCoercion_interface02.fsx
    [<Theory; FileInlineData("StaticCoercion_interface02.fsx")>]
    let ``StaticCoercion_interface02_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed

    // SOURCE=StaticCoercion_null_to_obj.fsx
    [<Theory; FileInlineData("StaticCoercion_null_to_obj.fsx")>]
    let ``StaticCoercion_null_to_obj_fsx`` compilation =
        compilation
        |> getCompilation
        |> typecheck
        |> shouldSucceed
