namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/CodeGen/operators")>]
    let ``Validate that non generic (fast) code is emmitted  for comparison involving decimals`` compilation =
        compilation
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline