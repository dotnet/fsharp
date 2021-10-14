// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module TypeChecker =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"GenericSubType01.fs"|])>]
    let ``TypeChecker - GenericSubType01.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

