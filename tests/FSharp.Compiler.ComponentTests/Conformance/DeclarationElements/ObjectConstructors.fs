// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects id="FS0557" span="(7,32-7,35)" status="error">A getter property is expected to be a function, e\.g\. 'get\(\) = \.\.\.' or 'get\(index\) = \.\.\.'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_MissingArgumentForGetterProp01.fs"|])>]
    let ``ObjectConstructors - E_MissingArgumentForGetterProp01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0557
        |> withDiagnosticMessageMatches "A getter property is expected to be a function, e\.g\. 'get\(\) = \.\.\.' or 'get\(index\) = \.\.\.'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ChainingCtors.fs"|])>]
    let ``ObjectConstructors - ChainingCtors.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"GenericTypesInObjCtor.fs"|])>]
    let ``ObjectConstructors - GenericTypesInObjCtor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ObjCtorParamsToBaseclass.fs"|])>]
    let ``ObjectConstructors - ObjCtorParamsToBaseclass.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"SanityCheck01.fs"|])>]
    let ``ObjectConstructors - SanityCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"SanityCheck02.fs"|])>]
    let ``ObjectConstructors - SanityCheck02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ValOKWithoutImplicitCtor.fs"|])>]
    let ``ObjectConstructors - ValOKWithoutImplicitCtor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects id="FS0762" span="(11,13)" status="error">Constructors for the type 't' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_ImplicitExplicitCTors.fs"|])>]
    let ``ObjectConstructors - E_ImplicitExplicitCTors.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0762
        |> withDiagnosticMessageMatches "Constructors for the type 't' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects status="error" span="(6,5)" id="FS0963">This definition may only be used in a type with a primary constructor\. Consider adding arguments to your type definition, e\.g\. 'type X\(args\) = \.\.\.'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_NoLetBindingsWOObjCtor.fs"|])>]
    let ``ObjectConstructors - E_NoLetBindingsWOObjCtor.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0963
        |> withDiagnosticMessageMatches "This definition may only be used in a type with a primary constructor\. Consider adding arguments to your type definition, e\.g\. 'type X\(args\) = \.\.\.'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects id="FS0866" status="error" span="(8,6)">Interfaces cannot contain definitions of object constructors$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_NoObjectConstructorOnInterfaces.fs"|])>]
    let ``ObjectConstructors - E_NoObjectConstructorOnInterfaces.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0866
        |> withDiagnosticMessageMatches "Interfaces cannot contain definitions of object constructors$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"AlternateGenericTypeSyntax01.fs"|])>]
    let ``ObjectConstructors - AlternateGenericTypeSyntax01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"MutuallyRecursive01.fs"|])>]
    let ``ObjectConstructors - MutuallyRecursive01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ImplicitCtorsCallingBaseclassPassingSelf.fs"|])>]
    let ``ObjectConstructors - ImplicitCtorsCallingBaseclassPassingSelf.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ExplicitCtors01.fs"|])>]
    let ``ObjectConstructors - ExplicitCtors01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by the type annotations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"WarningforLessGenericthanIndicated.fs"|])>]
    let ``ObjectConstructors - WarningforLessGenericthanIndicated.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ObjectConstructors)
    //<Expects id="FS0765" span="(16,11-16,36)" status="error">Extraneous fields have been given values</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_ExtraneousFields01.fs"|])>]
    let ``ObjectConstructors - E_ExtraneousFields01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0765
        |> withDiagnosticMessageMatches "Extraneous fields have been given values"

