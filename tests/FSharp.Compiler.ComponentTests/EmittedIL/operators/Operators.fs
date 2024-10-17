namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =

    // ``Validate that non generic (fast) code is emitted  for comparison involving decimals RealInternalSignatureOff`` compilation =
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"decimal_comparison_RealInternalSignatureOff.fs"|])>]
    let ``Non generic (fast) code emitted for decimal comparison RealInternalSignatureOff`` compilation =
        compilation
        |> asExe
        |> withRealInternalSignatureOff
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"decimal_comparison_RealInternalSignatureOn.fs"|])>]
    let ``Non generic (fast) code emitted for decimal comparison RealInternalSignatureOn`` compilation =
        compilation
        |> asExe
        |> withRealInternalSignatureOn
        |> ignoreWarnings
        |> verifyILBaseline
