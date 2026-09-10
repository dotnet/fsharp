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
#:ignore also at eof"""

    [<Fact>]
    let ignoreColonDirective () =

        FSharp source
        |> compile
        |> withDiagnostics [
            Error 3909, Line 6, Col 5, Line 6, Col 21, "#: directives must start at the beginning of a line"
        ]