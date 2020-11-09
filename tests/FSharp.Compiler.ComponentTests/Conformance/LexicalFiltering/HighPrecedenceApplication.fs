// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module HighPrecedenceApplication =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HighPrecedenceApplication)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HighPrecedenceApplication", Includes=[|"BasicCheck01.fs"|])>]
    let ``HighPrecedenceApplication - BasicCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HighPrecedenceApplication)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HighPrecedenceApplication", Includes=[|"BasicCheck02.fs"|])>]
    let ``HighPrecedenceApplication - BasicCheck02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HighPrecedenceApplication)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HighPrecedenceApplication", Includes=[|"RangeOperator01.fs"|])>]
    let ``HighPrecedenceApplication - RangeOperator01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

