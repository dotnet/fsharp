namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module Ifdef =

    let ifdefSource = """
[<EntryPoint>]
let main _ =
    #if MYDEFINE1
    printf "1"
    #else
    printf "2"
    #endif
    0
"""

    [<InlineData("MYDEFINE1", "1")>]
    [<InlineData("MYDEFINE", "2")>]
    [<Theory>]
    let ifdefTest (mydefine, expectedOutput) =

        FSharp ifdefSource
        |> withDefines [mydefine]
        |> compileExeAndRun
        |> verifyOutput expectedOutput
