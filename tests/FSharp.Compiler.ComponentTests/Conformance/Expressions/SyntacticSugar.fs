// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SyntacticSugar =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    //<Expects status="error" id="FS0003"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"infix_op01.fs"|])>]
    let ``SyntacticSugar - infix_op01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices01.fs"|])>]
    let ``SyntacticSugar - Slices01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices02.fs"|])>]
    let ``SyntacticSugar - Slices02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices03.fs"|])>]
    let ``SyntacticSugar - Slices03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices04.fs"|])>]
    let ``SyntacticSugar - Slices04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices06.fs"|])>]
    let ``SyntacticSugar - Slices06.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"Slices07.fs"|])>]
    let ``SyntacticSugar - Slices07.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugar)
    //<Expects id="FS0039" status="error">The type 'DU' does not define the field, constructor or member 'GetSlice'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugar", Includes=[|"E_GetSliceNotDef01.fs"|])>]
    let ``SyntacticSugar - E_GetSliceNotDef01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'DU' does not define the field, constructor or member 'GetSlice'"

