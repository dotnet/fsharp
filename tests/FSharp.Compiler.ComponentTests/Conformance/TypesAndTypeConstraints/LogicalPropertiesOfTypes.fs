// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module LogicalPropertiesOfTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"GenericTypeDef.fs"|])>]
    let ``LogicalPropertiesOfTypes - GenericTypeDef.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Abstract.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Abstract.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Array.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Array.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Class.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Class.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Delegate.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Delegate.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_DerivedClass.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_DerivedClass.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_DiscriminatedUnion.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_DiscriminatedUnion.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Exception.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Exception.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Interface.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Interface.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Record.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Record.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Struct.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Struct.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Variable.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Variable.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"BaseTypes_Tuple1.fs"|])>]
    let ``LogicalPropertiesOfTypes - BaseTypes_Tuple1.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"FSharpType_IsRecord.fs"|])>]
    let ``LogicalPropertiesOfTypes - FSharpType_IsRecord.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"AllowNullLiteral01.fs"|])>]
    let ``LogicalPropertiesOfTypes - AllowNullLiteral01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    //<Expects id="FS0043" status="error">The type 'Foo' does not have 'null' as a proper value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"E_AllowNullLiteral01.fs"|])>]
    let ``LogicalPropertiesOfTypes - E_AllowNullLiteral01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "The type 'Foo' does not have 'null' as a proper value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    //<Expects id="FS0934" span="(30,6-30,16)" status="error">Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"E_AllowNullLiteral02.fs"|])>]
    let ``LogicalPropertiesOfTypes - E_AllowNullLiteral02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0934
        |> withDiagnosticMessageMatches "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"SubtypeCoercion01.fs"|])>]
    let ``LogicalPropertiesOfTypes - SubtypeCoercion01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

