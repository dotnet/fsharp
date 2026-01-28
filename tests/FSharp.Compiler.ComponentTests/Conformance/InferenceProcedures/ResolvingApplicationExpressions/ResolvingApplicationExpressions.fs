// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ResolvingApplicationExpressions =

    // SOURCE="ComplexExpression01.fs"
    [<Theory; FileInlineData("ComplexExpression01.fs")>]
    let ``ComplexExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
