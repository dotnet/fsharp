// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AsDeclarations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AsDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AsDeclarations", Includes=[|"SanityCheck01.fs"|])>]
    let ``AsDeclarations - SanityCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AsDeclarations)
    //<Expects id="FS0010" status="error">Unexpected keyword 'as' in type definition\. Expected '\(' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/AsDeclarations", Includes=[|"SanityCheck02.fs"|])>]
    let ``AsDeclarations - SanityCheck02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'as' in type definition\. Expected '\(' or other token"

