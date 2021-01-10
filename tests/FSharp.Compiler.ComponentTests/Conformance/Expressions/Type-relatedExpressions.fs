// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TyperelatedExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"rigidtypeannotation01.fs"|])>]
    let ``TyperelatedExpressions - rigidtypeannotation01.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

