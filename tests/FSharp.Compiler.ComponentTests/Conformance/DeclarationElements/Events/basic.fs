// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.Events

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"SanityCheck.fs"|])>]
    let ``basic - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"SanityCheck02.fs"|])>]
    let ``basic - SanityCheck02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"Regression01.fs"|])>]
    let ``basic - Regression01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"Regression02.fs"|])>]
    let ``basic - Regression02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"Regression02b.fs"|])>]
    let ``basic - Regression02b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"EventsOnInterface01.fs"|])>]
    let ``basic - EventsOnInterface01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/Events/basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/Events/basic", Includes=[|"EventWithGenericTypeAsUnit01.fs"|])>]
    let ``basic - EventWithGenericTypeAsUnit01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

