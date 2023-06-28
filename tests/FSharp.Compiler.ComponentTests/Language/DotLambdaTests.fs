﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.DotLambdaTests

open Xunit
open FSharp.Test.Compiler


[<Fact>]
let ``Underscore Dot ToString`` () =
    Fsx """
let x = "a" |> _.ToString()
printfn "%s" x"""
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Underscore Dot Length on string`` () =         
    Fsx """ 
let x = "a" |> _.Length
printfn "%i" x
"""
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Types`` () =
    Fsx """
let a : (string array -> _) = _.Length
let b = _.ToString()
let c = _.ToString().Length
//let c = _.ToString()[0] """
    |> withLangVersionPreview
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
            |> withLangVersionPreview
            |> typecheck 
            |> shouldSucceed  

[<Fact>]
let ``DotLambda does NOT generalize automatically to a member based SRTP`` () =
    Fsx "let inline myFunc x = x |> _.WhatANiceProperty"
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [(Error 72, Line 1, Col 28, Line 1, Col 47, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")] 

[<Fact>]
let ``DotLambda does allow member based SRTP if labelled explicitely`` () =
    Fsx "let inline myFunc<'a when 'a:(member WhatANiceProperty: int)> (x: 'a) = x |> _.WhatANiceProperty "
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed    

[<Fact>]
let ``ToString with preview version`` () =
    Fsx "let myFunc = _.ToString()"
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression in neg typecheck hole as left arg`` () =
    Fsx """
let a = ( upcast _ ) : obj
let b = ( _ :> _ ) : obj
let c = ( _ :> obj) """
    |> withLangVersionPreview
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
    Fsx "_.ToString()"
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 3350, Line 1, Col 1, Line 1, Col 3, "Feature 'underscore dot shorthand for accessor only function' is not available in F# 7.0. Please use language version 'PREVIEW' or greater." )
        
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
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Nested anonymous unary function shorthands compile`` () =
    FSharp """
module One
let a : string = {| Inner =  (fun x -> x.ToString()) |} |> _.Inner([5] |> _.[0])
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 3, Col 75, Line 3, Col 76, "Discard is ambiguous")
        
[<Fact>]
let ``Anonymous unary function shorthand with conflicting wild argument`` () =
    FSharp """
module One
let a : string -> string = (fun _ -> 5 |> _.ToString())
let b : int -> int -> string = function |5 -> (fun _ -> "Five") |_ -> _.ToString()
let c : string = let _ = "test" in "asd" |> _.ToString()
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 3, Col 43, Line 3, Col 44, "Discard is ambiguous")