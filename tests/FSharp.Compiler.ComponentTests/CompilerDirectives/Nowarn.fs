// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module Nowarn =

    let warn20Text = "The result of this expression has type 'string' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."

    let checkFileBugSource = """
module A
#nowarn "20"
#line 1 "xyz.fs"
""
        """

    let checkFileBugSource2 = """
module A
#line 1 "xyz.fs"
#nowarn "20"
""
        """


    [<Fact>]
    let ``checkFile bug simulation for compatibility`` () =

        FSharp checkFileBugSource
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``checkFile bug fixed leads to new warning`` () =

        FSharp checkFileBugSource
        |> withLangVersion90
        |> compile
        |> shouldFail
        |> withDiagnostics [
                (Warning 20, Line 1, Col 1, Line 1, Col 3, warn20Text)
            ]

    [<Fact>]
    let ``checkFile bug fixed, no warning if nowarn is correctly used`` () =

        FSharp checkFileBugSource2
        |> withLangVersion90
        |> compile
        |> shouldSucceed
