// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ImplementationFilesAndSignatureFiles.NamespacesFragmentsAndImplementationFiles

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects status="error" span="(9,1-9,1)" id="FS0010">Incomplete structured construct at or before this point in definition\. Expected '=' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_NamespaceAndModule01.fs"|])>]
    let ``basic - E_NamespaceAndModule01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Incomplete structured construct at or before this point in definition\. Expected '=' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0988" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"hashdirectives01.fs"|])>]
    let ``basic - hashdirectives01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0988" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"hashdirectives02.fs"|])>]
    let ``basic - hashdirectives02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"HashDirectives03.fs"|])>]
    let ``basic - HashDirectives03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0530" status="error" span="(7,1)">Only '#' compiler directives may occur prior to the first 'namespace' declaration</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_HashDirectives01.fs"|])>]
    let ``basic - E_HashDirectives01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0530
        |> withDiagnosticMessageMatches "Only '#' compiler directives may occur prior to the first 'namespace' declaration"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0530" status="error" span="(7,1)">Only '#' compiler directives may occur prior to the first 'namespace' declaration</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_HashDirectives02.fs"|])>]
    let ``basic - E_HashDirectives02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0530
        |> withDiagnosticMessageMatches "Only '#' compiler directives may occur prior to the first 'namespace' declaration"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0249" status="error" span="(13,6)">Two type definitions named 'DU' occur in namespace 'A\.B\.C' in two parts of this assembly</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_NamespaceCollision01.fs"|])>]
    let ``basic - E_NamespaceCollision01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0249
        |> withDiagnosticMessageMatches "Two type definitions named 'DU' occur in namespace 'A\.B\.C' in two parts of this assembly"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0037" status="error" span="(13,1)">Duplicate definition of type, exception or module 'B'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_ModuleCollision01.fs"|])>]
    let ``basic - E_ModuleCollision01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of type, exception or module 'B'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects id="FS0247" status="error" span="(14,13)">A namespace and a module named 'A\.B' both occur in two parts of this assembly</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"E_NamespaceModuleCollision01.fs"|])>]
    let ``basic - E_NamespaceModuleCollision01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0247
        |> withDiagnosticMessageMatches "A namespace and a module named 'A\.B' both occur in two parts of this assembly"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic)
    //<Expects status="notin">SomeNamespace.SomeModule</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ImplementationFilesAndSignatureFiles/NamespacesFragmentsAndImplementationFiles/basic", Includes=[|"NoWarningForOneCompiland.fs"|])>]
    let ``basic - NoWarningForOneCompiland.fs - `` compilation =
        compilation
        |> typecheck
        |> withDiagnosticMessageMatches "SomeNamespace.SomeModule"

