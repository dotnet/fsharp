// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Miscellaneous

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OptimizedForLoops =

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"VariableStepForLoops.fs"|])>]
    let ``Optimized variable step for loops should match OperatorIntrinsics RangeInt32 behavior`` compilation =
        compilation
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ConstStepForLoops.fs"|])>]
    let ``Optimized constant step loops should match OperatorIntrinsics RangeInt32 behavior`` compilation =
        compilation
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NearMaxForLoops.fs"|])>]
    let ``Optimized loops with values near Int32 MaxValue should match OperatorIntrinsics RangeInt32 behavior`` compilation =
        compilation
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForLoopsInTasks.fs"|])>]
    let ``Optimized loops should work with resumable code`` compilation =
        compilation
        |> compileExeAndRun
        |> shouldSucceed