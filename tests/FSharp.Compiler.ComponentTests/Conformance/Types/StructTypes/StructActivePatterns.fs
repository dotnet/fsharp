// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.Types

module StructActivePatterns =

    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler


    [<Fact>]
    let ``Struct active pattern is possible`` () =
        Fs """
[<return: Struct>]
let rec (|IsOne|_|) someNumber = 
    match someNumber with
    | 1 -> ValueSome 1
    | _ -> ValueNone   
"""
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Struct active pattern must not lie about its return value when using Struct attribute`` () =
        Fs """
[<return: Struct>]
let rec (|IsOne|_|) someNumber = 
    match someNumber with
    | 1 -> Some 1
    | _ -> None   
"""
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1,Line 2, Col 1 , Line 3, Col 31, """This expression was expected to have type
    ''a voption'    
but here has type
    'int option'    """)

    [<Fact>]
    let ``Voption active pattern fails if not using return:Struct attribute`` () =
        Fs """
let rec (|IsOne|_|) someNumber = 
    match someNumber with
    | 1 -> ValueSome 1
    | _ -> ValueNone   
"""
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1,Line 2, Col 9 , Line 2, Col 31, """This expression was expected to have type
    ''a option'    
but here has type
    'int voption'    """)

    [<Fact>]
    let ``Rec struct active pattern is possible`` () =
        Fs """
[<return: Struct>]
let rec (|HasOne|_|) xs = 
    match xs with
    | [] -> ValueNone
    | h::_ when h = 1 -> ValueSome true
    | _::tail ->
        match tail with
        | HasOne x -> ValueSome x
        | _ -> ValueNone
"""
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
