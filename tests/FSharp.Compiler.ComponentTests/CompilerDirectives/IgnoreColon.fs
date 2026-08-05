namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module IgnoreColon =

    let source = """
module test
#:r test.dll
[<EntryPoint>]
let main _ =
    #:source test.fs
    0
"""

    [<Fact>]
    let ignoreColonDirective () =

        FSharp source
        |> compile
        |> withDiagnostics [
            Error 3908, Line 6, Col 5, Line 6, Col 22, "#: directives must appear as the first non-whitespace characters on a line"
        ]
