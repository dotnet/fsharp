// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module NonStringArgs =

    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#nowarn - errors`` (languageVersion) =

        FSharp """
#nowarn "988"
#nowarn FS
#nowarn FSBLAH
#nowarn ACME 
#nowarn "FS"
#nowarn "FSBLAH"
#nowarn "ACME"
        """
        |> withLangVersion languageVersion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            if languageVersion = "8.0" then
                (Error 3350, Line 3, Col 9, Line 3, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 4, Col 9, Line 4, Col 15, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 5, Col 9, Line 5, Col 13, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Warning 203, Line 6, Col 9, Line 6, Col 13, "Invalid warning number 'FS'");
                (Warning 203, Line 7, Col 9, Line 7, Col 17, "Invalid warning number 'FSBLAH'");
            else
                (Warning 203, Line 3, Col 9, Line 3, Col 11, "Invalid warning number 'FS'");
                (Warning 203, Line 4, Col 9, Line 4, Col 15, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 5, Col 9, Line 5, Col 13, "Invalid warning number 'ACME'");
                (Warning 203, Line 6, Col 9, Line 6, Col 13, "Invalid warning number 'FS'");
                (Warning 203, Line 7, Col 9, Line 7, Col 17, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 8, Col 9, Line 8, Col 15, "Invalid warning number 'ACME'")
            ]


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#nowarn - errors - collected`` (languageVersion) =

        FSharp """
#nowarn
    "988"
    FS
    FSBLAH
    ACME 
    "FS"
    "FSBLAH"
    "ACME"
        """
        |> withLangVersion languageVersion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            if languageVersion = "8.0" then
                (Error 3350, Line 4, Col 5, Line 4, Col 7, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 5, Col 5, Line 5, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 6, Col 5, Line 6, Col 9, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Warning 203, Line 7, Col 5, Line 7, Col 9, "Invalid warning number 'FS'");
                (Warning 203, Line 8, Col 5, Line 8, Col 13, "Invalid warning number 'FSBLAH'");
            else
                (Warning 203, Line 4, Col 5, Line 4, Col 7, "Invalid warning number 'FS'");
                (Warning 203, Line 5, Col 5, Line 5, Col 11, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 6, Col 5, Line 6, Col 9, "Invalid warning number 'ACME'");
                (Warning 203, Line 7, Col 5, Line 7, Col 9, "Invalid warning number 'FS'");
                (Warning 203, Line 8, Col 5, Line 8, Col 13, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 9, Col 5, Line 9, Col 11, "Invalid warning number 'ACME'")
            ]


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#nowarn - errors - inline`` (languageVersion) =

        FSharp """
#nowarn "988"
#nowarn FS FSBLAH ACME "FS" "FSBLAH" "ACME"
        """
        |> withLangVersion languageVersion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            if languageVersion = "8.0" then
                (Error 3350, Line 3, Col 9, Line 3, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 3, Col 12, Line 3, Col 18, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 3, Col 19, Line 3, Col 23, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Warning 203, Line 3, Col 24, Line 3, Col 28, "Invalid warning number 'FS'");
                (Warning 203, Line 3, Col 29, Line 3, Col 37, "Invalid warning number 'FSBLAH'");
            else
                (Warning 203, Line 3, Col 9, Line 3, Col 11, "Invalid warning number 'FS'");
                (Warning 203, Line 3, Col 12, Line 3, Col 18, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 3, Col 19, Line 3, Col 23, "Invalid warning number 'ACME'");
                (Warning 203, Line 3, Col 24, Line 3, Col 28, "Invalid warning number 'FS'");
                (Warning 203, Line 3, Col 29, Line 3, Col 37, "Invalid warning number 'FSBLAH'");
                (Warning 203, Line 3, Col 38, Line 3, Col 44, "Invalid warning number 'ACME'")
            ]


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#nowarn - realcode`` (langVersion) =

        let compileResult =
            FSharp """
#nowarn 20 FS1104 "3391" "FS3221"

module Exception =
    exception ``Crazy@name.p`` of string

module Decimal =
    type T1 = { a : decimal }
    module M0 =
        type T1 = { a : int;}
    let x = { a = 10 }              // error - 'expecting decimal' (which means we are not seeing M0.T1)

module MismatchedYields =
    let collection () = [
        yield "Hello"
        "And this"
        ]
module DoBinding =
    let square x = x * x
    square 32
            """
            |> withLangVersion langVersion
            |> asExe
            |> compile

        if langVersion = "8.0" then
            compileResult
            |> shouldFail
            |> withDiagnostics [
                (Warning 1104, Line 5, Col 15, Line 5, Col 31, "Identifiers containing '@' are reserved for use in F# code generation")
                (Error 3350, Line 2, Col 9, Line 2, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 2, Col 12, Line 2, Col 18, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Warning 203, Line 2, Col 26, Line 2, Col 34, "Invalid warning number 'FS3221'")
                ]
        else
            compileResult
            |> shouldSucceed


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#nowarn - errors - compiler options`` (languageVersion) =

        FSharp """
match None with None -> ()      // creates FS0025 - ignored due to flag
""                              // creates FS0020 - ignored due to flag
        """                     // creates FS0988 - not ignored, different flag prefix
        |> withLangVersion languageVersion
        |> withOptions ["--nowarn:NU0988"; "--nowarn:FS25"; "--nowarn:20"; "--nowarn:FS"; "--nowarn:FSBLAH"]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
                (Warning 988, Line 3, Col 3, Line 3, Col 3, "Main module of program is empty: nothing will happen when it is run")
            ]


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#time - mixed - Fsc`` (langversion) =

        Fsx """
#time on;;
#time off;;
#time blah;;
#time Ident;;
#time Long.Ident;;
#time 123;;
#time on off;;
#time;;
#time "on";;
#time "off";;
#time "blah";;
#time "on" "off";;

printfn "Hello, World"
        """
        |> withLangVersion langversion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            if langversion = "8.0" then
                (Error 3350, Line 2, Col 7, Line 2, Col 9, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 3, Col 7, Line 3, Col 10, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 4, Col 7, Line 4, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 5, Col 7, Line 5, Col 12, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 6, Col 7, Line 6, Col 17, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 7, Col 7, Line 7, Col 10, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 8, Col 7, Line 8, Col 9, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 8, Col 10, Line 8, Col 13, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 235, Line 12, Col 1, Line 12, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 13, Col 1, Line 13, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
            else
                (Error 235, Line 4, Col 1, Line 4, Col 11, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 5, Col 1, Line 5, Col 12, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 6, Col 1, Line 6, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 7, Col 1, Line 7, Col 10, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 8, Col 1, Line 8, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 12, Col 1, Line 12, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 13, Col 1, Line 13, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
            ]


    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``#time - mixed - Fsx`` (langversion) =

        Fsx """
#time on;;
#time off;;
#time blah;;
#time Ident;;
#time Long.Ident;;
#time 123;;
#time on off;;
#time;;
#time "on";;
#time "off";;
#time "blah";;
#time "on" "off";;

printfn "Hello, World"
        """
        |> withLangVersion langversion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            if langversion = "8.0" then
                (Error 3350, Line 2, Col 7, Line 2, Col 9, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 3, Col 7, Line 3, Col 10, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 4, Col 7, Line 4, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 5, Col 7, Line 5, Col 12, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 6, Col 7, Line 6, Col 17, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 7, Col 7, Line 7, Col 10, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 8, Col 7, Line 8, Col 9, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 3350, Line 8, Col 10, Line 8, Col 13, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 9.0 or greater.")
                (Error 235, Line 12, Col 1, Line 12, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 13, Col 1, Line 13, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
            else
                (Error 235, Line 4, Col 1, Line 4, Col 11, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 5, Col 1, Line 5, Col 12, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 6, Col 1, Line 6, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 7, Col 1, Line 7, Col 10, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 8, Col 1, Line 8, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 12, Col 1, Line 12, Col 13, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
                (Error 235, Line 13, Col 1, Line 13, Col 17, """Invalid directive. Expected '#time', '#time "on"' or '#time "off"'.""")
            ]


    [<InlineData("8.0")>]
    [<InlineData("preview")>]
    [<Theory>]
    let ``#r errors - Fsc`` (langVersion) =

        FSharp """
        #r;;
        #r "";;
        #r Ident;;
        #r Long.Ident;;
        #r 123;;

        printfn "Hello, World"
        """
        |> withLangVersion langVersion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 76, Line 2, Col 9, Line 2, Col 11, "This directive may only be used in F# script files (extensions .fsx or .fsscript). Either remove the directive, move this code to a script file or delimit the directive with '#if INTERACTIVE'/'#endif'.")
            (Error 76, Line 3, Col 9, Line 3, Col 14, "This directive may only be used in F# script files (extensions .fsx or .fsscript). Either remove the directive, move this code to a script file or delimit the directive with '#if INTERACTIVE'/'#endif'.")
            (Error 76, Line 4, Col 9, Line 4, Col 17, "This directive may only be used in F# script files (extensions .fsx or .fsscript). Either remove the directive, move this code to a script file or delimit the directive with '#if INTERACTIVE'/'#endif'.")
            (Error 76, Line 5, Col 9, Line 5, Col 22, "This directive may only be used in F# script files (extensions .fsx or .fsscript). Either remove the directive, move this code to a script file or delimit the directive with '#if INTERACTIVE'/'#endif'.")
            (Error 76, Line 6, Col 9, Line 6, Col 15, "This directive may only be used in F# script files (extensions .fsx or .fsscript). Either remove the directive, move this code to a script file or delimit the directive with '#if INTERACTIVE'/'#endif'.")
            ]


    [<InlineData("8.0")>]
    [<InlineData("preview")>]
    [<Theory>]
    let ``#r errors - Fsi`` (langVersion) =

        Fsx """
        #r;;
        #r "";;
        #r Ident;;
        #r Long.Ident;;
        #r 123;;

        printfn "Hello, World"
        """
        |> withLangVersion langVersion
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3353, Line 2, Col 9, Line 2, Col 11, "Invalid directive '#r '")
            (Warning 213, Line 3, Col 9, Line 3, Col 14, "'' is not a valid assembly name")
            (Error 3869, Line 4, Col 12, Line 4, Col 17, "Unexpected identifier 'Ident'.")
            (Error 3869, Line 5, Col 12, Line 5, Col 22, "Unexpected identifier 'Long.Ident'.")
            (Error 3869, Line 6, Col 12, Line 6, Col 15, "Unexpected integer literal '123'.")
        ]
