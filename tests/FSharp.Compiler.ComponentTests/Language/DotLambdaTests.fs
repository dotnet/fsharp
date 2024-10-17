// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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
let ``Argument expression in inner app expression`` () =
    Fsx """
let x =  List.map _.ToString() [1; 2; 3]"""
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Argument to a function expression - should fail`` () =
    Fsx """
type Person = { DrawFromBox : int -> string }
let x : Person -> string = _.DrawFromBox 123"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCodes [3584]


[<Fact>]
let ``Bug - constant lambdas are not part of this feature`` () =
    Fsx """
let lambdaWhichAlwaysReturnsThree = _.3"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCodes [3584]

[<Fact>]
let ``Bug - bigger parenthesized expressions are not part of this feature`` () =
    Fsx """
let neverEndingLambda = _.(while true do ())"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCodes [3584]

[<Theory>]
[<InlineData("_.3")>]
[<InlineData("_.1e-04")>]
[<InlineData("_.\"🙃\"")>]
[<InlineData("_.[||]")>]
[<InlineData("_.{||}")>]
[<InlineData("_.null")>]
[<InlineData("_.__SOURCE_DIRECTORY__")>]
[<InlineData("_.(<@ 1 @>)")>]
[<InlineData("_.(nameof nameof)")>]
[<InlineData("_.struct (1, 2, 3)")>]
[<InlineData("_.{ new System.IDisposable with member _.Dispose () = () }")>]
[<InlineData("_.(while true do ())")>]
[<InlineData("_.(let x = 3 in x + x)")>]
let ``Bug - all of these should be an error`` (code:string) =
    Fsx $"""let _ = {code}"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCodes [3584]

[<Fact>]
let ``Underscore Dot ToString With Space Before Parenthesis - NonAtomic`` () =    
    Fsx """
let x = "a" |> _.ToString () """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        Error 3584, Line 2, Col 16, Line 2, Col 26, "Shorthand lambda syntax is only supported for atomic expressions, such as method, property, field or indexer on the implied '_' argument. For example: 'let f = _.Length'."
        Error 1, Line 2, Col 16, Line 2, Col 29, """Type mismatch. Expecting a
    'string -> 'a'    
but given a
    'unit -> string'    
The type 'unit' does not match the type 'string'""" ]

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
            Error 3584, Line 4, Col 36, Line 4, Col 45, "Shorthand lambda syntax is only supported for atomic expressions, such as method, property, field or indexer on the implied '_' argument. For example: 'let f = _.Length'."
            Error 72, Line 4, Col 36, Line 4, Col 45, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved."
            ]

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
let ``DotLambda does allow member based SRTP if labelled explicitly`` () =
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
        Error 10, Line 4, Col 13, Line 4, Col 15, "Unexpected symbol ':>' in expression. Expected '.' or other token."]
        
[<Fact>]
let ``ToString with F# 7`` () =
    Fsx """let x = "a" |> _.ToString()"""
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 3350, Line 1, Col 16, Line 1, Col 18, "Feature 'underscore dot shorthand for accessor only function' is not available in F# 7.0. Please use language version 8.0 or greater." )
  
[<Theory>]
[<InlineData("let f (a, (b, c)) = _.ToString()")>]
[<InlineData("""let c = let _ = "test" in "asd" |> _.ToString() """)>]
[<InlineData("""let a = (fun _almost -> 5 |> _.ToString()) """)>]
let ``Regression 16276 - hidden discard value`` (code) =
    Fsx code
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression 16276 - hidden discard value - nested`` () =
    Fsx """
let f (a, (b, c)) = 
    let _ = 42
    let _ = 43
    (fun (a, (b, c)) -> 
        let _ = 458
        (fun _ -> 5 |> _.ToString()))"""
    |> withLangVersion80
    |> withWarnOn 3570
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 7, Col 24, Line 7, Col 25, "The meaning of _ is ambiguous here. It cannot be used for a discarded variable and a function shorthand in the same scope.")

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
let ``Regression 16318 Error on explicit generic type argument dot dot lambda`` () =

    
    FSharp """
module Regression
type A() =
    member x.M<'T>() = 1

let _ = [A()] |> Seq.map _.M<int>()
let _ = [A()] |> Seq.map _.M()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression 16318 typeof dotlambda should fail`` () = 
    FSharp """ let x = _.typeof<int>"""
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [Error 72, Line 1, Col 10, Line 1, Col 18, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved."]

[<Fact>]
let ``Nested anonymous unary function shorthands fails because of ambiguous discard`` () =
    FSharp """
module One
let a : string = {| Inner =  (fun x -> x.ToString()) |} |> _.Inner([5] |> _.[0])
    """
    |> withLangVersion80
    |> withWarnOn 3570
    |> typecheck
    |> shouldFail
    |> withDiagnostics [ 
        Warning 3570, Line 3, Col 75, Line 3, Col 76, "The meaning of _ is ambiguous here. It cannot be used for a discarded variable and a function shorthand in the same scope." 
        Error 3584, Line 3, Col 77, Line 3, Col 80, "Shorthand lambda syntax is only supported for atomic expressions, such as method, property, field or indexer on the implied '_' argument. For example: 'let f = _.Length'." ]
        
[<Fact>]
let ``Anonymous unary function shorthand with conflicting wild argument`` () =
    FSharp """
module One
let a : string -> string = (fun _ -> 5 |> _.ToString())
let b : int -> int -> string = function |5 -> (fun _ -> "Five") |_ -> _.ToString()
let c : string = let _ = "test" in "asd" |> _.ToString()
    """
    |> withLangVersion80
    |> withWarnOn 3570
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Warning 3570, Line 3, Col 43, Line 3, Col 44, "The meaning of _ is ambiguous here. It cannot be used for a discarded variable and a function shorthand in the same scope.")
    
[<Fact>]
let ``DotLambda selector converted to Func when used in LINQ`` () =
    FSharp """open System.Linq
let _ = [""; ""; ""].Select(fun x -> x.Length)
let _ = [""; ""; ""].Select(_.Length)
let _ = [""; ""; ""].Select _.Length

let asQ = [""; ""; ""].AsQueryable()
let _ = asQ.Select(fun x -> x.Length)
let _ = asQ.Select(_.Length)
let _ = asQ.Select _.Length
"""
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
    let ``Error when property has same name as DU case`` () =
        Fsx """
type MyId =
    | IdA of int
    | IdB of string
    | IdC of float

    member this.IdA =
        match this with
        | IdA x -> Some x
        | _ -> None
        
    member this.IdX =
        match this with
        | IdB x -> Some x
        | _ -> None

    member this.IdC =
        match this with
        | IdC x -> Some x
        | _ -> None

let onlyIdA (ids: MyId list) = ids |> List.choose _.IdA
let onlyIdX (ids: MyId list) = ids |> List.choose _.IdX
let onlyIdC (ids: MyId list) = ids |> List.choose _.IdC
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 7, Col 17, Line 7, Col 20, "The member 'IdA' cannot be defined because the name 'IdA' clashes with the union case 'IdA' in this type or module");
            (Error 812, Line 22, Col 51, Line 22, Col 56, "The syntax 'expr.id' may only be used with record labels, properties and fields");
            (Error 23, Line 17, Col 17, Line 17, Col 20, "The member 'IdC' cannot be defined because the name 'IdC' clashes with the union case 'IdC' in this type or module");
            (Error 812, Line 24, Col 51, Line 24, Col 56, "The syntax 'expr.id' may only be used with record labels, properties and fields")
        ]