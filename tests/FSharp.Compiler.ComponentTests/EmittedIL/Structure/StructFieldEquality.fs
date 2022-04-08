namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"decimal_comparison.fs"|])>]
    let ``Validate that non generic (fast) code is emitted  for comparison involving decimals`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> verifyILBaseline