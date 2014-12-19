// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open NUnit.Framework

// Various tests for the:
// Microsoft.FSharp.Core.Option module

(*
[Test Strategy]
Make sure each method works on:
* Integer option (value type)
* String option  (reference type)
* None   (0 elements)
*)

[<TestFixture>]
type OptionModule() =

    [<Test>]
    member this.FilterSomeIntegerWhenPredicateReturnsTrue () =
        let test x =
            let actual = x |> Some |> Option.filter (fun _ -> true)

            let expected = x |> Some
            Assert.AreEqual(expected, actual)            
        [0;1;-1;42] |> List.iter test

    [<Test>]
    member this.FilterSomeStringWhenPredicateReturnsTrue () =
        let test x =
            let actual = x |> Some |> Option.filter (fun _ -> true)

            let expected = x |> Some
            Assert.AreEqual(expected, actual)
        [""; " "; "Foo"; "Bar"] |> List.iter test

    [<Test>]
    member this.FilterSomeIntegerWhenPredicateReturnsFalse () =
        let test x =
            let actual = x |> Some |> Option.filter (fun _ -> false)

            let expected = None
            Assert.AreEqual(expected, actual)
        [0; 1; -1; 1337] |> List.iter test

    [<Test>]
    member this.FilterSomeStringWhenPredicateReturnsFalse () =
        let test x =
            let actual = x |> Some |> Option.filter (fun _ -> false)

            let expected = None
            Assert.AreEqual(expected, actual)
        [""; "  "; "Ploeh"; "Fnaah"] |> List.iter test

    [<Test>]
    member this.FilterNoneReturnsCorrectResult () =
        let test x =
            let actual = None |> Option.filter (fun _ -> x)

            let expected = None
            Assert.AreEqual(expected, actual)
        [false; true] |> List.iter test

    [<Test>]
    member this.FilterSomeIntegerWhenPredicateEqualsInput () =
        let test x =
            let actual = x |> Some |> Option.filter ((=) x)

            let expected = x |> Some
            Assert.AreEqual(expected, actual)
        [0; 1; -1; -2001] |> List.iter test

    [<Test>]
    member this.FilterSomeStringWhenPredicateEqualsInput () =
        let test x =
            let actual = x |> Some |> Option.filter ((=) x)

            let expected = x |> Some
            Assert.AreEqual(expected, actual)
        [""; "     "; "Xyzz"; "Sgryt"] |> List.iter test

    [<Test>]
    member this.FilterSomeIntegerWhenPredicateDoesNotEqualsInput () =
        let test x =
            let actual = x |> Some |> Option.filter ((<>) x)

            let expected = None
            Assert.AreEqual(expected, actual)
        [0; 1; -1; 927] |> List.iter test

    [<Test>]
    member this.FilterSomeStringWhenPredicateDoesNotEqualsInput () =
        let test x =
            let actual = x |> Some |> Option.filter ((<>) x)

            let expected = None
            Assert.AreEqual(expected, actual)
        [""; "     "; "Baz Quux"; "Corge grault"] |> List.iter test
        
    [<Test>]
    member this.OfToNullable() =
        Assert.IsTrue( Option.ofNullable (System.Nullable<int>()) = None)
        Assert.IsTrue( Option.ofNullable (System.Nullable<int>(3)) = Some 3)

        Assert.IsTrue( Option.toNullable (None : int option) = System.Nullable<int>())
        Assert.IsTrue( Option.toNullable (None : System.DateTime option) = System.Nullable())
        Assert.IsTrue( Option.toNullable (Some 3) = System.Nullable(3))

    [<Test>]
    member this.OfToObj() =
        Assert.IsTrue( Option.toObj (Some "3") = "3")
        Assert.IsTrue( Option.toObj (Some "") = "")
        Assert.IsTrue( Option.toObj (Some null) = null)
        Assert.IsTrue( Option.toObj None = null)     
     
        Assert.IsTrue( Option.ofObj "3" = Some "3")
        Assert.IsTrue( Option.ofObj "" = Some "")
        Assert.IsTrue( Option.ofObj [| "" |] = Some [| "" |])
        Assert.IsTrue( Option.ofObj (null : string array) = None)
        Assert.IsTrue( Option.ofObj<string> null = None)
        Assert.IsTrue( Option.ofObj<string[]> null = None)
        Assert.IsTrue( Option.ofObj<int[]> null = None)