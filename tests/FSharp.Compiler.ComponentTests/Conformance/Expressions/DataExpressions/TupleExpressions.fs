// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TupleExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/TupleExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/TupleExpressions", Includes=[|"EqualityDifferentRuntimeType01.fs"|])>]
    let ``TupleExpressions - EqualityDifferentRuntimeType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/TupleExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/TupleExpressions", Includes=[|"Tuples01.fs"|])>]
    let ``TupleExpressions - Tuples01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

