// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecursiveSafetyAnalysis =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects id="FS1118" span="(8,15-8,19)" status="error">Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RecursiveInline.fs"|])>]
    let ``E_RecursiveInline_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 1118
        |> withDiagnosticMessageMatches "Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InfiniteRecursiveExplicitConstructor.fs"|])>]
    let ``InfiniteRecursiveExplicitConstructor_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeDeclaration02.fs"|])>]
    let ``E_TypeDeclaration02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "Type constraint mismatch"
        |> ignore

