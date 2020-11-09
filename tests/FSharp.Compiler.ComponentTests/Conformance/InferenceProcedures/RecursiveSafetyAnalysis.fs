// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module RecursiveSafetyAnalysis =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects id="FS0001" span="(8,25-8,34)" status="error">This expression was expected to have type.    'bogusType'    .but here has type.    'Map<'a,'b>'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_CyclicReference01.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_CyclicReference01.fs - --mlcompatibility --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--mlcompatibility"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"RecursiveTypeDeclarations01.fs"|])>]
    let ``RecursiveSafetyAnalysis - RecursiveTypeDeclarations01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"RecursiveTypeDeclarations02.fs"|])>]
    let ``RecursiveSafetyAnalysis - RecursiveTypeDeclarations02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"RecursiveValueDeclarations01.fs"|])>]
    let ``RecursiveSafetyAnalysis - RecursiveValueDeclarations01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects id="FS1118" span="(8,15-8,19)" status="error">Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_RecursiveInline.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_RecursiveInline.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1118
        |> withDiagnosticMessageMatches "Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"InfiniteRecursiveExplicitConstructor.fs"|])>]
    let ``RecursiveSafetyAnalysis - InfiniteRecursiveExplicitConstructor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects id="FS0954" span="(6,6-6,8)" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_VariationsOnRecursiveStruct.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_VariationsOnRecursiveStruct.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0954
        |> withDiagnosticMessageMatches "This type definition involves an immediate cyclic reference through a struct field or inheritance relation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects status="error" id="FS0001" span="(15,14-15,20)">This expression was expected to have type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_TypeDeclaration01.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_TypeDeclaration01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/RecursiveSafetyAnalysis)
    //<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/RecursiveSafetyAnalysis", Includes=[|"E_TypeDeclaration02.fs"|])>]
    let ``RecursiveSafetyAnalysis - E_TypeDeclaration02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "Type constraint mismatch"

