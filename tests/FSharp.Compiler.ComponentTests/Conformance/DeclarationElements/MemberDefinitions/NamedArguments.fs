// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module NamedArguments =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    //<Expects id="FS0003" span="(12,1-12,8)" status="error">This value is not a function and cannot be applied</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"E_SyntaxErrors01.fs"|])>]
    let ``NamedArguments - E_SyntaxErrors01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003
        |> withDiagnosticMessageMatches "This value is not a function and cannot be applied"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"SanityCheck.fs"|])>]
    let ``NamedArguments - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    //<Expects id="FS0691" span="(9,33)" status="error">Named arguments must appear after all other arguments</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"E_NonNamedAfterNamed.fs"|])>]
    let ``NamedArguments - E_NonNamedAfterNamed.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0691
        |> withDiagnosticMessageMatches "Named arguments must appear after all other arguments"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    //<Expects status="error" span="(7,1)" id="FS0364">The named argument 'param1' has been assigned more than one value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"E_ReusedParam.fs"|])>]
    let ``NamedArguments - E_ReusedParam.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0364
        |> withDiagnosticMessageMatches "The named argument 'param1' has been assigned more than one value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"genericNamedParams.fs"|])>]
    let ``NamedArguments - genericNamedParams.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"mixNamedNonNamed.fs"|])>]
    let ``NamedArguments - mixNamedNonNamed.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"refLibsHaveNamedParams.fs"|])>]
    let ``NamedArguments - refLibsHaveNamedParams.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    //<Expects id="FS0500" status="error">The member or object constructor 'NamedMeth1' requires 4 argument\(s\) but is here given 2 unnamed and 3 named argument\(s\)\. The required signature is 'abstract member IFoo\.NamedMeth1 : arg1:int \* arg2:int \* arg3:int \* arg4:int -> float'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"E_NumParamMismatch01.fs"|])>]
    let ``NamedArguments - E_NumParamMismatch01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0500
        |> withDiagnosticMessageMatches " float'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction01NamedExtensions.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction01NamedExtensions.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction01NamedExtensionsInheritance.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction01NamedExtensionsInheritance.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction01NamedExtensionsOptional.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction01NamedExtensionsOptional.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction02NamedExtensions.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction02NamedExtensions.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction01.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"PropertySetterAfterConstruction02.fs"|])>]
    let ``NamedArguments - PropertySetterAfterConstruction02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/NamedArguments)
    //<Expects id="FS0508" status="error" span="(5,9)">No accessible member or object constructor named 'ProcessStartInfo' takes 0 arguments\. The named argument 'Argument' doesn't correspond to any argument or settable return property for any overload</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/NamedArguments", Includes=[|"E_MisspeltParam01.fs"|])>]
    let ``NamedArguments - E_MisspeltParam01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0508
        |> withDiagnosticMessageMatches "No accessible member or object constructor named 'ProcessStartInfo' takes 0 arguments\. The named argument 'Argument' doesn't correspond to any argument or settable return property for any overload"

