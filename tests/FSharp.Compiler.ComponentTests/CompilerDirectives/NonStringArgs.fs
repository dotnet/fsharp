// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module NonStringArgs =


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
