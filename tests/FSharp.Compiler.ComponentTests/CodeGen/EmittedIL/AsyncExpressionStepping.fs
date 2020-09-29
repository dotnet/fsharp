// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CodeGen.EmittedIL

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AsyncExpressionStepping =
    
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/codegen/EmittedIL/AsyncExpressionStepping")>]
    let ``Verify Async Computational Expressoin Stepping`` compilation = // Regression tests for FSHARP1.0:4058 
        compilation
        |> ignoreWarnings
        |> withOptions ["-a"; "-g"; "--test:EmitFeeFeeAs100001"; "--optimize-"]
        |> verifyBaseline
        |> verifyILBaseline
