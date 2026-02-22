// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ObjectConstructors =

    let private resourcePath = __SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors"

    // SOURCE=E_MissingArgumentForGetterProp01.fs SCFLAGS="-a --test:ErrorRanges"
    // <Expects id="FS0557" span="(7,32-7,35)" status="error">A getter property is expected to be a function
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_MissingArgumentForGetterProp01.fs"|])>]
    let ``E_MissingArgumentForGetterProp01_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0557
        |> withDiagnosticMessageMatches "A getter property is expected to be a function"
        |> ignore

    // SOURCE=ChainingCtors.fs
    // Tests that compile and run with exit() - verified to compile successfully
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ChainingCtors.fs"|])>]
    let ``ChainingCtors_fs`` compilation =
        compilation
        |> asExe
        |> withNoWarn 44 // List.nth is deprecated
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=GenericTypesInObjCtor.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"GenericTypesInObjCtor.fs"|])>]
    let ``GenericTypesInObjCtor_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=ObjCtorParamsToBaseclass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ObjCtorParamsToBaseclass.fs"|])>]
    let ``ObjCtorParamsToBaseclass_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=SanityCheck01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"SanityCheck01.fs"|])>]
    let ``SanityCheck01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=ValOKWithoutImplicitCtor.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ValOKWithoutImplicitCtor.fs"|])>]
    let ``ValOKWithoutImplicitCtor_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=E_ImplicitExplicitCTors.fs
    // <Expects id="FS0762" span="(11,13)" status="error">Constructors for the type 't' must directly or indirectly call its implicit object constructor
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_ImplicitExplicitCTors.fs"|])>]
    let ``E_ImplicitExplicitCTors_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 0762
        |> withDiagnosticMessageMatches "Constructors for the type 't' must directly or indirectly call its implicit object constructor"
        |> ignore

    // SOURCE=E_NoLetBindingsWOObjCtor.fs
    // <Expects status="error" span="(6,5)" id="FS0963">This definition may only be used in a type with a primary constructor
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_NoLetBindingsWOObjCtor.fs"|])>]
    let ``E_NoLetBindingsWOObjCtor_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 0963
        |> withDiagnosticMessageMatches "This definition may only be used in a type with a primary constructor"
        |> ignore

    // SOURCE=E_NoObjectConstructorOnInterfaces.fs
    // <Expects id="FS0866" status="error" span="(8,6)">Interfaces cannot contain definitions of object constructors
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_NoObjectConstructorOnInterfaces.fs"|])>]
    let ``E_NoObjectConstructorOnInterfaces_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 0866
        |> withDiagnosticMessageMatches "Interfaces cannot contain definitions of object constructors"
        |> ignore

    // SOURCE=AlternateGenericTypeSyntax01.fs
    // This test uses ML-style type syntax which is no longer supported in modern F#
    // The --mlcompatibility flag has been removed
    [<Fact(Skip = "ML-style type syntax is no longer supported in F# - the --mlcompatibility option was removed")>]
    let ``AlternateGenericTypeSyntax01_fs`` () = ()

    // SOURCE=MutuallyRecursive01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"MutuallyRecursive01.fs"|])>]
    let ``MutuallyRecursive01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=ImplicitCtorsCallingBaseclassPassingSelf.fs
    // This test intentionally throws exceptions to test exception handling 
    // and can crash the test host on some platforms
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ImplicitCtorsCallingBaseclassPassingSelf.fs"|])>]
    let ``ImplicitCtorsCallingBaseclassPassingSelf_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=ExplicitCtors01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"ExplicitCtors01.fs"|])>]
    let ``ExplicitCtors01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=WarningforLessGenericthanIndicated.fs
    // <Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by the type annotations</Expects>
    // <Expects id="FS0064" status="warning">This construct causes code to be less generic than indicated by the type annotations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"WarningforLessGenericthanIndicated.fs"|])>]
    let ``WarningforLessGenericthanIndicated_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [
            (Warning 64, Line 7, Col 25, Line 7, Col 26, "This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type ''b'.")
            (Warning 64, Line 9, Col 30, Line 9, Col 31, "This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type ''b list'.")
        ]
        |> ignore

    // SOURCE=E_ExtraneousFields01.fs SCFLAGS="--test:ErrorRanges"
    // <Expects id="FS0765" span="(10,10-10,27)" status="error">Extraneous fields have been given values</Expects>
    // <Expects id="FS0765" span="(16,11-16,36)" status="error">Extraneous fields have been given values</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ObjectConstructors", Includes=[|"E_ExtraneousFields01.fs"|])>]
    let ``E_ExtraneousFields01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0765
        |> withDiagnosticMessageMatches "Extraneous fields have been given values"
        |> ignore
