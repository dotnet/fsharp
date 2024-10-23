namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module Ifdef =

    let ifdefSource = """
[<EntryPoint>]
let main _ =
    #if MYDEFINE1
    1
    #else
    2
    #endif
"""

    [<InlineData("MYDEFINE1", 1)>]
    [<InlineData("MYDEFINE", 2)>]
    [<Theory>]
    let ifdefTest (mydefine, expectedExitCode) =

        FSharp ifdefSource
        |> withDefines [mydefine]
        |> compileExeAndRun
        |> withExitCode expectedExitCode


    let sourceExtraEndif = """
#if MYDEFINE1
printf "1"
#endif
(**)#endif(**)
0
"""

    [<Fact>]
    let extraEndif () =

        FSharp sourceExtraEndif
        |> withDefines ["MYDEFINE1"]
        |> asExe
        |> compile
        |> withDiagnosticMessage "#endif has no matching #if in implementation file"

    let sourceUnknownHash = """
module A
#ifxx
#abc
"""

    [<Fact>]
    let unknownHashDirectiveIsIgnored () =

        FSharp sourceUnknownHash
        |> compile
        |> shouldSucceed