// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ImportDeclarations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"openSystem01.fs"|])>]
    let ``ImportDeclarations - openSystem01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"OpenNestedModule01.fs"|])>]
    let ``ImportDeclarations - OpenNestedModule01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"openDU.fs"|])>]
    let ``ImportDeclarations - openDU.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    //<Expects status="error" span="(12,6-12,39)" id="FS0892">This declaration opens the module 'Microsoft\.FSharp\.Collections\.List', which is marked as 'RequireQualifiedAccess'\. Adjust your code to use qualified references to the elements of the module instead, e\.g\. 'List\.map' instead of 'map'\. This change will ensure that your code is robust as new constructs are added to libraries\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"E_OpenTwice.fs"|])>]
    let ``ImportDeclarations - E_OpenTwice.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0892
        |> withDiagnosticMessageMatches "This declaration opens the module 'Microsoft\.FSharp\.Collections\.List', which is marked as 'RequireQualifiedAccess'\. Adjust your code to use qualified references to the elements of the module instead, e\.g\. 'List\.map' instead of 'map'\. This change will ensure that your code is robust as new constructs are added to libraries\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    //<Expects status="error" span="(34,9-34,10)" id="FS0039">The value or constructor 'C' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"E_openEnum.fs"|])>]
    let ``ImportDeclarations - E_openEnum.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'C' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    //<Expects status="error" span="(7,5-7,9)" id="FS0010">Unexpected keyword 'open' in member definition$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"E_openInTypeDecl.fs"|])>]
    let ``ImportDeclarations - E_openInTypeDecl.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'open' in member definition$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    //<Expects status="error" span="(23,9-23,13)" id="FS0010">Unexpected keyword 'open' in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"E_openModInFun.fs"|])>]
    let ``ImportDeclarations - E_openModInFun.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'open' in expression$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/ImportDeclarations)
    //<Expects id="FS0039" status="error">The namespace or module 'SomeUnknownNamespace' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/ImportDeclarations", Includes=[|"E_OpenUnknownNS.fs"|])>]
    let ``ImportDeclarations - E_OpenUnknownNS.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The namespace or module 'SomeUnknownNamespace' is not defined"

