﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.DotLambdaTests

open Xunit
open FSharp.Test.Compiler



// range test for _.ExistingProperty.NonExistingProperty

[<Fact>]
let ``Underscore Dot ToString`` () =
    Fsx """
let x = "a" |> _.ToString()
printfn "%s" x"""
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Underscore Dot ToString With Space Before Paranthesis - NonAtomic`` () =    
    Fsx """
let x = "a" |> _.ToString () """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
            (Error 10, Line 2, Col 1, Line 2, Col 30, "Incomplete structured construct at or before this point in expression")
            (Error 3571, Line 2, Col 16, Line 2, Col 17, " _. shorthand syntax for lambda functions can only be used with atomic expressions. That means expressions with no whitespace unless enclosed in parentheses.")]


[<Fact>]
let ``Underscore Dot Curried Function With Arguments - NonAtomic`` () =
    Fsx """
type MyRecord = {MyRecordField:string}
    with member x.DoStuff a b c = $"%s{x.MyRecordField} %i{a} %i{b} %i{c}"
let myFunction (x:MyRecord) = x |> _.DoStuff 1 2 3"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
            (Error 10, Line 4, Col 1, Line 4, Col 51, "Incomplete structured construct at or before this point in expression")
            (Error 3571, Line 4, Col 36, Line 4, Col 37, " _. shorthand syntax for lambda functions can only be used with atomic expressions. That means expressions with no whitespace unless enclosed in parentheses.")]

[<Fact>]
let ``Underscore Dot Length on string`` () =         
    Fsx """ 
let x = "a" |> _.Length
printfn "%i" x
"""
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Types`` () =
    Fsx """
let a : (string array -> _) = _.Length
let b = _.ToString()
let c = _.ToString().Length
//let c = _.ToString()[0] """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Regression with an underscore using pattern match`` () =
            Fsx """
type MyDU = A | B
let getnumberOutOfDU x = 
    match x with
    | A -> 42
    | _ -> 43""" 
            |> withLangVersion80
            |> typecheck 
            |> shouldSucceed  

[<Fact>]
let ``DotLambda does NOT generalize automatically to a member based SRTP`` () =
    Fsx "let inline myFunc x = x |> _.WhatANiceProperty"
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [(Error 72, Line 1, Col 28, Line 1, Col 47, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")] 

[<Fact>]
let ``DotLambda does allow member based SRTP if labelled explicitely`` () =
    Fsx "let inline myFunc<'a when 'a:(member WhatANiceProperty: int)> (x: 'a) = x |> _.WhatANiceProperty "
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed    

[<Fact>]
let ``ToString with preview version`` () =
    Fsx "let myFunc = _.ToString()"
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression in neg typecheck hole as left arg`` () =
    Fsx """
let a = ( upcast _ ) : obj
let b = ( _ :> _ ) : obj
let c = ( _ :> obj) """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        Error 10, Line 2, Col 20, Line 2, Col 21, "Unexpected symbol ')' in expression. Expected '.' or other token."
        Error 10, Line 3, Col 13, Line 3, Col 15, "Unexpected symbol ':>' in expression. Expected '.' or other token."
        Error 583, Line 3, Col 9, Line 3, Col 10, "Unmatched '('"
        Error 10, Line 4, Col 13, Line 4, Col 15, "Unexpected symbol ':>' in expression. Expected '.' or other token."
        Error 583, Line 4, Col 9, Line 4, Col 10, "Unmatched '('"]
        
[<Fact>]
let ``ToString with F# 7`` () =
    Fsx """let x = "a" |> _.ToString()"""
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 3350, Line 1, Col 16, Line 1, Col 18, "Feature 'underscore dot shorthand for accessor only function' is not available in F# 7.0. Please use language version 8.0 or greater." )
        
[<Fact>]
let ``Simple anonymous unary function shorthands compile`` () =
    FSharp """
module One
let a1 : {| Foo : {| Bar : string |}|} -> string = _.Foo.Bar
let a2 : {| Foo : int array |} -> int = _.Foo.[5]
let a3 : {| Foo : int array |} -> int = _.Foo[5]
let a4 : {| Foo : unit -> string |} -> string = _.Foo()
let a5 : {| Foo : int -> {| X : string |} |} -> string = _.Foo(5).X

open System
let a6 = [1] |> List.map _.ToString()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Nested anonymous unary function shorthands fails because of ambigous discard`` () =
    FSharp """
module One
let a : string = {| Inner =  (fun x -> x.ToString()) |} |> _.Inner([5] |> _.[0])
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 3, Col 75, Line 3, Col 76, "The meaning of _ is ambiguous here. It cannot be used for a discarded variable and a function shorthand in the same scope.")
        
[<Fact>]
let ``Anonymous unary function shorthand with conflicting wild argument`` () =
    FSharp """
module One
let a : string -> string = (fun _ -> 5 |> _.ToString())
let b : int -> int -> string = function |5 -> (fun _ -> "Five") |_ -> _.ToString()
let c : string = let _ = "test" in "asd" |> _.ToString()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 3, Col 43, Line 3, Col 44, "The meaning of _ is ambiguous here. It cannot be used for a discarded variable and a function shorthand in the same scope.")