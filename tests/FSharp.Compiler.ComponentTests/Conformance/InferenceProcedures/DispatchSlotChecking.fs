// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module DispatchSlotChecking =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/DispatchSlotChecking)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/DispatchSlotChecking", Includes=[|"InferSlotType01.fs"|])>]
    let ``DispatchSlotChecking - InferSlotType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

