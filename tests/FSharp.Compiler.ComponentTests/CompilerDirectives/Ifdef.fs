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

    // #elif tests
    let elifSource1 = """
[<EntryPoint>]
let main _ =
    #if DEFINE1
    1
    #elif DEFINE2
    2
    #elif DEFINE3
    3
    #else
    4
    #endif
"""

    [<InlineData("DEFINE1", 1)>]
    [<InlineData("DEFINE2", 2)>]
    [<InlineData("DEFINE3", 3)>]
    [<InlineData("NONE", 4)>]
    [<Theory>]
    let elifBasicTest (mydefine, expectedExitCode) =
        let defines = if mydefine = "NONE" then [] else [mydefine]
        FSharp elifSource1
        |> withDefines defines
        |> compileExeAndRun
        |> withExitCode expectedExitCode

    let elifSource2 = """
[<EntryPoint>]
let main _ =
    #if DEFINE1
    1
    #elif DEFINE2 && DEFINE3
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifWithComplexExpression () =
        FSharp elifSource2
        |> withDefines ["DEFINE2"; "DEFINE3"]
        |> compileExeAndRun
        |> withExitCode 2

    [<Fact>]
    let elifWithComplexExpressionFalse () =
        FSharp elifSource2
        |> withDefines ["DEFINE2"]
        |> compileExeAndRun
        |> withExitCode 3

    let elifNestedSource = """
[<EntryPoint>]
let main _ =
    #if OUTER
        #if INNER
        1
        #elif INNER2
        2
        #else
        3
        #endif
    #elif OUTER2
    4
    #else
    5
    #endif
"""

    [<Fact>]
    let elifNested () =
        FSharp elifNestedSource
        |> withDefines ["OUTER"; "INNER2"]
        |> compileExeAndRun
        |> withExitCode 2

    [<Fact>]
    let elifNestedOuter () =
        FSharp elifNestedSource
        |> withDefines ["OUTER2"]
        |> compileExeAndRun
        |> withExitCode 4

    let elifMultipleSource = """
[<EntryPoint>]
let main _ =
    #if DEFINE1
    1
    #elif DEFINE2
    2
    #elif DEFINE3
    3
    #elif DEFINE4
    4
    #elif DEFINE5
    5
    #else
    6
    #endif
"""

    [<Fact>]
    let elifMultiple () =
        FSharp elifMultipleSource
        |> withDefines ["DEFINE4"]
        |> compileExeAndRun
        |> withExitCode 4

    let elifAfterElseSource = """
#if DEFINE1
let x = 1
#else
let x = 2
#elif DEFINE2
let x = 3
#endif
"""

    [<Fact>]
    let elifAfterElseError () =
        FSharp elifAfterElseSource
        |> withDefines ["DEFINE2"]
        |> asExe
        |> compile
        |> withDiagnosticMessage "#endif required for #else"

    let elifNoMatchingIfSource = """
#elif DEFINE1
let x = 1
#endif
"""

    [<Fact>]
    let elifNoMatchingIf () =
        FSharp elifNoMatchingIfSource
        |> asExe
        |> compile
        |> withDiagnosticMessage "#else has no matching #if in implementation file"

    let elifWithOrExpression = """
[<EntryPoint>]
let main _ =
    #if DEFINE1
    1
    #elif DEFINE2 || DEFINE3
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifWithOrTrue () =
        FSharp elifWithOrExpression
        |> withDefines ["DEFINE3"]
        |> compileExeAndRun
        |> withExitCode 2

    let elifWithNotExpression = """
[<EntryPoint>]
let main _ =
    #if DEFINE1
    1
    #elif !DEFINE2
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifWithNotTrue () =
        FSharp elifWithNotExpression
        |> compileExeAndRun
        |> withExitCode 2

    [<Fact>]
    let elifWithNotFalse () =
        FSharp elifWithNotExpression
        |> withDefines ["DEFINE2"]
        |> compileExeAndRun
        |> withExitCode 3