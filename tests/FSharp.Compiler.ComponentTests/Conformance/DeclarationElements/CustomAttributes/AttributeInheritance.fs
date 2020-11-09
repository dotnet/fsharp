// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AttributeInheritance =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeInheritance)
    //<Expects id="FS3242" span="(12,3)" status="error">This type does not inherit Attribute, it will not work correctly with other .NET languages.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeInheritance", Includes=[|"InheritedAttribute_001.fs"|])>]
    let ``AttributeInheritance - InheritedAttribute_001.fs - --target:library --warnaserror:3242`` compilation =
        compilation
        |> withOptions ["--target:library"; "--warnaserror:3242"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3242
        |> withDiagnosticMessageMatches "This type does not inherit Attribute, it will not work correctly with other .NET languages."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeInheritance)
    //<Expects id="FS3242" span="(12,3)" status="warning">This type does not inherit Attribute, it will not work correctly with other .NET languages.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeInheritance", Includes=[|"InheritedAttribute_002.fs"|])>]
    let ``AttributeInheritance - InheritedAttribute_002.fs - --target:library`` compilation =
        compilation
        |> withOptions ["--target:library"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3242
        |> withDiagnosticMessageMatches "This type does not inherit Attribute, it will not work correctly with other .NET languages."

