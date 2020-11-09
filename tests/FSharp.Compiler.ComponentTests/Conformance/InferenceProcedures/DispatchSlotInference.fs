// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module DispatchSlotInference =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/DispatchSlotInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/DispatchSlotInference", Includes=[|"GenInterfaceWGenMethods01.fs"|])>]
    let ``DispatchSlotInference - GenInterfaceWGenMethods01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/DispatchSlotInference)
    //<Expects span="(16,5-16,11)" status="error" id="FS0030">Value restriction\. The value 'result' has been inferred to have generic type.    val result : '_a array when '_a : equality and '_a : null    .Either define 'result' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/DispatchSlotInference", Includes=[|"E_GenInterfaceWGenMethods01.fs"|])>]
    let ``DispatchSlotInference - E_GenInterfaceWGenMethods01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0030
        |> withDiagnosticMessageMatches "Value restriction\. The value 'result' has been inferred to have generic type.    val result : '_a array when '_a : equality and '_a : null    .Either define 'result' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/DispatchSlotInference)
    //<Expects span="(19,18-19,19)" status="error" id="FS0361">The override 'M : int -> int' implements more than one abstract slot, e\.g\. 'abstract member IB\.M : int -> int' and 'abstract member IA\.M : int -> int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/DispatchSlotInference", Includes=[|"E_MoreThanOneDispatchSlotMatch01.fs"|])>]
    let ``DispatchSlotInference - E_MoreThanOneDispatchSlotMatch01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0361
        |> withDiagnosticMessageMatches " int'"

