// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering.Basic

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module OffsideExceptions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/OffsideExceptions", Includes=[|"Offside01a.fs"|])>]
    let ``OffsideExceptions - Offside01a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/OffsideExceptions", Includes=[|"Offside01b.fs"|])>]
    let ``OffsideExceptions - Offside01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects span="(11,1-11,1)" status="error" id="FS0010">Incomplete structured construct at or before this point in implementation file$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/OffsideExceptions", Includes=[|"Offside01c.fs"|])>]
    let ``OffsideExceptions - Offside01c.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in implementation file$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/OffsideExceptions", Includes=[|"InfixTokenPlusOne.fs"|])>]
    let ``OffsideExceptions - InfixTokenPlusOne.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

