// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeConstraints =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeConstraints)
    //<Expects status="error" span="(9,25)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeConstraints", Includes=[|"E_ConstructorConstraint01.fs"|])>]
    let ``TypeConstraints - E_ConstructorConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3066
        |> withDiagnosticMessageMatches "Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$"

