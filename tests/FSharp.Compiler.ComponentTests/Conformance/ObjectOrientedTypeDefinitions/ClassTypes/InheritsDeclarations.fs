// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module InheritsDeclarations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    //<Expects status="error" span="(10,73)" id="FS0859">No abstract property was found that corresponds to this override</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"E_DefaultNoAbstractInBaseClass.fs"|])>]
    let ``InheritsDeclarations - E_DefaultNoAbstractInBaseClass.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0859
        |> withDiagnosticMessageMatches "No abstract property was found that corresponds to this override"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    //<Expects status="error" span="(22,16)" id="FS0859">No abstract property was found that corresponds to this override$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"E_MemberNoAbstractInBaseClass.fs"|])>]
    let ``InheritsDeclarations - E_MemberNoAbstractInBaseClass.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0859
        |> withDiagnosticMessageMatches "No abstract property was found that corresponds to this override$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"BaseValue01.fs"|])>]
    let ``InheritsDeclarations - BaseValue01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    //<Expects id="FS0946" status="error">Cannot inherit from interface type\. Use interface \.\.\. with instead</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"E_InheritInterface01.fs"|])>]
    let ``InheritsDeclarations - E_InheritInterface01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0946
        |> withDiagnosticMessageMatches "Cannot inherit from interface type\. Use interface \.\.\. with instead"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    //<Expects id="FS0753" span="(9,19)" status="error">Cannot inherit from a variable type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"E_InheritFromGenericType01.fs"|])>]
    let ``InheritsDeclarations - E_InheritFromGenericType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0753
        |> withDiagnosticMessageMatches "Cannot inherit from a variable type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"InheritFromAbstractClass.fs"|])>]
    let ``InheritsDeclarations - InheritFromAbstractClass.fs - --reference:CSharpClassLibrary.dll`` compilation =
        compilation
        |> withOptions ["--reference:CSharpClassLibrary.dll"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations", Includes=[|"Regression01.fs"|])>]
    let ``InheritsDeclarations - Regression01.fs - --reference:CSharpClassLibrary.dll`` compilation =
        compilation
        |> withOptions ["--reference:CSharpClassLibrary.dll"]
        |> typecheck
        |> shouldSucceed

