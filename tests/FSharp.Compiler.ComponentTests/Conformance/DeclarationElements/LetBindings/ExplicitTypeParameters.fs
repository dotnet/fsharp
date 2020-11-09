// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ExplicitTypeParameters =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/ExplicitTypeParameters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/ExplicitTypeParameters", Includes=[|"SanityCheck.fs"|])>]
    let ``ExplicitTypeParameters - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/ExplicitTypeParameters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/ExplicitTypeParameters", Includes=[|"SanityCheck2.fs"|])>]
    let ``ExplicitTypeParameters - SanityCheck2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

