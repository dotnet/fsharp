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

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest1.fs"|])>]
    let ``AsyncExpressionSteppingTest1_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest1.fs"|])>]
    let ``AsyncExpressionSteppingTest1_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest2.fs"|])>]
    let ``AsyncExpressionSteppingTest2_RealInternalSignatureOnfs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest2.fs"|])>]
    let ``AsyncExpressionSteppingTest2_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest3.fs"|])>]
    let ``AsyncExpressionSteppingTest3_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest3.fs"|])>]
    let ``AsyncExpressionSteppingTest3_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest4.fs"|])>]
    let ``AsyncExpressionSteppingTest4_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest4.fs"|])>]
    let ``AsyncExpressionSteppingTest4_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest5.fs"|])>]
    let ``AsyncExpressionSteppingTest5_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest5.fs"|])>]
    let ``AsyncExpressionSteppingTest5_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AsyncExpressionSteppingTest6.fs"|])>]
    let ``AsyncExpressionSteppingTest6_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AsyncExpressionSteppingTest6.fs"|])>]
    let ``AsyncExpressionSteppingTest6_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation
