// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TyperelatedExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"rigidtypeannotation01.fs"|])>]
    let ``rigidtypeannotation01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

