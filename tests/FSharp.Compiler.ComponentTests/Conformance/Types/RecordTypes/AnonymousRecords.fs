// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test.Compiler

module AnonymousRecord =

    [<Fact>]
    let ``Anonymous Records with duplicate labels`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| A = 1; A = 2 |}
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3522
        |> withMessage "The field 'A' appears multiple times in this record expression."

    [<Fact>]
    let ``Anonymous Record with unit of measures`` () =
        FSharp """
namespace FSharpTest

[<Measure>]
type m

module AnonRecd =
    let a = {|a=1<m>|}
    let b = {|a=1<m>; b=2<m>|}
    let c = {|a=1<m> |}
    let d = {| a=1<m>; b=2<m>; c=3<m> |}   
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 8, Col 20, Line 8, Col 23, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 9, Col 28, Line 9, Col 31, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Fact>]
    let ``Preview : Anonymous Record with unit of measures`` () =
        FSharp """
namespace FSharpTest

[<Measure>]
type m

module AnonRecd =
    let a = {|a=1<m>|}
    let b = {|a=1<m>; b=2<m>|}
    let c = {|a=1<m> |}
    let d = {| a=1<m>; b=2<m>; c=3<m> |}   
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``Anonymous Record with typeof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a = {|a=typeof<int>|}
    let b = {|a=typeof<int> |}
    let c = {| a=typeof<int>|}
    let d = {| a=typeof<int> |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 27, Line 4, Col 30, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 6, Col 28, Line 6, Col 31, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ] 
    
    [<Fact>]
    let ``Preview: Anonymous Record with typeof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a = {|a=typeof<int>|}
    let b = {|a=typeof<int> |}
    let c = {| a=typeof<int>|}
    let d = {| a=typeof<int> |}
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed    
    
    [<Fact>]
    let ``Anonymous Record with typedefof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a = {|a=typedefof<_ option>|}
    let b = {|a=typedefof<_ option> |}
    let c = {| a=typedefof<_ option>|}
    let d = {| a=typedefof<_ option> |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 35, Line 4, Col 38, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 6, Col 36, Line 6, Col 39, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
    
    [<Fact>]
    let ``Preview: Anonymous Record with typedefof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a = {|a=typedefof<_ option>|}
    let b = {|a=typedefof<_ option> |}
    let c = {| a=typedefof<_ option>|}
    let d = {| a=typedefof<_ option> |}
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed    
    
    [<Fact>]
    let ``Anonymous Record with nameof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a<'T> = {|a=nameof<'T>|}
    let b<'T> = {|a=nameof<'T> |}
    let c<'T> = {| a=nameof<'T>|}
    let d<'T> = {| a=nameof<'T> |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 30, Line 4, Col 33, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 6, Col 31, Line 6, Col 34, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
    
    [<Fact>]
    let ``Preview: Anonymous Record with nameof`` () =
        FSharp """
namespace FSharpTest
module AnonRecd =
    let a<'T> = {|a=nameof<'T>|}
    let b<'T> = {|a=nameof<'T> |}
    let c<'T> = {| a=nameof<'T>|}
    let d<'T> = {| a=nameof<'T> |}
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``Anonymous Record missing single field`` () =
        Fsx """
let x () : {| A: int; B: string  |} =  {| A = 123 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 40, Line 2, Col 53, "This anonymous record is missing field 'B'.")
        ]
        
    [<Fact>]
    let ``Anonymous Record missing multiple fields`` () =
        Fsx """
let x () : {| A: int; B: string; C: int  |} =  {| A = 123 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 48, Line 2, Col 61, "This anonymous record is missing fields 'B', 'C'.")
        ]
        
    [<Fact>]
    let ``Anonymous Record with extra field`` () =
        Fsx """
let x () : {| A: int; B: string  |} =  {| A = 123; B = ""; C = 1 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 40, Line 2, Col 68, "This anonymous record has an extra field. Remove field 'C'.")
        ]
        
    [<Fact>]
    let ``Anonymous Record with extra fields`` () =
        Fsx """
let x () : {| A: int  |} =  {| A = 123 ; B = ""; C = 1 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 29, Line 2, Col 58, "This anonymous record has extra fields. Remove fields 'B', 'C'.")
        ]
        
    [<Fact>]
    let ``Using the wrong anon record with single field`` () =
        Fsx """
let x() = ({| b = 2 |} = {| a = 2 |} )
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 26, Line 2, Col 37, "This anonymous record should have field 'b' but here has field 'a'.")
        ]
        
    [<Fact>]
    let ``Using the wrong anon record with single field 2`` () =
        Fsx """
let x() = ({| b = 2 |} = {| a = 2; c = "" |} )
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 26, Line 2, Col 45, "This anonymous record should have field 'b' but here has fields 'a', 'c'.")
        ]
        
    [<Fact>]
    let ``Using the wrong anon record with multiple fields`` () =
        Fsx """
let x() = ({| b = 2; c = 3 |} = {| a = 2 |} )
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 33, Line 2, Col 44, "This anonymous record should have fields 'b', 'c'; but here has field 'a'.")
        ]
        
    [<Fact>]
    let ``Using the wrong anon record with multiple fields 2`` () =
        Fsx """
let x() = ({| b = 2; c = 3 |} = {| a = 2; d = "" |} )
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 33, Line 2, Col 52, "This anonymous record should have fields 'b', 'c'; but here has fields 'a', 'd'.")
        ]
        
    [<Fact>]
    let ``Two anon records with no fields`` () =
        Fsx """
let x() = ({||} = {||})
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Anonymous Records with duplicate labels - Copy and update expression`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| {| y = 3 |} with y = 2; y = 4 |}
"""
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3522, Line 5, Col 13, Line 5, Col 48, "The field 'y' appears multiple times in this record expression.")

    [<Fact>]
    let ``Anonymous Record type annotation with duplicate labels`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let (f : {| A : int; A : string |} option) = None
"""
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3523, Line 5, Col 17, Line 5, Col 18, "The field 'A' appears multiple times in this anonymous record type.")

    [<Fact>]
    let ``Anonymous record types with parser errors or no fields do not produce overlapping diagnostics`` () =
        FSharp """
module AnonRecd

type ContactMethod =
    | Address of {| Line1 : string; Line 2 : string; Postcode : string |}

let (x: {| |}) = ()

type ErrorResponse =
    { error: {| type : string
                message : string |} }
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 10, Line 5, Col 42, Line 5, Col 43, "Unexpected integer literal in field declaration. Expected ':' or other token."
            Error 10, Line 7, Col 12, Line 7, Col 14, "Unexpected symbol '|}' in field declaration. Expected identifier or other token."
            Error 3244, Line 7, Col 9, Line 7, Col 14, "Invalid anonymous record type"
            Error 10, Line 10, Col 17, Line 10, Col 21, "Incomplete structured construct at or before this point in field declaration. Expected identifier or other token."
            Error 3244, Line 10, Col 14, Line 11, Col 36, "Invalid anonymous record type"
        ]
        
    [<Fact>]
    let ``Anonymous Record type annotation with fields defined in a record`` () =
        Fsx """
type T = { ff : int }

let t3 (t1: {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 4, Col 42, Line 4, Col 43, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 4, Col 59, Line 4, Col 60, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an anonymous Record but has a record`` () =
        Fsx """
let t3 (t1: {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 42, Line 2, Col 43, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 59, Line 2, Col 60, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an struct anonymous Record but has a record`` () =
        Fsx """
let t3 (t1: struct {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 49, Line 2, Col 50, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 66, Line 2, Col 67, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an anonymous with various properties Record but has a record`` () =
        Fsx """
let f (r: {| A: int; C: int |}) = { r with A = 1; B = 2; C = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 35, Line 2, Col 36, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 64, Line 2, Col 65, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
    
    [<Fact>]
    let ``Nested anonymous records where outer label = concatenated inner labels (see secondary issue reported in 6411)`` () =
        FSharp """
module NestedAnonRecds

let x = {| abcd = {| ab = 4; cd = 1 |} |}
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record definition`` () =
        Fsx """
let x(f: {| A: int; A: int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 13, Line 2, Col 14, "The field 'A' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record definition 2`` () =
        Fsx """
let x(f: {| A: int; A: int; A:int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 13, Line 2, Col 14, "The field 'A' appears multiple times in this anonymous record type.")
            (Error 3523, Line 2, Col 21, Line 2, Col 22, "The field 'A' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record declaration 3`` () =
        Fsx """
let f(x:{| A: int; B: int; A:string; B: int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this anonymous record type.")
            (Error 3523, Line 2, Col 20, Line 2, Col 21, "The field 'B' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record declaration 4`` () =
        Fsx """
let f(x:{| A: int; C: string; A: int; B: int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record declaration 5`` () =
        Fsx """
let f(x:{| A: int; C: string; A: int; B: int; A: int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this anonymous record type.")
            (Error 3523, Line 2, Col 31, Line 2, Col 32, "The field 'A' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field with in double backticks appears multiple times in this anonymous record declaration`` () =
        Fsx """
let f(x:{| ``A``: int; B: int; A:string; B: int |}) = ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 12, Line 2, Col 17, "The field 'A' appears multiple times in this anonymous record type.")
            (Error 3523, Line 2, Col 24, Line 2, Col 25, "The field 'B' appears multiple times in this anonymous record type.")
        ]

    [<Fact>]
    let ``Anonymous Records field appears multiple times in this anonymous record declaration 6`` () =
        Fsx """
let foo: {| A: int; C: string; A: int; B: int; A: int |} = failwith "foo"
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3523, Line 2, Col 13, Line 2, Col 14, "The field 'A' appears multiple times in this anonymous record type.")
            (Error 3523, Line 2, Col 32, Line 2, Col 33, "The field 'A' appears multiple times in this anonymous record type.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression`` () =
        Fsx """
let v = {| A = 1; A = 2 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this record expression.")
        ]

    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression 2`` () =
        Fsx """
let v = {| A = 1; A = 2; A = 3 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this record expression.")
            (Error 3522, Line 2, Col 19, Line 2, Col 20, "The field 'A' appears multiple times in this record expression.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression 3`` () =
        Fsx """
let v = {| A = 0; B = 2; A = 5; B = 6 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this record expression.")
            (Error 3522, Line 2, Col 19, Line 2, Col 20, "The field 'B' appears multiple times in this record expression.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression 4`` () =
        Fsx """
let v = {| A = 2; C = "W"; A = 8; B = 6 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this record expression.")
        ]

    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression 5`` () =
        Fsx """
let v = {| A = 0; C = ""; A = 1; B = 2; A = 5 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 13, "The field 'A' appears multiple times in this record expression.")
            (Error 3522, Line 2, Col 27, Line 2, Col 28, "The field 'A' appears multiple times in this record expression.")
        ]
        
    [<Fact>]
    let ``Anonymous Records field appears multiple times in this record expression 6`` () =
        Fsx """
let v = {| ``A`` = 0; B = 5; A = ""; B = 0 |}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3522, Line 2, Col 12, Line 2, Col 17, "The field 'A' appears multiple times in this record expression.")
            (Error 3522, Line 2, Col 23, Line 2, Col 24, "The field 'B' appears multiple times in this record expression.")
        ]
            
    [<Fact>]
    let ``Preview: Anonymous records with typed quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr1 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @> |}
let expr2 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @>|}
let expr3 : {| A: Expr<string>; B: Expr<int> |} = {| A = <@ "hello" @>; B = <@ 42 @>|}
let expr4 : {| A: Expr<string>; B: Expr<int> |} = {| A = <@ "hello" @>; B = <@ 42 @>|}

let expr5 : {| A: Expr<int> |} = {| A= <@ 1 + 1 @> |}
let expr6 : {| A: Expr<int> |} = {| A= <@ 1 + 1 @>|}
let expr7 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @> |}
let expr8 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @>|}

let (=<@) = (@)
let (@>=) = (@)

[1..2] =<@ [3..4] @>= [5..6]
    """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Preview: Anonymous records with untyped quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr1 : {| A: Expr |} = {| A = <@@ 1 + 1 @@> |}
let expr2 : {| A: Expr |} = {| A = <@@ 1 + 1 @@>|}
let expr3 : {| A: Expr; B: Expr |} = {| A = <@@ "hello" @@>; B = <@@ 42 @@>|}
let expr4 : {| A: Expr; B: Expr |} = {| A = <@@ "hello" @@>; B = <@@ 42 @@>|}

let expr5 : {| A: Expr |} = {| A= <@@ 1 + 1 @@> |}
let expr6 : {| A: Expr |} = {| A= <@@ 1 + 1 @@>|}
let expr7 : {| A: Expr |} = {| A = <@@ 1 + 1 @@> |}
let expr8 : {| A: Expr |} = {| A = <@@ 1 + 1 @@>|}
    """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Preview: Anonymous records with mixed quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr : {| Typed: Expr<int>; Untyped: Expr |} = 
    {| Typed = <@ 1 + 1 @>; Untyped = <@@ "test" @@>|}
    
let expr2 : {| A: Expr<int>; B: Expr; C: string |} = 
    {| A = <@ 42 @>; B = <@@ true @@>; C = "normal string"|}
    """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Preview: Nested anonymous records with quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let nested = 
    {| Outer = {| Inner = <@ 1 + 1 @>|}; Other = <@@ "test" @@>|}
    
let nested2 : {| A: {| B: Expr<int> |}; C: Expr |} = 
    {| A = {| B = <@ 42 @>|}; C = <@@ true @@>|}
    """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Version 9: Anonymous records with typed quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr1 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @> |}
let expr2 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @>|}
let expr3 : {| A: Expr<string>; B: Expr<int> |} = {| A = <@ "hello" @>; B = <@ 42 @>|}
let expr4 : {| A: Expr<string>; B: Expr<int> |} = {| A = <@ "hello" @>; B = <@ 42 @>|}

let expr5 : {| A: Expr<int> |} = {| A= <@ 1 + 1 @> |}
let expr6 : {| A: Expr<int> |} = {| A= <@ 1 + 1 @>|}
let expr7 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @> |}
let expr8 : {| A: Expr<int> |} = {| A = <@ 1 + 1 @>|}

let (=<@) = (@)
let (@>=) = (@)

[1..2] =<@ [3..4] @>= [5..6]
    """
        |> withLangVersion90
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 48, Line 5, Col 49, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 6, Col 80, Line 6, Col 82, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 7, Col 80, Line 7, Col 82, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 10, Col 47, Line 10, Col 48, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 12, Col 48, Line 12, Col 49, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        
        ]

    [<Fact>]
    let ``Version 9: Anonymous records with untyped quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr1 : {| A: Expr |} = {| A = <@@ 1 + 1 @@> |}
let expr2 : {| A: Expr |} = {| A = <@@ 1 + 1 @@>|}
let expr3 : {| A: Expr; B: Expr |} = {| A = <@@ "hello" @@>; B = <@@ 42 @@>|}
let expr4 : {| A: Expr; B: Expr |} = {| A = <@@ "hello" @@>; B = <@@ 42 @@>|}

let expr5 : {| A: Expr |} = {| A= <@@ 1 + 1 @@> |}
let expr6 : {| A: Expr |} = {| A= <@@ 1 + 1 @@>|}
let expr7 : {| A: Expr |} = {| A = <@@ 1 + 1 @@> |}
let expr8 : {| A: Expr |} = {| A = <@@ 1 + 1 @@>|}
    """
        |> withLangVersion90
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 44, Line 5, Col 45, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 6, Col 70, Line 6, Col 72, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 7, Col 70, Line 7, Col 72, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 10, Col 43, Line 10, Col 44, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 12, Col 44, Line 12, Col 45, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Fact>]
    let ``Version 9: Anonymous records with mixed quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let expr : {| Typed: Expr<int>; Untyped: Expr |} = 
    {| Typed = <@ 1 + 1 @>; Untyped = <@@ "test" @@>|}
    
let expr2 : {| A: Expr<int>; B: Expr; C: string |} = 
    {| A = <@ 42 @>; B = <@@ true @@>; C = "normal string"|}
    """
        |> withLangVersion90
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 50, Line 5, Col 55, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Fact>]
    let ``Version 9: Nested anonymous records with quotations should parse correctly``() =
        Fsx """
open Microsoft.FSharp.Quotations

let nested = 
    {| Outer = {| Inner = <@ 1 + 1 @>|}; Other = <@@ "test" @@>|}
    
let nested2 : {| A: {| B: Expr<int> |}; C: Expr |} = 
    {| A = {| B = <@ 42 @>|}; C = <@@ true @@>|}
    """
        |> withLangVersion90
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 34, Line 5, Col 35, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 5, Col 61, Line 5, Col 66, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 8, Col 22, Line 8, Col 24, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 8, Col 44, Line 8, Col 49, "Feature 'Support for better anonymous record parsing' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
