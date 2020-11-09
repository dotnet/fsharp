// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module LineDirectives =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/LineDirectives)
    //<Expects id="FS1156" span="(1006,9)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/LineDirectives", Includes=[|"Line01.fs"|])>]
    let ``LineDirectives - Line01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal. Valid numeric literals include"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/LineDirectives)
    //<Expects id="FS1156" span="(1006,9)" status="error">This is not a valid numeric literal. Valid numeric literals include</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/LineDirectives", Includes=[|"Line01.fs"|])>]
    let ``LineDirectives - Line01.fs - --warnaserror+ --nowarn:75`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:75"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal. Valid numeric literals include"

