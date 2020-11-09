// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ApplicationExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ObjectConstruction =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/ObjectConstruction)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ApplicationExpressions/ObjectConstruction", Includes=[|"ObjectConstruction01.fs"|])>]
    let ``ObjectConstruction - ObjectConstruction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/ObjectConstruction)
    //<Expects id="FS0039" status="error" span="(5,37)">The type 'BitArray' does not define the field, constructor or member 'BitArrayEnumeratorSimple'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ApplicationExpressions/ObjectConstruction", Includes=[|"E_ObjectConstruction01.fs"|])>]
    let ``ObjectConstruction - E_ObjectConstruction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'BitArray' does not define the field, constructor or member 'BitArrayEnumeratorSimple'"

