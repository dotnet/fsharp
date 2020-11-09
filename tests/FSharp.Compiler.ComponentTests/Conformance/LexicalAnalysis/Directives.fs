// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Directives =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Directives)
    //<Expects span="(3,1-3,17)" status="warning" id="FS0203">Invalid warning number 'FS0000'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"W_nowarn_invalid01.fs"|])>]
    let ``Directives - W_nowarn_invalid01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0203
        |> withDiagnosticMessageMatches "Invalid warning number 'FS0000'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Directives)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn01.fs"|])>]
    let ``Directives - multiple_nowarn01.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Directives)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn_many.fs"|])>]
    let ``Directives - multiple_nowarn_many.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Directives)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn_one.fs"|])>]
    let ``Directives - multiple_nowarn_one.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

