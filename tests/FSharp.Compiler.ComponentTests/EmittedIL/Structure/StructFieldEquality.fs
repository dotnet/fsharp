namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =

    [<Theory; FileInlineData("decimal_comparison.fs")>]
    let ``Validate that non generic (fast) code is emitted  for comparison involving decimals`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> verifyILBaseline