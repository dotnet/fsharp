namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =

    // ``Validate that non generic (fast) code is emitted  for comparison involving decimals RealInternalSignatureOff`` compilation =
    [<Theory; FileInlineData("decimal_comparison.fs", Realsig=BooleanOptions.Both)>]
    let ``Non generic (fast) code emitted for decimal comparison`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> verifyILBaseline
