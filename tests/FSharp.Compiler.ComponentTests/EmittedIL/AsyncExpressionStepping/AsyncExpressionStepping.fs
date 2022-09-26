namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AsyncExpressionStepping =

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest1.fs"|])>]
    let ``AsyncExpressionSteppingTest1_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest2.fs"|])>]
    let ``AsyncExpressionSteppingTest2_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest3.fs"|])>]
    let ``AsyncExpressionSteppingTest3_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest4.fs"|])>]
    let ``AsyncExpressionSteppingTest4_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest5.fs"|])>]
    let ``AsyncExpressionSteppingTest5_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AsyncExpressionSteppingTest6.fs"|])>]
    let ``AsyncExpressionSteppingTest6_fs`` compilation =
        verifyCompilation compilation


