// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module NoWarnTest =

    // #nowarn is super forgiving the only real error is FS alerts you that you forgot the error ID
    [<Fact>]
    let ``NoWarn Errors F# 8`` () =

        FSharp """
#nowarn "988"
#nowarn FS
#nowarn FSBLAH
#nowarn ACME 
#nowarn "FS"
#nowarn "FSBLAH"
#nowarn "ACME"
        """
        |> withLangVersion80
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics[
            (Error 3350, Line 3, Col 9, Line 3, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 4, Col 9, Line 4, Col 15, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 5, Col 9, Line 5, Col 13, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Warning 203, Line 6, Col 1, Line 6, Col 13, "Invalid warning number 'FS'")
            ]


    [<Fact>]
    let ``NoWarn Errors F# 9`` () =

        FSharp """
#nowarn FS988
#nowarn FS
#nowarn FSBLAH
#nowarn ACME 
#nowarn "FS"
#nowarn "FSBLAH"
#nowarn "ACME"
        """
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 203, Line 3, Col 1, Line 3, Col 11, "Invalid warning number 'FS'")
            (Warning 203, Line 6, Col 1, Line 6, Col 13, "Invalid warning number 'FS'")
            ]


    [<Fact>]
    let ``NoWarn Errors collection F# 8`` () =

        FSharp """
#nowarn "988"
#nowarn FS FSBLAH ACME "FS" "FSBLAH" "ACME"
        """
        |> withLangVersion80
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 9, Line 3, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 3, Col 12, Line 3, Col 18, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 3, Col 19, Line 3, Col 23, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Warning 203, Line 3, Col 1, Line 3, Col 44, "Invalid warning number 'FS'")
            ]

    [<Fact>]
    let ``NoWarn Errors collection F# 9`` () =

        FSharp """
#nowarn "988"
#nowarn FS FSBLAH ACME "FS" "FSBLAH" "ACME"
        """
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 203, Line 3, Col 1, Line 3, Col 44, "Invalid warning number 'FS'")
            ]

    [<Fact>]
    let ``Mixed Warnings F# 8`` () =

        FSharp """
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
        |> withLangVersion80
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1104, Line 3, Col 15, Line 3, Col 31, "Identifiers containing '@' are reserved for use in F# code generation")
            (Warning 3391, Line 9, Col 19, Line 9, Col 21, """This expression uses the implicit conversion 'System.Decimal.op_Implicit(value: int) : decimal' to convert type 'int' to type 'decimal'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".""")
            (Warning 3221, Line 14, Col 9, Line 14, Col 19, "This expression returns a value of type 'string' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'.")
            (Warning 20, Line 18, Col 5, Line 18, Col 14, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            ]


    [<Fact>]
    let ``Mixed Warnings F# 9`` () =

        FSharp """
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
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1104, Line 3, Col 15, Line 3, Col 31, "Identifiers containing '@' are reserved for use in F# code generation")
            (Warning 3391, Line 9, Col 19, Line 9, Col 21, """This expression uses the implicit conversion 'System.Decimal.op_Implicit(value: int) : decimal' to convert type 'int' to type 'decimal'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391".""")
            (Warning 3221, Line 14, Col 9, Line 14, Col 19, "This expression returns a value of type 'string' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'.")
            (Warning 20, Line 18, Col 5, Line 18, Col 14, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            ]


    [<Fact>]
    let ``Mixed Nowarns F# 8`` () =

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
        |> withLangVersion80
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 2, Col 9, Line 2, Col 11, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 2, Col 12, Line 2, Col 18, "Feature '# directives with non-quoted string arguments' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
            (Warning 1104, Line 5, Col 15, Line 5, Col 31, "Identifiers containing '@' are reserved for use in F# code generation")
            ]


    [<Fact>]
    let ``Mixed Nowarns F# 9`` () =

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
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldSucceed
