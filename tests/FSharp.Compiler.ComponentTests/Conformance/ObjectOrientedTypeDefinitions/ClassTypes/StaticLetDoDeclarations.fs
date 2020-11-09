// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module StaticLetDoDeclarations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"WithValueType.fs"|])>]
    let ``StaticLetDoDeclarations - WithValueType.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"WithReferenceType.fs"|])>]
    let ``StaticLetDoDeclarations - WithReferenceType.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"WithStringType.fs"|])>]
    let ``StaticLetDoDeclarations - WithStringType.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"Attributes01.fs"|])>]
    let ``StaticLetDoDeclarations - Attributes01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects status="error" span="(11,23-11,24)" id="FS0039">The value, namespace, type or module 'n' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"E_LexicalScoping01.fs"|])>]
    let ``StaticLetDoDeclarations - E_LexicalScoping01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value, namespace, type or module 'n' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"LexicalScoping01.fs"|])>]
    let ``StaticLetDoDeclarations - LexicalScoping01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects id="FS0874" span="(7,37-7,38)" status="error">Mutable 'let' bindings can't be recursive or defined in recursive modules or namespaces</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"E_RecMutable01.fs"|])>]
    let ``StaticLetDoDeclarations - E_RecMutable01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0874
        |> withDiagnosticMessageMatches "Mutable 'let' bindings can't be recursive or defined in recursive modules or namespaces"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"RecNonMutable01.fs"|])>]
    let ``StaticLetDoDeclarations - RecNonMutable01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/StaticLetDoDeclarations", Includes=[|"Offside01.fs"|])>]
    let ``StaticLetDoDeclarations - Offside01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

