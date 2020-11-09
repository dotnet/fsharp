// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module MethodsAndProperties =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"tupledValueProperties01.fs"|])>]
    let ``MethodsAndProperties - tupledValueProperties01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"genericGenericClass.fs"|])>]
    let ``MethodsAndProperties - genericGenericClass.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"instMembers-class.fs"|])>]
    let ``MethodsAndProperties - instMembers-class.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"instMembers-DU.fs"|])>]
    let ``MethodsAndProperties - instMembers-DU.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"instMembers-Records.fs"|])>]
    let ``MethodsAndProperties - instMembers-Records.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"tupesAndFuncsAsArgs.fs"|])>]
    let ``MethodsAndProperties - tupesAndFuncsAsArgs.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"typeMethodsCurrable.fs"|])>]
    let ``MethodsAndProperties - typeMethodsCurrable.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0438" span="(10,18-10,29)" status="error">Duplicate method\. The method 'get_Property001' has the same name and signature as another method in type 'NM.ClassMembers'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_DuplicateProperty01.fs"|])>]
    let ``MethodsAndProperties - E_DuplicateProperty01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "Duplicate method\. The method 'get_Property001' has the same name and signature as another method in type 'NM.ClassMembers'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"StaticGenericField01.fs"|])>]
    let ``MethodsAndProperties - StaticGenericField01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"staticMembers-class.fs"|])>]
    let ``MethodsAndProperties - staticMembers-class.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"staticMembers-DU.fs"|])>]
    let ``MethodsAndProperties - staticMembers-DU.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"staticMembers-Records.fs"|])>]
    let ``MethodsAndProperties - staticMembers-Records.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"staticMembers-instance.fs"|])>]
    let ``MethodsAndProperties - staticMembers-instance.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" span="(10,14)" id="FS0493">StaticMethod is not an instance method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_useStaticMethodThroughInstance.fs"|])>]
    let ``MethodsAndProperties - E_useStaticMethodThroughInstance.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0493
        |> withDiagnosticMessageMatches "StaticMethod is not an instance method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" id="FS3214" span="(10,14)">Method or object constructor 'DoStuff' is not static$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_useInstMethodThroughStatic.fs"|])>]
    let ``MethodsAndProperties - E_useInstMethodThroughStatic.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3214
        |> withDiagnosticMessageMatches "Method or object constructor 'DoStuff' is not static$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Properties01.fs"|])>]
    let ``MethodsAndProperties - Properties01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Properties02.fs"|])>]
    let ``MethodsAndProperties - Properties02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0554" span="(9,27-9,35)" status="error">Invalid declaration syntax</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_Properties02.fs"|])>]
    let ``MethodsAndProperties - E_Properties02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0554
        |> withDiagnosticMessageMatches "Invalid declaration syntax"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Properties03.fs"|])>]
    let ``MethodsAndProperties - Properties03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Properties04.fs"|])>]
    let ``MethodsAndProperties - Properties04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Properties05.fs"|])>]
    let ``MethodsAndProperties - Properties05.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0001" span="(15,13-15,16)" status="error">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_Properties06.fs"|])>]
    let ``MethodsAndProperties - E_Properties06.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'unit'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"AbstractProperties01.fs"|])>]
    let ``MethodsAndProperties - AbstractProperties01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0435" span="(7,13-7,14)" status="error">The property 'A' of type 'T' has a getter and a setter that do not match\. If one is abstract then the other must be as well</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_AbstractProperties02.fs"|])>]
    let ``MethodsAndProperties - E_AbstractProperties02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0435
        |> withDiagnosticMessageMatches "The property 'A' of type 'T' has a getter and a setter that do not match\. If one is abstract then the other must be as well"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0435" span="(8,14-8,19)" status="error">The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_AbstractProperties03.fs"|])>]
    let ``MethodsAndProperties - E_AbstractProperties03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0435
        |> withDiagnosticMessageMatches "The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0001" status="error" span="(10,66)">This expression was expected to have type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_SettersMustHaveUnit01.fs"|])>]
    let ``MethodsAndProperties - E_SettersMustHaveUnit01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0001" status="error" span="(9,53-9,71)">This expression was expected to have type.*'unit'.*but here has type.*'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_SettersMustHaveUnit02.fs"|])>]
    let ``MethodsAndProperties - E_SettersMustHaveUnit02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.*'unit'.*but here has type.*'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"GetterSetterDiff01.fs"|])>]
    let ``MethodsAndProperties - GetterSetterDiff01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"GetAndSetKeywords01.fs"|])>]
    let ``MethodsAndProperties - GetAndSetKeywords01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0435" status="error" span="(10,14-10,19)">The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_AbstractAndConcereteProp.fs"|])>]
    let ``MethodsAndProperties - E_AbstractAndConcereteProp.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0435
        |> withDiagnosticMessageMatches "The property 'State' of type 'X' has a getter and a setter that do not match\. If one is abstract then the other must be as well\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"multiParamIndexer.fs"|])>]
    let ``MethodsAndProperties - multiParamIndexer.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Indexer01.fs"|])>]
    let ``MethodsAndProperties - Indexer01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Indexer02.fs"|])>]
    let ``MethodsAndProperties - Indexer02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"Indexer02.fs"|])>]
    let ``MethodsAndProperties - Indexer02.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"TupledIndexer.fs"|])>]
    let ``MethodsAndProperties - TupledIndexer.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0039" status="error">The type 'Foo' does not define the field, constructor or member 'Item'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_IndexerNotSpecified01.fs"|])>]
    let ``MethodsAndProperties - E_IndexerNotSpecified01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'Foo' does not define the field, constructor or member 'Item'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0001" span="(14,27-14,38)" status="error">This expression was expected to have type.    ''a \[\]'    .but here has type.    'int \[,,,\]'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_IndexerArityMismatch01.fs"|])>]
    let ``MethodsAndProperties - E_IndexerArityMismatch01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    ''a \[\]'    .but here has type.    'int \[,,,\]'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0001" span="(14,27-14,36)" status="error">This expression was expected to have type.    ''a \[,\]'    .but here has type.    'int \[,,,]'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_IndexerArityMismatch02.fs"|])>]
    let ``MethodsAndProperties - E_IndexerArityMismatch02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    ''a \[,\]'    .but here has type.    'int \[,,,]'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" span="(5,11-5,16)" id="FS0752">The operator 'expr\.\[idx\]' has been used on an object of indeterminate type based on information prior to this program point\. Consider adding further type constraints$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_IndexerIndeterminateType01.fs"|])>]
    let ``MethodsAndProperties - E_IndexerIndeterminateType01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0752
        |> withDiagnosticMessageMatches "The operator 'expr\.\[idx\]' has been used on an object of indeterminate type based on information prior to this program point\. Consider adding further type constraints$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" span="(6,12)" id="FS0673">This instance member needs a parameter to represent the object being invoked\. Make the member static or use the notation 'member x\.Member\(args\) = \.\.\.'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_UndefinedThisVariable.fs"|])>]
    let ``MethodsAndProperties - E_UndefinedThisVariable.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0673
        |> withDiagnosticMessageMatches "This instance member needs a parameter to represent the object being invoked\. Make the member static or use the notation 'member x\.Member\(args\) = \.\.\.'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0673" status="error">This instance member needs a parameter to represent the object being invoked</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_UndefinedThisVariable02.fs"|])>]
    let ``MethodsAndProperties - E_UndefinedThisVariable02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0673
        |> withDiagnosticMessageMatches "This instance member needs a parameter to represent the object being invoked"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects id="FS0038" span="(7,24-7,28)" status="error">'this' is bound twice in this pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_OutscopeThisPtr01.fs"|])>]
    let ``MethodsAndProperties - E_OutscopeThisPtr01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0038
        |> withDiagnosticMessageMatches "'this' is bound twice in this pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"RecursiveLetValues.fs"|])>]
    let ``MethodsAndProperties - RecursiveLetValues.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" id="FS0039" span="(21,10-21,13)">The type 'FaaBor' does not define the field, constructor or member 'Foo'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_ActivePatternMember01.fs"|])>]
    let ``MethodsAndProperties - E_ActivePatternMember01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'FaaBor' does not define the field, constructor or member 'Foo'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties)
    //<Expects status="error" id="FS0827" span="(6,12-6,27)">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/MethodsAndProperties", Includes=[|"E_ActivePatternMember02.fs"|])>]
    let ``MethodsAndProperties - E_ActivePatternMember02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

