// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ImplicitObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors)
    //<Expects id="FS0044" span="(9,9-9,15)" status="warning">Message1</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors", Includes=[|"WithAttribute.fs"|])>]
    let ``ImplicitObjectConstructors - WithAttribute.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message1"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors)
    //<Expects id="FS0762" status="error" span="(8,13)">Constructors for the type 'Foo' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors", Includes=[|"E_AddExplicitWithImplicit.fs"|])>]
    let ``ImplicitObjectConstructors - E_AddExplicitWithImplicit.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0762
        |> withDiagnosticMessageMatches "Constructors for the type 'Foo' must directly or indirectly call its implicit object constructor\. Use a call to the implicit object constructor instead of a record expression"

