// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.AbbreviationTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Type abbreviations with members should report all member errors`` () =
    Fsx """
type StringAlias = string

type StringAlias with
    member x.Length = x.Length
    member x.ToUpper() = x.ToUpper()
    static member Parse(s) = s
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 17, "Type abbreviations cannot have augmentations")
            (Error 895, Line 5, Col 5, Line 5, Col 31, "Type abbreviations cannot have members")
            (Error 895, Line 6, Col 5, Line 6, Col 37, "Type abbreviations cannot have members")
            (Error 895, Line 7, Col 5, Line 7, Col 31, "Type abbreviations cannot have members")
        ]

[<Fact>]
let ``Type abbreviations with interface declarations should report all errors`` () =
    Fsx """
type IntAlias = int

type IntAlias with
    interface System.IComparable with
        member x.CompareTo(obj) = 0
    interface System.IFormattable with
        member x.ToString(format, provider) = ""
    member x.Value = x
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 14, "Type abbreviations cannot have augmentations")
            (Error 906, Line 5, Col 15, Line 5, Col 33, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 5, Col 15, Line 5, Col 33, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Type abbreviations with mixed members and interfaces should report all errors`` () =
    Fsx """
type FloatAlias = float

type FloatAlias with
    member x.IsNaN = System.Double.IsNaN(x)
    interface System.IConvertible
    static member Zero = 0.0
    member x.Sqrt() = sqrt x
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 16, "Type abbreviations cannot have augmentations")
            (Error 906, Line 6, Col 15, Line 6, Col 34, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 6, Col 15, Line 6, Col 34, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Multiple type abbreviations with errors should all be reported`` () =
    Fsx """
type ListAlias = int list
type ArrayAlias = string[]
type OptionAlias = int option

type ListAlias with
    member x.Head = x.Head 
    member x.Tail = x.Tail

type ArrayAlias with
    member x.Length = x.Length
    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = null

type OptionAlias with
    member x.IsSome = x.IsSome
    member x.IsNone = x.IsNone
    static member None = None 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 6, Col 6, Line 6, Col 15, "Type abbreviations cannot have augmentations")
            (Error 895, Line 7, Col 5, Line 7, Col 27, "Type abbreviations cannot have members")
            (Error 895, Line 8, Col 5, Line 8, Col 27, "Type abbreviations cannot have members")
            (Error 964, Line 10, Col 6, Line 10, Col 16, "Type abbreviations cannot have augmentations")
            (Error 906, Line 12, Col 15, Line 12, Col 45, "Type abbreviations cannot have interface declarations")
            (Error 909, Line 12, Col 15, Line 12, Col 45, "All implemented interfaces should be declared on the initial declaration of the type")
            (Error 964, Line 15, Col 6, Line 15, Col 17, "Type abbreviations cannot have augmentations")
            (Error 895, Line 16, Col 5, Line 16, Col 31, "Type abbreviations cannot have members")
            (Error 895, Line 17, Col 5, Line 17, Col 31, "Type abbreviations cannot have members")
            (Error 895, Line 18, Col 5, Line 18, Col 30, "Type abbreviations cannot have members")
            (Error 1198, Line 18, Col 19, Line 18, Col 23, "The generic member 'None' has been used at a non-uniform instantiation prior to this program point. Consider reordering the members so this member occurs first. Alternatively, specify the full type of the member explicitly, including argument types, return type and any additional generic parameters and constraints.")
      
        ]

[<Fact>]
let ``Nested type abbreviations with augmentations should all report errors`` () =
    Fsx """
namespace Test

type Alias1 = int

module Nested =
    type Alias2 = string
    
    type Alias1 with
        member x.Value = x  // Error
        
    type Alias2 with
        member x.Length = x.Length  // Error
        interface System.IComparable with  // Error
            member x.CompareTo(obj) = 0

open Nested

type Alias2 with
    member x.ToUpper() = x.ToUpper()  // Error
    static member Empty = ""  // Error
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 9, Col 10, Line 9, Col 16, "Type abbreviations cannot have augmentations");
            (Error 895, Line 10, Col 9, Line 10, Col 27, "Type abbreviations cannot have members");
            (Error 964, Line 12, Col 10, Line 12, Col 16, "Type abbreviations cannot have augmentations");
            (Error 906, Line 14, Col 19, Line 14, Col 37, "Type abbreviations cannot have interface declarations");
            (Error 909, Line 14, Col 19, Line 14, Col 37, "All implemented interfaces should be declared on the initial declaration of the type");
            (Error 964, Line 19, Col 6, Line 19, Col 12, "Type abbreviations cannot have augmentations");
            (Error 895, Line 20, Col 5, Line 20, Col 37, "Type abbreviations cannot have members");
            (Error 644, Line 20, Col 14, Line 20, Col 21, "Namespaces cannot contain extension members except in the same file and namespace declaration group where the type is defined. Consider using a module to hold declarations of extension members.");
            (Error 895, Line 21, Col 5, Line 21, Col 29, "Type abbreviations cannot have members");
            (Error 644, Line 21, Col 19, Line 21, Col 24, "Namespaces cannot contain extension members except in the same file and namespace declaration group where the type is defined. Consider using a module to hold declarations of extension members.")
        ]

[<Fact>]
let ``Generic type abbreviations with augmentations should report all errors`` () =
    Fsx """
type Result<'T> = Result<'T, string>
type MyList<'a> = 'a list

type Result<'T> with
    member x.IsOk = match x with Ok _ -> true | Error _ -> false
    member x.IsError = not x.IsOk
    static member Ok(value) = Ok value

type MyList<'a> with
    member x.Head = List.head x 
    member x.Tail = List.tail x
    interface seq<'a> with
        member x.GetEnumerator() = (x :> seq<'a>).GetEnumerator()
    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = (x :> System.Collections.IEnumerable).GetEnumerator()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 5, Col 6, Line 5, Col 12, "Type abbreviations cannot have augmentations");
            (Error 895, Line 6, Col 5, Line 6, Col 65, "Type abbreviations cannot have members");
            (Error 895, Line 7, Col 5, Line 7, Col 34, "Type abbreviations cannot have members");
            (Error 895, Line 8, Col 5, Line 8, Col 39, "Type abbreviations cannot have members");
            (Error 1198, Line 8, Col 19, Line 8, Col 21, "The generic member 'Ok' has been used at a non-uniform instantiation prior to this program point. Consider reordering the members so this member occurs first. Alternatively, specify the full type of the member explicitly, including argument types, return type and any additional generic parameters and constraints.");
            (Error 964, Line 10, Col 6, Line 10, Col 12, "Type abbreviations cannot have augmentations");
            (Error 906, Line 13, Col 15, Line 13, Col 22, "Type abbreviations cannot have interface declarations");
            (Error 909, Line 13, Col 15, Line 13, Col 22, "All implemented interfaces should be declared on the initial declaration of the type")
        ]

[<Fact>]
let ``Type abbreviations with property getters and setters should report all errors`` () =
    Fsx """
type IntRef = int ref

type IntRef with
    member x.Value 
        with get() = !x 
        and set(v) = x := v
    member x.Increment() = x := !x + 1
    member x.Decrement() = x := !x - 1 
    static member Zero = ref 0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 964, Line 4, Col 6, Line 4, Col 12, "Type abbreviations cannot have augmentations")
            (Error 895, Line 5, Col 5, Line 6, Col 1, "Type abbreviations cannot have members")
            (Error 895, Line 8, Col 5, Line 8, Col 39, "Type abbreviations cannot have members")
            (Error 895, Line 9, Col 5, Line 9, Col 39, "Type abbreviations cannot have members")
            (Error 895, Line 10, Col 5, Line 10, Col 31, "Type abbreviations cannot have members")
            (Error 1198, Line 10, Col 19, Line 10, Col 23, "The generic member 'Zero' has been used at a non-uniform instantiation prior to this program point. Consider reordering the members so this member occurs first. Alternatively, specify the full type of the member explicitly, including argument types, return type and any additional generic parameters and constraints.")
        ]
