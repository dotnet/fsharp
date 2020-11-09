// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AttributeUsage =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"MarshalAsAttribute.fs"|])>]
    let ``AttributeUsage - MarshalAsAttribute.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AttributeTargetsIsCtor01.fs"|])>]
    let ``AttributeUsage - AttributeTargetsIsCtor01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AttributeTargetsIsMethod01.fs"|])>]
    let ``AttributeUsage - AttributeTargetsIsMethod01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS0842" span="(24,28)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"E_AttributeTargets01.fs"|])>]
    let ``AttributeUsage - E_AttributeTargets01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS0842" span="(24,14)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"E_AttributeTargets02.fs"|])>]
    let ``AttributeUsage - E_AttributeTargets02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS1213" status="error" span="(10,6-10,9)">Attribute 'System.Diagnostics.ConditionalAttribute' is only valid on methods or attribute classes</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"E_ConditionalAttribute.fs"|])>]
    let ``AttributeUsage - E_ConditionalAttribute.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1213
        |> withDiagnosticMessageMatches "Attribute 'System.Diagnostics.ConditionalAttribute' is only valid on methods or attribute classes"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"ConditionalAttribute.fs"|])>]
    let ``AttributeUsage - ConditionalAttribute.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    // <Expects status="error" id="FS0685" span="(28,1-28,6)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"E_RequiresExplicitTypeArguments01.fs"|])>]
    let ``AttributeUsage - E_RequiresExplicitTypeArguments01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0685
        |> withDiagnosticMessageMatches "The generic function 'Foo' must be given explicit type argument\(s\)"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    // <Expects status="error" id="FS0685" span="(20,5-20,10)">The generic function 'Foo' must be given explicit type argument\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"E_RequiresExplicitTypeArguments02.fs"|])>]
    let ``AttributeUsage - E_RequiresExplicitTypeArguments02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0685
        |> withDiagnosticMessageMatches "The generic function 'Foo' must be given explicit type argument\(s\)"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"RequiresExplicitTypeArguments01.fs"|])>]
    let ``AttributeUsage - RequiresExplicitTypeArguments01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"RequiresExplicitTypeArguments02.fs"|])>]
    let ``AttributeUsage - RequiresExplicitTypeArguments02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AssemblyVersion01.fs"|])>]
    let ``AttributeUsage - AssemblyVersion01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AssemblyVersion02.fs"|])>]
    let ``AttributeUsage - AssemblyVersion02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AssemblyVersion03.fs"|])>]
    let ``AttributeUsage - AssemblyVersion03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"AssemblyVersion04.fs"|])>]
    let ``AttributeUsage - AssemblyVersion04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS2003" status="warning" span="(4,46-4,59)">The attribute System.Reflection.AssemblyVersionAttribute specified version '1\.2\.3\.4\.5\.6', but this value is invalid and has been ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"W_AssemblyVersion01.fs"|])>]
    let ``AttributeUsage - W_AssemblyVersion01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 2003
        |> withDiagnosticMessageMatches "The attribute System.Reflection.AssemblyVersionAttribute specified version '1\.2\.3\.4\.5\.6', but this value is invalid and has been ignored"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS2003" status="warning" span="(4,46-4,55)">The attribute System.Reflection.AssemblyVersionAttribute specified version '1\.2\.\*\.4', but this value is invalid and has been ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"W_AssemblyVersion02.fs"|])>]
    let ``AttributeUsage - W_AssemblyVersion02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 2003
        |> withDiagnosticMessageMatches "The attribute System.Reflection.AssemblyVersionAttribute specified version '1\.2\.\*\.4', but this value is invalid and has been ignored"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/AttributeUsage)
    //<Expects id="FS2003" status="warning">The attribute System.Reflection.AssemblyFileVersionAttribute specified version '9\.8\.7\.6\.5', but this value is invalid and has been ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/AttributeUsage", Includes=[|"X_AssemblyVersion01.fs"|])>]
    let ``AttributeUsage - X_AssemblyVersion01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 2003
        |> withDiagnosticMessageMatches "The attribute System.Reflection.AssemblyFileVersionAttribute specified version '9\.8\.7\.6\.5', but this value is invalid and has been ignored"

