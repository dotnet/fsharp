// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeChecker =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"GenericSubType01.fs"|])>]
    let ``TypeChecker - GenericSubType01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

