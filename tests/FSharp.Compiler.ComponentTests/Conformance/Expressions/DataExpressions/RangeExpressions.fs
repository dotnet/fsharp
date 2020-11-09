// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module RangeExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/RangeExpressions)
    //<Expects status="notin">Floating point ranges are experimental and may be deprecated in a future release</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/RangeExpressions", Includes=[|"FloatingPointRangeExp01.fs"|])>]
    let ``RangeExpressions - FloatingPointRangeExp01.fs - `` compilation =
        compilation
        |> typecheck
        |> withDiagnosticMessageMatches "Floating point ranges are experimental and may be deprecated in a future release"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/RangeExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/RangeExpressions", Includes=[|"CustomType01.fs"|])>]
    let ``RangeExpressions - CustomType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

