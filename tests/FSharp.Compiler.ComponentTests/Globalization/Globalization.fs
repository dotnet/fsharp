// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Globalization

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Globalization =

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Arabic.fs"|])>]
    let ``Globalization - Arabic.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Hindi.fs"|])>]
    let ``Globalization - Hindi.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"gb18030.fs"|])>]
    let ``Globalization - gb18030.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Mongolian.fs"|])>]
    let ``Globalization - Mongolian.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"RightToLeft.fs"|])>]
    let ``Globalization - RightToLeft.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Surrogates.fs"|])>]
    let ``Globalization - Surrogates.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Tamil.fs"|])>]
    let ``Globalization - Tamil.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Tibetan.fs"|])>]
    let ``Globalization - Tibetan.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"TurkishI.fs"|])>]
    let ``Globalization - TurkishI.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Uighur.fs"|])>]
    let ``Globalization - Uighur.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"utf8.fs"|])>]
    let ``Globalization - utf8.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"utf16.fs"|])>]
    let ``Globalization - utf16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"Yi.fs"|])>]
    let ``Globalization - Yi.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Globalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Globalization", Includes=[|"HexCharEncode.fs"|])>]
    let ``Globalization - HexCharEncode.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

