// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Printing

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Printing =

    // This test was automatically generated (moved from FSharpQA suite - Printing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Printing", Includes=[|"CustomExceptions01.fs"|])>]
    let ``Printing - CustomExceptions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Printing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Printing", Includes=[|"WidthForAFormatter.fs"|])>]
    let ``Printing - WidthForAFormatter.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Printing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Printing", Includes=[|"ToStringOnCollections.fs"|])>]
    let ``Printing - ToStringOnCollections.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

