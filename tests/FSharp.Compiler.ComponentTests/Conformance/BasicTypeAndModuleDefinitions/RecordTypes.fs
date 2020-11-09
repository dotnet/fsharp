// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module RecordTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"FieldBindingAfterWith01a.fs"|])>]
    let ``RecordTypes - FieldBindingAfterWith01a.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"FieldBindingAfterWith01b.fs"|])>]
    let ``RecordTypes - FieldBindingAfterWith01b.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"EqualAndBoxing01.fs"|])>]
    let ``RecordTypes - EqualAndBoxing01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"NoClashMemberIFaceMember.fs"|])>]
    let ``RecordTypes - NoClashMemberIFaceMember.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_Equals.fs"|])>]
    let ``RecordTypes - Overload_Equals.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``RecordTypes - Overload_GetHashCode.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_ToString.fs"|])>]
    let ``RecordTypes - Overload_ToString.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"ImplicitEquals001.fs"|])>]
    let ``RecordTypes - ImplicitEquals001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"FullyQualify01.fs"|])>]
    let ``RecordTypes - FullyQualify01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"TypeInference01.fs"|])>]
    let ``RecordTypes - TypeInference01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"TypeInference02.fs"|])>]
    let ``RecordTypes - TypeInference02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0764" span="(12,15-12,24)" status="error">No assignment given for field 'Y' of type 'N\.Blue'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_TypeInference01.fs"|])>]
    let ``RecordTypes - E_TypeInference01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0764
        |> withDiagnosticMessageMatches "No assignment given for field 'Y' of type 'N\.Blue'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0764" span="(13,20-13,30)" status="error">No assignment given for field 'Y' of type 'N\.M\.Blue+</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_TypeInference01b.fs"|])>]
    let ``RecordTypes - E_TypeInference01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0764
        |> withDiagnosticMessageMatches "No assignment given for field 'Y' of type 'N\.M\.Blue+"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0656" span="(8,9-8,47)" status="error">This record contains fields from inconsistent types</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_TypeInference02.fs"|])>]
    let ``RecordTypes - E_TypeInference02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0656
        |> withDiagnosticMessageMatches "This record contains fields from inconsistent types"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"RecordCloning01.fs"|])>]
    let ``RecordTypes - RecordCloning01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"RecordCloning02.fs"|])>]
    let ``RecordTypes - RecordCloning02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"RecordCloning03.fs"|])>]
    let ``RecordTypes - RecordCloning03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"StructRecordCloning01.fs"|])>]
    let ``RecordTypes - StructRecordCloning01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"StructRecordCloning02.fs"|])>]
    let ``RecordTypes - StructRecordCloning02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"StructRecordCloning03.fs"|])>]
    let ``RecordTypes - StructRecordCloning03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0001" status="error" span="(7,17)">This expression was expected to have type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_RecordCloning01.fs"|])>]
    let ``RecordTypes - E_RecordCloning01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Syntax01.fs"|])>]
    let ``RecordTypes - Syntax01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0043" status="error">The type 'RecordType' does not have 'null' as a proper value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_RecordsNotNull01.fs"|])>]
    let ``RecordTypes - E_RecordsNotNull01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "The type 'RecordType' does not have 'null' as a proper value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_RecordsNotNull02.fs"|])>]
    let ``RecordTypes - E_RecordsNotNull02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects id="FS0887" span="(9,8-9,22)" status="error">is not an interface type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"E_InheritRecord01.fs"|])>]
    let ``RecordTypes - E_InheritRecord01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0887
        |> withDiagnosticMessageMatches "is not an interface type"

