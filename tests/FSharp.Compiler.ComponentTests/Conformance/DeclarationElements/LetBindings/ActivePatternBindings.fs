// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ActivePatternBindings =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/ActivePatternBindings)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/ActivePatternBindings", Includes=[|"SanityCheck.fs"|])>]
    let ``ActivePatternBindings - SanityCheck.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/ActivePatternBindings)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/ActivePatternBindings", Includes=[|"parameterizedActivePattern.fs"|])>]
    let ``ActivePatternBindings - parameterizedActivePattern.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/ActivePatternBindings)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/ActivePatternBindings", Includes=[|"partialActivePattern.fs"|])>]
    let ``ActivePatternBindings - partialActivePattern.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

