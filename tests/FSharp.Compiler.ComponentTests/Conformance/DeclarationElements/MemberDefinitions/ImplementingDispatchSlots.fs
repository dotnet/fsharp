// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ImplementingDispatchSlots =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/MemberDefinitions/ImplementingDispatchSlots)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/MemberDefinitions/ImplementingDispatchSlots", Includes=[|"SanityCheck.fs"|])>]
    let ``ImplementingDispatchSlots - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

