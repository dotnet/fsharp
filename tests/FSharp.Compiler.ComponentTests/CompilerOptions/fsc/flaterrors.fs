// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions

open Xunit
open System
open FSharp.Test.Compiler

//# Sanity check - simply check that the option is valid
module flaterrors =

    //# Functional: the option does what it is meant to do
    let compile (options: string) compilation  =
        let options =
            if String.IsNullOrEmpty options then [||]
            else options.Split([|';'|]) |> Array.map(fun s -> s.Trim())
        compilation
        |> asExe
        |> withOptions (options |> Array.toList)
        |> compile

    [<InlineData("")>]                                  // default -off-
    [<Theory>]
    let ``E_MultiLine01_fs`` (options: string) =
        Fs """List.rev {1..10}"""
        |> compile options
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\n    ''a list'    \nbut here has type\n    ''b seq'    ")
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\n    ''a list'    \nbut here has type\n    'int seq'    ")
            (Warning 20, Line 1, Col 1, Line 1, Col 17, "The result of this expression has type ''a list' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    [<InlineData("--flaterrors")>]                      //once
    [<InlineData("--flaterrors;--flaterrors")>]         //twice
    [<InlineData("--nologo;--flaterrors")>]             // with nologo
    [<Theory>]
    let ``E_MultiLine02_fs`` (options: string) =
        Fs """List.rev {1..10} |> ignore"""
        |> compile options
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\029    ''a list'    \029but here has type\029    ''b seq'")
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\029    ''a list'    \029but here has type\029    'int seq'")
        ]

    [<InlineData("--flaterrors")>]                          //once
    [<InlineData("--flaterrors;--flaterrors")>]             //twice
    [<InlineData("--nologo;--flaterrors")>]                 // with nologo
    [<InlineData("--out:E_MultiLine03.exe;--flaterrors")>]  // with out
    [<Theory>]
    let ``E_MultiLine03_fs`` (options: string) =
        Fs """let a = b"""
        |> compile options
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 1, Col 9, Line 1, Col 10, """The value or constructor 'b' is not defined.""")
        ]

    [<InlineData("--FlatErrors")>]                          //Invalid case
    [<InlineData("--FLATERRORS")>]                          //Even more invalid case
    [<InlineData("--flaterrors-")>]                         // no + allowed
    [<InlineData("--flaterrors-")>]                         // no - allowed
    [<Theory>]
    let ``E_MultiLine04_fs`` (option: string) =
        Fs """List.rev {1..10} |> ignore"""
        |> compile option
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, $"Unrecognized option: '{option}'. Use '--help' to learn about recognized command line options.")
        ]
