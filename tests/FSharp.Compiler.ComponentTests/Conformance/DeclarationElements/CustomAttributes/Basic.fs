// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"StructLayout.fs"|])>]
    let ``Basic - StructLayout.fs - -a --nowarn:9 --warnaserror`` compilation =
        compilation
        |> withOptions ["-a"; "--nowarn:9"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(24,1-25,1)" id="FS1211">The FieldOffset attribute can only be placed on members of types marked with the StructLayout\(LayoutKind\.Explicit\)$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayout.fs"|])>]
    let ``Basic - E_StructLayout.fs - -a --test:ErrorRanges --flaterrors --nowarn:9`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"; "--flaterrors"; "--nowarn:9"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1211
        |> withDiagnosticMessageMatches "The FieldOffset attribute can only be placed on members of types marked with the StructLayout\(LayoutKind\.Explicit\)$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"InExternDecl.fs"|])>]
    let ``Basic - InExternDecl.fs - -a --warnaserror+`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"FunctionArg01.fs"|])>]
    let ``Basic - FunctionArg01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"Function01.fs"|])>]
    let ``Basic - Function01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"ReturnType01.fs"|])>]
    let ``Basic - ReturnType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"ReturnType02.fs"|])>]
    let ``Basic - ReturnType02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"ReturnType03.fs"|])>]
    let ``Basic - ReturnType03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0429" span="(16,28-16,31)" status="error">The attribute type 'CA1' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"W_ReturnType03b.fs"|])>]
    let ``Basic - W_ReturnType03b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0429
        |> withDiagnosticMessageMatches "The attribute type 'CA1' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"ArrayParam.fs"|])>]
    let ``Basic - ArrayParam.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"AttribWithEnumFlags01.fs"|])>]
    let ``Basic - AttribWithEnumFlags01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"SanityCheck01.fs"|])>]
    let ``Basic - SanityCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0841" status="error">This attribute is not valid for use on this language element\. Assembly attributes should be attached to a 'do \(\)' declaration, if necessary within an F# module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication01.fs"|])>]
    let ``Basic - E_AttributeApplication01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0841
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element\. Assembly attributes should be attached to a 'do \(\)' declaration, if necessary within an F# module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0429" span="(6,15-6,22)" status="error">The attribute type 'MeasureAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication02.fs"|])>]
    let ``Basic - E_AttributeApplication02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0429
        |> withDiagnosticMessageMatches "The attribute type 'MeasureAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0842" span="(15,7-15,17)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication03.fs"|])>]
    let ``Basic - E_AttributeApplication03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0842" span="(14,3-14,13)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication04.fs"|])>]
    let ``Basic - E_AttributeApplication04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(8,7-8,8)" id="FS0842">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication05.fs"|])>]
    let ``Basic - E_AttributeApplication05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(8,5-8,20)" id="FS0824">Attributes are not permitted on 'let' bindings in expressions$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication06.fs"|])>]
    let ``Basic - E_AttributeApplication06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0824
        |> withDiagnosticMessageMatches "Attributes are not permitted on 'let' bindings in expressions$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(16,13)" id="FS0850">This attribute cannot be used in this version of F#</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeApplication07.fs"|])>]
    let ``Basic - E_AttributeApplication07.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0850
        |> withDiagnosticMessageMatches "This attribute cannot be used in this version of F#"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"TypesAsAttrArgs01.fs"|])>]
    let ``Basic - TypesAsAttrArgs01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"FreeTypeVariable01.fs"|])>]
    let ``Basic - FreeTypeVariable01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects id="FS0840" status="error">Unrecognized attribute target\. Valid attribute targets are 'assembly', 'module', 'type', 'method', 'property', 'return', 'param', 'field', 'event', 'constructor'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_AttributeTargetSpecifications.fs"|])>]
    let ``Basic - E_AttributeTargetSpecifications.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0840
        |> withDiagnosticMessageMatches "Unrecognized attribute target\. Valid attribute targets are 'assembly', 'module', 'type', 'method', 'property', 'return', 'param', 'field', 'event', 'constructor'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"ParamArrayAttrUsage.fs"|])>]
    let ``Basic - ParamArrayAttrUsage.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"W_StructLayoutSequentialPos_Record.fs"|])>]
    let ``Basic - W_StructLayoutSequentialPos_Record.fs - --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"W_StructLayoutSequentialPos_AbstractClass.fs"|])>]
    let ``Basic - W_StructLayoutSequentialPos_AbstractClass.fs - --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"W_StructLayoutSequentialPos_ClassnoCtr.fs"|])>]
    let ``Basic - W_StructLayoutSequentialPos_ClassnoCtr.fs - --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"W_StructLayoutSequentialPos_ClassExpliCtr.fs"|])>]
    let ``Basic - W_StructLayoutSequentialPos_ClassExpliCtr.fs - --test:ErrorRanges --warnaserror+`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"StructLayoutSequentialPos_Exception.fs"|])>]
    let ``Basic - StructLayoutSequentialPos_Exception.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(8,10-8,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayoutSequentialNeg_Interface.fs"|])>]
    let ``Basic - E_StructLayoutSequentialNeg_Interface.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0937
        |> withDiagnosticMessageMatches "Only structs and classes without primary constructors may be given the 'StructLayout' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(9,10-9,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayoutSequentialNeg_AbstractClass.fs"|])>]
    let ``Basic - E_StructLayoutSequentialNeg_AbstractClass.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0937
        |> withDiagnosticMessageMatches "Only structs and classes without primary constructors may be given the 'StructLayout' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(9,10-9,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayoutSequentialNeg_DU1.fs"|])>]
    let ``Basic - E_StructLayoutSequentialNeg_DU1.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0937
        |> withDiagnosticMessageMatches "Only structs and classes without primary constructors may be given the 'StructLayout' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(9,10-9,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayoutSequentialNeg_DU2.fs"|])>]
    let ``Basic - E_StructLayoutSequentialNeg_DU2.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0937
        |> withDiagnosticMessageMatches "Only structs and classes without primary constructors may be given the 'StructLayout' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" span="(9,10-9,12)" id="FS0937">Only structs and classes without primary constructors may be given the 'StructLayout' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_StructLayoutSequentialNeg_Delegate.fs"|])>]
    let ``Basic - E_StructLayoutSequentialNeg_Delegate.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0937
        |> withDiagnosticMessageMatches "Only structs and classes without primary constructors may be given the 'StructLayout' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"TypeofTypedefofInAttribute.fs"|])>]
    let ``Basic - TypeofTypedefofInAttribute.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/CustomAttributes/Basic)
    //<Expects status="error" id="FS1196" span="(51,6-51,15)">The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/CustomAttributes/Basic", Includes=[|"E_UseNullAsTrueValue01.fs"|])>]
    let ``Basic - E_UseNullAsTrueValue01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1196
        |> withDiagnosticMessageMatches "The 'UseNullAsTrueValue' attribute flag may only be used with union types that have one nullary case and at least one non-nullary case"

