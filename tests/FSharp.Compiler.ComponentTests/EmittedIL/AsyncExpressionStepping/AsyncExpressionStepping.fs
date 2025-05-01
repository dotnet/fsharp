namespace EmittedIL.RealInternalSignature

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

    [<Theory; FileInlineData("AsyncExpressionSteppingTest1.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest1_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("AsyncExpressionSteppingTest2.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest2`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("AsyncExpressionSteppingTest3.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest3_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("AsyncExpressionSteppingTest4.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest4_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("AsyncExpressionSteppingTest5.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest5_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("AsyncExpressionSteppingTest6.fs", Realsig=BooleanOptions.Both)>]
    let ``AsyncExpressionSteppingTest6_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
