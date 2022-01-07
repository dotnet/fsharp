// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CheckingSyntacticTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" id="FS3151" span="(12,5-12,35)">This member, function or value declaration may not be declared 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_CannotInlineVirtualMethods1.fs"|])>]
    let ``CheckingSyntacticTypes - E_CannotInlineVirtualMethods1.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 3151
        |> withDiagnosticMessageMatches "This member, function or value declaration may not be declared 'inline'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" id="FS3151" span="(27,20-27,34)">This member, function or value declaration may not be declared 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_CannotInlineVirtualMethod2.fs"|])>]
    let ``CheckingSyntacticTypes - E_CannotInlineVirtualMethod2.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 3151
        |> withDiagnosticMessageMatches "This member, function or value declaration may not be declared 'inline'"
        |> ignore

