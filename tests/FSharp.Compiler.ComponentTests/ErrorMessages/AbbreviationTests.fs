// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.AbbreviationTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Members 01`` () =
    Fsx """
type StringAlias = string

type StringAlias with
    member x.Length = x.Length
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 17, "Type abbreviations cannot have augmentations")
            (Error 895, Line 5, Col 5, Line 5, Col 31, "Type abbreviations cannot have members")
        ]

[<Fact>]
let ``Members 02 - Interface impl`` () =
    Fsx """
type IntAlias = int

type IntAlias with
    interface System.IComparable with
        member x.CompareTo(obj) = 0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 14, "Type abbreviations cannot have augmentations")
            (Error 906, Line 5, Col 15, Line 5, Col 33, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 5, Col 15, Line 5, Col 33, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Members 03 - Members and interface`` () =
    Fsx """
type FloatAlias = float

type FloatAlias with
    member x.IsNaN = System.Double.IsNaN(x)
    interface System.IConvertible
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 16, "Type abbreviations cannot have augmentations")
            (Error 906, Line 6, Col 15, Line 6, Col 34, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 6, Col 15, Line 6, Col 34, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Multiple types 01`` () =
    Fsx """
type ListAlias = int list
type ArrayAlias = string[]

type ListAlias with
    member x.Head = x.Head 

type ArrayAlias with
    member x.Length = x.Length
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 5, Col 6, Line 5, Col 15, "Type abbreviations cannot have augmentations")
            (Error 895, Line 6, Col 5, Line 6, Col 27, "Type abbreviations cannot have members")
            (Error 964, Line 8, Col 6, Line 8, Col 16, "Type abbreviations cannot have augmentations")
            (Error 895, Line 9, Col 5, Line 9, Col 31, "Type abbreviations cannot have members")
        ]

[<Fact>]
let ``Multiple types 02 - With interface`` () =
    Fsx """
type ArrayAlias = string[]

type ArrayAlias with
    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = null
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 16, "Type abbreviations cannot have augmentations")
            (Error 906, Line 5, Col 15, Line 5, Col 45, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 5, Col 15, Line 5, Col 45, "All implemented interfaces should be declared on the initial declaration of the type")
        ]
[<Fact>]
let ``Nested 01`` () =
    Fsx """
namespace Test

type Alias1 = int

module Nested =
    type Alias2 = string
    
    type Alias1 with
        member x.Value = x
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 9, Col 10, Line 9, Col 16, "Type abbreviations cannot have augmentations");
            (Error 895, Line 10, Col 9, Line 10, Col 27, "Type abbreviations cannot have members");
        ]

[<Fact>]
let ``Nested 02 - Different namespace`` () =
    Fsx """
namespace Test

module Nested =
    type Alias2 = string
    
    type Alias2 with
        interface System.IComparable with
            member x.CompareTo(obj) = 0

open Nested

type Alias2 with
    member x.ToUpper() = x.ToUpper()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 7, Col 10, Line 7, Col 16, "Type abbreviations cannot have augmentations");
            (Error 906, Line 8, Col 19, Line 8, Col 37, "Type abbreviations cannot have interface declarations");
            (Error 909, Line 8, Col 19, Line 8, Col 37, "All implemented interfaces should be declared on the initial declaration of the type");
            (Error 964, Line 13, Col 6, Line 13, Col 12, "Type abbreviations cannot have augmentations");
            (Error 895, Line 14, Col 5, Line 14, Col 37, "Type abbreviations cannot have members");
            (Error 644, Line 14, Col 14, Line 14, Col 21, "Namespaces cannot contain extension members except in the same file and namespace declaration group where the type is defined. Consider using a module to hold declarations of extension members.");
        ]

[<Fact>]
let ``Generic 01`` () =
    Fsx """
type Result<'T> = Result<'T, string>

type Result<'T> with
    member x.IsOk = match x with Ok _ -> true | Error _ -> false
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 12, "Type abbreviations cannot have augmentations");
            (Error 895, Line 5, Col 5, Line 5, Col 65, "Type abbreviations cannot have members");
        ]

[<Fact>]
let ``Generic 02 - Interface`` () =
    Fsx """
type MyList<'a> = 'a list

type MyList<'a> with
    interface seq<'a> with
        member x.GetEnumerator() = (x :> seq<'a>).GetEnumerator()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 12, "Type abbreviations cannot have augmentations");
            (Error 906, Line 5, Col 15, Line 5, Col 22, "Type abbreviations cannot have interface declarations");
            (Error 909, Line 5, Col 15, Line 5, Col 22, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Property getters and setters`` () =
    Fsx """
type IntRef = int ref

type IntRef with
    member x.Value 
        with get() = !x 
        and set(v) = x := v
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 12, "Type abbreviations cannot have augmentations")
            (Error 895, Line 5, Col 5, Line 6, Col 1, "Type abbreviations cannot have members")
        ]