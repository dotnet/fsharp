// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module RecursiveSafetyAnalysis =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects id="FS1118" span="(8,15-8,19)" status="error">Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_RecursiveInline.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_RecursiveInline.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1118
        |> withDiagnosticMessageMatches "Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"InfiniteRecursiveExplicitConstructor.fs"|])>]
    let ``RecursiveSafetyAnalysis - InfiniteRecursiveExplicitConstructor.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_TypeDeclaration02.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_TypeDeclaration02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "Type constraint mismatch"
        |> ignore

