// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module EnumTypes =

    // Error tests - should fail with expected error codes

    [<Theory; FileInlineData("E_BoolUnderlyingType.fs")>]
    let ``E_BoolUnderlyingType_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 951

    [<Theory; FileInlineData("E_DiscriminantOfDifferentTypes.fs")>]
    let ``E_DiscriminantOfDifferentTypes_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1

    [<Theory; FileInlineData("E_InvalidCase01.fs")>]
    let ``E_InvalidCase01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 745

    [<Theory; FileInlineData("E_NamedTypeInScope.fs")>]
    let ``E_NamedTypeInScope_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 37

    [<Theory; FileInlineData("E_NeedToQualify01.fs")>]
    let ``E_NeedToQualify01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_NoMethodsOnEnums01.fs")>]
    let ``E_NoMethodsOnEnums01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 896

    [<Theory; FileInlineData("E_NoValueFieldOnEnum.fs")>]
    let ``E_NoValueFieldOnEnum_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_NonInt32Enums01.fs")>]
    let ``E_NonInt32Enums01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [951; 886; 886; 951; 951; 951; 951; 951; 951; 951]

    // Success tests - should compile successfully

    [<Theory; FileInlineData("AttributesOn01.fs")>]
    let ``AttributesOn01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("BinaryOr01.fs")>]
    let ``BinaryOr01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("EqualAndBoxing01.fs")>]
    let ``EqualAndBoxing01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("EqualsTag.fs")>]
    let ``EqualsTag_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NamedTypeInScope.fs")>]
    let ``NamedTypeInScope_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NonInt32Enums01.fs")>]
    let ``NonInt32Enums01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Simple001.fs")>]
    let ``Simple001_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ConsumeFromCS.fs")>]
    let ``ConsumeFromCS_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    // Note: CallCSharpEnum.fs requires C# interop (PRECMD) - skipping for now
