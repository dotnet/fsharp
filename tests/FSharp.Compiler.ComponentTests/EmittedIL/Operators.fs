
namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module Operators =

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/codegen/operators")>]
    let ``Validate that non generic (fast) code is emmitted  for comparison involving decimals`` compilation =
        compilation
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline