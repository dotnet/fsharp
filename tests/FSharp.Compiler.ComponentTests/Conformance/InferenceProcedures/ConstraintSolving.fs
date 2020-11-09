// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ConstraintSolving =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    //<Expects id="FS0001" status="error" span="(14,15-14,24)">This expression was expected to have type.    'Foo'    .but here has type.    'Bar'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"E_NoImplicitDowncast01.fs"|])>]
    let ``ConstraintSolving - E_NoImplicitDowncast01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'Foo'    .but here has type.    'Bar'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    //<Expects id="FS0030" status="error">Value restriction</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"E_TypeFuncDeclaredExplicit01.fs"|])>]
    let ``ConstraintSolving - E_TypeFuncDeclaredExplicit01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0030
        |> withDiagnosticMessageMatches "Value restriction"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"ValueRestriction01.fs"|])>]
    let ``ConstraintSolving - ValueRestriction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    //<Expects id="FS0030" status="error">Value restriction. The value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"E_ValueRestriction01.fs"|])>]
    let ``ConstraintSolving - E_ValueRestriction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0030
        |> withDiagnosticMessageMatches "Value restriction. The value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"EnumConstraint01.fs"|])>]
    let ``ConstraintSolving - EnumConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    //<Expects id="FS0001" status="error">The type 'int16' does not match the type 'int64'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"E_EnumConstraint01.fs"|])>]
    let ``ConstraintSolving - E_EnumConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'int16' does not match the type 'int64'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"DelegateConstraint01.fs"|])>]
    let ``ConstraintSolving - DelegateConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ConstraintSolving)
    //<Expects id="FS0001" status="error">The type 'CallbackBravo' has a non-standard delegate type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ConstraintSolving", Includes=[|"E_DelegateConstraint01.fs"|])>]
    let ``ConstraintSolving - E_DelegateConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'CallbackBravo' has a non-standard delegate type"

