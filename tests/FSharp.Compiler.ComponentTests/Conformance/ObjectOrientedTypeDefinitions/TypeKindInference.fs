// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeKindInference =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects id="FS0946" span="(32,4-32,20)" status="error">Cannot inherit from interface type\. Use interface \.\.\. with instead</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface002e.fs"|])>]
    let ``TypeKindInference - infer_interface002e.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0946
        |> withDiagnosticMessageMatches "Cannot inherit from interface type\. Use interface \.\.\. with instead"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects id="FS0927" status="error" span="(14,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class001e.fs"|])>]
    let ``TypeKindInference - infer_class001e.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0927
        |> withDiagnosticMessageMatches "The kind of the type specified by its attributes does not match the kind implied by its definition"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects id="FS0927" status="error">kind.*does not match</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_struct001e.fs"|])>]
    let ``TypeKindInference - infer_struct001e.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0927
        |> withDiagnosticMessageMatches "kind.*does not match"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects id="FS0926" status="error">multiple kinds</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_struct002e.fs"|])>]
    let ``TypeKindInference - infer_struct002e.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0926
        |> withDiagnosticMessageMatches "multiple kinds"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects id="FS0927" status="error" span="(14,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface001e.fs"|])>]
    let ``TypeKindInference - infer_interface001e.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0927
        |> withDiagnosticMessageMatches "The kind of the type specified by its attributes does not match the kind implied by its definition"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    //<Expects status="error" span="(19,6)" id="FS0054">This type is 'abstract' since some abstract members have not been given an implementation\. If this is intentional then add the '\[<AbstractClass>\]' attribute to your type\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface003e.fs"|])>]
    let ``TypeKindInference - infer_interface003e.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0054
        |> withDiagnosticMessageMatches "\]' attribute to your type\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class001.fs"|])>]
    let ``TypeKindInference - infer_class001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class002.fs"|])>]
    let ``TypeKindInference - infer_class002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class003.fs"|])>]
    let ``TypeKindInference - infer_class003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class004.fs"|])>]
    let ``TypeKindInference - infer_class004.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_class005.fs"|])>]
    let ``TypeKindInference - infer_class005.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface001.fs"|])>]
    let ``TypeKindInference - infer_interface001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface002.fs"|])>]
    let ``TypeKindInference - infer_interface002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface003.fs"|])>]
    let ``TypeKindInference - infer_interface003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_interface004.fs"|])>]
    let ``TypeKindInference - infer_interface004.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_struct001.fs"|])>]
    let ``TypeKindInference - infer_struct001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_struct002.fs"|])>]
    let ``TypeKindInference - infer_struct002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/TypeKindInference)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/TypeKindInference", Includes=[|"infer_struct003.fs"|])>]
    let ``TypeKindInference - infer_struct003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

