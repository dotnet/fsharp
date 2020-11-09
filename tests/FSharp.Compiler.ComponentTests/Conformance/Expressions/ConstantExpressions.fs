// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ConstantExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"bigint.fs"|])>]
    let ``ConstantExpressions - bigint.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"bigint02.fs"|])>]
    let ``ConstantExpressions - bigint02.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"bool.fs"|])>]
    let ``ConstantExpressions - bool.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"byte.fs"|])>]
    let ``ConstantExpressions - byte.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"byteArr.fs"|])>]
    let ``ConstantExpressions - byteArr.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"char.fs"|])>]
    let ``ConstantExpressions - char.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"double.fs"|])>]
    let ``ConstantExpressions - double.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"float.fs"|])>]
    let ``ConstantExpressions - float.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"float32.fs"|])>]
    let ``ConstantExpressions - float32.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"int.fs"|])>]
    let ``ConstantExpressions - int.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"int16.fs"|])>]
    let ``ConstantExpressions - int16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"int32.fs"|])>]
    let ``ConstantExpressions - int32.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"int64.fs"|])>]
    let ``ConstantExpressions - int64.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"nativenint.fs"|])>]
    let ``ConstantExpressions - nativenint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"sbyte.fs"|])>]
    let ``ConstantExpressions - sbyte.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"single.fs"|])>]
    let ``ConstantExpressions - single.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"string.fs"|])>]
    let ``ConstantExpressions - string.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"uint16.fs"|])>]
    let ``ConstantExpressions - uint16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"uint32.fs"|])>]
    let ``ConstantExpressions - uint32.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"uint64.fs"|])>]
    let ``ConstantExpressions - uint64.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"unativenint.fs"|])>]
    let ``ConstantExpressions - unativenint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ConstantExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/ConstantExpressions", Includes=[|"unit.fs"|])>]
    let ``ConstantExpressions - unit.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

