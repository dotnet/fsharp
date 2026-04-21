// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Else branch is missing`` =

    [<Fact>]
    let ``Fail if else branch is missing``() =
        FSharp """
let x = 10
let y =
   if x > 10 then "test"
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 19, Line 4, Col 25,
                                 "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'.")

    [<Fact>]
    let ``Fail on type error in condition``() =
        FSharp """
let x = 10
let y =
   if x > 10 then
     if x <> "test" then printfn "test"
     ()
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 5, Col 14, Line 5, Col 20,
                                 "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    ")

    [<Fact>]
    let ``Fail if else branch is missing in nesting``() =
        FSharp """
let x = 10
let y =
   if x > 10 then ("test")
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 4, Col 20, Line 4, Col 26,
                                 "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'.")

    [<Fact>]
    let ``Fail if else branch is missing in elif chain``() =
        FSharp """
let x = 10
let y =
   if x > 10 then "test"
   elif x > 2 then "blah"
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 5, Col 4, Line 5, Col 26,
                                 "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'.")

    [<Fact>]
    let ``Fail if else branch is missing in nested elif chain``() =
        FSharp """
let x = 10
let y =
   if x > 10 then "test"
   elif x > 5 then "middle"
   elif x > 2 then "blah"
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 6, Col 4, Line 6, Col 26,
                                 "This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'.")

    [<Fact>]
    let ``Elif chain with final else compiles cleanly``() =
        FSharp """
let x = 10
let y =
   if x > 10 then "a"
   elif x > 5 then "b"
   else "c"
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Unit-returning elif chain without else compiles cleanly``() =
        FSharp """
let x = 10
if x > 10 then printfn "a"
elif x > 5 then printfn "b"
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Deep elif chain with final else compiles cleanly``() =
        FSharp """
let x = 10
let y =
   if x > 10 then "a"
   elif x > 8 then "b"
   elif x > 5 then "c"
   elif x > 2 then "d"
   else "e"
        """
        |> typecheck
        |> shouldSucceed
