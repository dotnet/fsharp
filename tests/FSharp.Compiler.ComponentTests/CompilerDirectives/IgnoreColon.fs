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
        |> withDiagnosticMessage "#: directives must appear as the first non-whitespace characters on a line"