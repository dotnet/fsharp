// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module Nowarn =

    let warn20Text = "The result of this expression has type 'string' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."

    [<InlineData(8.0)>]
    [<InlineData(9.0)>]
    [<Theory>]
    let ``line after nowarn - should warn`` (langVersion) =

        FSharp """
module A
#nowarn "20"
#line 1 "xyz.fs"
""                                      // Warning disabled by # nowarn
        """
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
                (Warning 20, Line 1, Col 1, Line 1, Col 3, warn20Text)
            ]

    [<InlineData(8.0)>]
    [<InlineData(9.0)>]
    [<Theory>]
    let ``line before nowarn - should succeed`` (langVersion) =

        FSharp """
module A
#line 1 "xyz.fs"
#nowarn "20"
""                                      // Warning disabled by # nowarn
        """
        |> withLangVersion langVersion
        |> compile
        |> shouldSucceed


    [<InlineData(8.0)>]
    [<InlineData(9.0)>]
    [<Theory>]
    let ``NoWarn compat nowarn without line`` (langVersion) =

        FSharp """
module A
""                                      // Should produce an error
# nowarn "20"
""                                      // Warning disabled by # nowarn
        """
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withDiagnostics [
                (Warning 20, Line 3, Col 1, Line 3, Col 3, warn20Text)
            ]
