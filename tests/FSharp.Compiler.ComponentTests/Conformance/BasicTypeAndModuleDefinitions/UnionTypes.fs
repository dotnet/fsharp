// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module UnionTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" span="(15,10-15,25)" id="FS3068">The function or member 'toList' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member private list1\.toList : \('a list1 -> 'a list\)'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_GenericFunctionValuedStaticProp01.fs"|])>]
    let ``UnionTypes - E_GenericFunctionValuedStaticProp01.fs - --test:ErrorRanges --warnaserror-`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror-"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3068
        |> withDiagnosticMessageMatches " 'a list\)'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="warning" span="(12,29-12,41)" id="FS1125">The instantiation of the generic type 'list1' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'list1<_>'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"W_GenericFunctionValuedStaticProp02.fs"|])>]
    let ``UnionTypes - W_GenericFunctionValuedStaticProp02.fs - --test:ErrorRanges --warnaserror-`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror-"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 1125
        |> withDiagnosticMessageMatches "'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" span="(7,23-7,29)" id="FS0438">Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'DU'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_Overload_Equals.fs"|])>]
    let ``UnionTypes - E_Overload_Equals.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'DU'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" span="(8,25-8,36)" id="FS0438">Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'DU'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_Overload_GetHashCode.fs"|])>]
    let ``UnionTypes - E_Overload_GetHashCode.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'DU'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"EqualAndBoxing01.fs"|])>]
    let ``UnionTypes - EqualAndBoxing01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_Equals.fs"|])>]
    let ``UnionTypes - Overload_Equals.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``UnionTypes - Overload_GetHashCode.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_ToString.fs"|])>]
    let ``UnionTypes - Overload_ToString.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS0053" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_LowercaseDT.fs"|])>]
    let ``UnionTypes - E_LowercaseDT.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0053
        |> withDiagnosticMessageMatches "Discriminated union cases and exception labels must be uppercase identifiers"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"ImplicitEquals001.fs"|])>]
    let ``UnionTypes - ImplicitEquals001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS0043" status="error">The type 'DU' does not have 'null' as a proper value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionsNotNull01.fs"|])>]
    let ``UnionTypes - E_UnionsNotNull01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "The type 'DU' does not have 'null' as a proper value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionsNotNull02.fs"|])>]
    let ``UnionTypes - E_UnionsNotNull02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"ReflectionOnUnionTypes01.fs"|])>]
    let ``UnionTypes - ReflectionOnUnionTypes01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_InheritUnion.fs"|])>]
    let ``UnionTypes - E_InheritUnion.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0945
        |> withDiagnosticMessageMatches "Cannot inherit a sealed type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS1219" status="error">The union case named 'Tags' conflicts with the generated type 'Tags'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionFieldNamedTag.fs"|])>]
    let ``UnionTypes - E_UnionFieldNamedTag.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1219
        |> withDiagnosticMessageMatches "The union case named 'Tags' conflicts with the generated type 'Tags'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" id="FS1219" span="(7,7-7,11)">The union case named 'Tags' conflicts with the generated type 'Tags'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionFieldNamedTagNoDefault.fs"|])>]
    let ``UnionTypes - E_UnionFieldNamedTagNoDefault.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1219
        |> withDiagnosticMessageMatches "The union case named 'Tags' conflicts with the generated type 'Tags'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="notin" id="FS0023" span="(21,14-21,17)">The member 'Tag' can not be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionMemberNamedTag.fs"|])>]
    let ``UnionTypes - E_UnionMemberNamedTag.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withDiagnosticMessageMatches "The member 'Tag' can not be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" id="FS0023" span="(19,14-19,17)">The member 'Tag' can not be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionMemberNamedTagNoDefault.fs"|])>]
    let ``UnionTypes - E_UnionMemberNamedTagNoDefault.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0023
        |> withDiagnosticMessageMatches "The member 'Tag' can not be defined because the name 'Tag' clashes with the generated property 'Tag' in this type or module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" id="FS0023" span="(19,14-19,18)">The member 'Tags' can not be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionMemberNamedTags.fs"|])>]
    let ``UnionTypes - E_UnionMemberNamedTags.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0023
        |> withDiagnosticMessageMatches "The member 'Tags' can not be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects status="error" id="FS0023" span="(19,14-19,18)">The member 'Tags' can not be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionMemberNamedTagsNoDefault.fs"|])>]
    let ``UnionTypes - E_UnionMemberNamedTagsNoDefault.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0023
        |> withDiagnosticMessageMatches "The member 'Tags' can not be defined because the name 'Tags' clashes with the generated type 'Tags' in this type or module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS3176" span="(10,26-10,27)" status="error">Named field 'A' is used more than once\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionFieldConflictingName.fs"|])>]
    let ``UnionTypes - E_UnionFieldConflictingName.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3176
        |> withDiagnosticMessageMatches "Named field 'A' is used more than once\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS3174" span="(17,12-17,14)" status="error">The union case 'Case1' does not have a field named 'V4'\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_UnionConstructorBadFieldName.fs"|])>]
    let ``UnionTypes - E_UnionConstructorBadFieldName.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3174
        |> withDiagnosticMessageMatches "The union case 'Case1' does not have a field named 'V4'\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    //<Expects id="FS3175" span="(18,20-18,22)" status="error">Union case/exception field 'V1' cannot be used more than once\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"E_FieldNameUsedMulti.fs"|])>]
    let ``UnionTypes - E_FieldNameUsedMulti.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3175
        |> withDiagnosticMessageMatches "Union case/exception field 'V1' cannot be used more than once\."

