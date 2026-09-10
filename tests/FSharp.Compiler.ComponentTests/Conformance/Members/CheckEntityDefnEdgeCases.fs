// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Members

open Xunit
open FSharp.Test.Compiler

// Regression coverage for duplicate-member diagnostics emitted by CheckEntityDefn
// in src/Compiler/Checking/PostInferenceChecks.fs (lines ~2483-2566).
module CheckEntityDefnEdgeCases =

    // (a) Duplicate abstract method inherited from a base type, where the two
    // signatures differ only in an erased unit-of-measure type argument.
    // Exercises chkDuplicateMethodInheritedType(WithSuffix) (FS0442).
    [<Fact>]
    let ``Duplicate abstract method differing only in erased measure (FS0442)`` () =
        FSharp """
module Test
[<Measure>] type m

[<AbstractClass>]
type A() =
    abstract DoStuff : int -> int

[<AbstractClass>]
type B() =
    inherit A()
    abstract DoStuff : int<m> -> int<m>
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 442

    // (b) Property and method on the same type share a name.
    // Exercises chkPropertySameNameMethod (FS0434).
    [<Fact>]
    let ``Property and method with same name (FS0434)`` () =
        FSharp """
module Test
type T() =
    member val P = 0 with get
    member _.P() = 1
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 434

    // (c) A new member in a derived class has the same name/signature as an
    // inherited abstract member but is not declared as override.
    // Exercises tcNewMemberHidesAbstractMember(WithSuffix) (FS0864 warning).
    [<Fact>]
    let ``New member hides inherited abstract member (FS0864 warning)`` () =
        FSharp """
module Test
type Base() =
    abstract M : int -> int
    default _.M x = x

type Derived() =
    inherit Base()
    member _.M(x: int) = x + 1
        """
        |> withOptions [ "--warnaserror+" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 864

    // (d) Indexer and non-indexer property share a name on the same type.
    // Exercises chkPropertySameNameIndexer (FS0436).
    [<Fact>]
    let ``Indexer vs non-indexer property with same name (FS0436)`` () =
        FSharp """
module Test
type T() =
    let mutable x = 0
    member _.P with get () = x and set v = x <- v
    member _.P with get (i: int) = i
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 436

    // (e) Getter/setter for the same (non-indexer) property have different types.
    // Exercises chkGetterAndSetterHaveSamePropertyType (FS3172).
    [<Fact>]
    let ``Getter and setter with different property types (FS3172)`` () =
        FSharp """
module Test
type T() =
    let mutable x = 0
    member _.P
        with get () : int = x
        and set (v: string) = ignore v
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 3172
