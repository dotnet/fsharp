// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Core

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

    let assertWasNotCalledThunk () = raise (exn "Thunk should not have been called.")

    [<Test>]
    member this.Flatten () =
        Assert.AreEqual( Option.flatten None, None)
        Assert.AreEqual( Option.flatten (Some None), None)
        Assert.AreEqual( Option.flatten (Some <| Some 1), Some 1)
        Assert.AreEqual( Option.flatten (Some <| Some ""), Some "") 

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
    member this.Contains() =
        Assert.IsFalse( Option.contains 1 None)
        Assert.IsTrue( Option.contains 1 (Some 1))

        Assert.IsFalse( Option.contains "" None)
        Assert.IsTrue( Option.contains "" (Some ""))

        Assert.IsFalse( Option.contains None None)
        Assert.IsTrue( Option.contains None (Some None))
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

    [<Test>]
    member this.DefaultValue() =
        Assert.AreEqual( Option.defaultValue 3 None, 3)
        Assert.AreEqual( Option.defaultValue 3 (Some 42), 42)
        Assert.AreEqual( Option.defaultValue "" None, "")
        Assert.AreEqual( Option.defaultValue "" (Some "x"), "x")

    [<Test>]
    member this.DefaultWith() =
        Assert.AreEqual( Option.defaultWith (fun () -> 3) None, 3)
        Assert.AreEqual( Option.defaultWith (fun () -> "") None, "")

        Assert.AreEqual( Option.defaultWith assertWasNotCalledThunk (Some 42), 42)
        Assert.AreEqual( Option.defaultWith assertWasNotCalledThunk (Some ""), "")

    [<Test>]
    member this.OrElse() =
        Assert.AreEqual( Option.orElse None None, None)
        Assert.AreEqual( Option.orElse (Some 3) None, Some 3)
        Assert.AreEqual( Option.orElse None (Some 42), Some 42)
        Assert.AreEqual( Option.orElse (Some 3) (Some 42), Some 42)

        Assert.AreEqual( Option.orElse (Some "") None, Some "")
        Assert.AreEqual( Option.orElse None (Some "x"), Some "x")
        Assert.AreEqual( Option.orElse (Some "") (Some "x"), Some "x")

    [<Test>]
    member this.OrElseWith() =
        Assert.AreEqual( Option.orElseWith (fun () -> None) None, None)
        Assert.AreEqual( Option.orElseWith (fun () -> Some 3) None, Some 3)
        Assert.AreEqual( Option.orElseWith (fun () -> Some "") None, Some "")

        Assert.AreEqual( Option.orElseWith assertWasNotCalledThunk (Some 42), Some 42)
        Assert.AreEqual( Option.orElseWith assertWasNotCalledThunk (Some ""), Some "")

    [<Test>]
    member this.Map2() =
        Assert.AreEqual( Option.map2 (-) None None, None)
        Assert.AreEqual( Option.map2 (-) (Some 1) None, None)
        Assert.AreEqual( Option.map2 (-) None (Some 2), None)
        Assert.AreEqual( Option.map2 (-) (Some 1) (Some 2), Some -1)

        Assert.AreEqual( Option.map2 (+) None None, None)
        Assert.AreEqual( Option.map2 (+) (Some "x") None, None)
        Assert.AreEqual( Option.map2 (+) None (Some "y"), None)
        Assert.AreEqual( Option.map2 (+) (Some "x") (Some "y"), Some "xy")

    [<Test>]
    member this.Map3() =
        let add3 x y z = string x + string y + string z
        Assert.AreEqual( Option.map3 add3 None None None, None)
        Assert.AreEqual( Option.map3 add3 (Some 1) None None, None)
        Assert.AreEqual( Option.map3 add3 None (Some 2) None, None)
        Assert.AreEqual( Option.map3 add3 (Some 1) (Some 2) None, None)
        Assert.AreEqual( Option.map3 add3 None None (Some 3), None)
        Assert.AreEqual( Option.map3 add3 (Some 1) None (Some 3), None)
        Assert.AreEqual( Option.map3 add3 None (Some 2) (Some 3), None)
        Assert.AreEqual( Option.map3 add3 (Some 1) (Some 2) (Some 3), Some "123")

        let concat3 x y z = x + y + z
        Assert.AreEqual( Option.map3 concat3 None None None, None)
        Assert.AreEqual( Option.map3 concat3 (Some "x") None None, None)
        Assert.AreEqual( Option.map3 concat3 None (Some "y") None, None)
        Assert.AreEqual( Option.map3 concat3 (Some "x") (Some "y") None, None)
        Assert.AreEqual( Option.map3 concat3 None None (Some "z"), None)
        Assert.AreEqual( Option.map3 concat3 (Some "x") None (Some "z"), None)
        Assert.AreEqual( Option.map3 concat3 None (Some "y") (Some "z"), None)
        Assert.AreEqual( Option.map3 concat3 (Some "x") (Some "y") (Some "z"), Some "xyz")

    [<Test>]
    member this.MapBindEquivalenceProperties () =
        let fn x = x + 3
        Assert.AreEqual(Option.map fn None, Option.bind (fn >> Some) None)
        Assert.AreEqual(Option.map fn (Some 5), Option.bind (fn >> Some) (Some 5))

[<TestFixture>]
type ValueOptionTests() =

    let assertWasNotCalledThunk () = raise (exn "Thunk should not have been called.")

    [<Test>]
    member this.ValueOptionBasics () =
        Assert.AreEqual( (ValueNone: int voption), (ValueNone: int voption))
        Assert.True( (ValueNone: int voption) <= (ValueNone: int voption))
        Assert.True( (ValueNone: int voption) >= (ValueNone: int voption))
        Assert.True( (ValueNone: int voption) < (ValueSome 1: int voption))
        Assert.True( (ValueSome 0: int voption) < (ValueSome 1: int voption))
        Assert.True( (ValueSome 1: int voption) > (ValueSome 0: int voption))
        Assert.False( (ValueSome 1: int voption) < (ValueNone : int voption))
        Assert.True( (ValueSome 1: int voption) <= (ValueSome 1: int voption))
        Assert.AreEqual( compare (ValueSome 1) (ValueSome 1), 0)
        Assert.True( compare (ValueSome 0) (ValueSome 1) < 0)
        Assert.True( compare (ValueNone: int voption) (ValueSome 1) < 0)
        Assert.True( compare (ValueSome 1) (ValueNone : int voption) > 0)
        Assert.AreEqual( ValueSome 1, ValueSome 1)
        Assert.AreNotEqual( ValueSome 2, ValueSome 1)
        Assert.AreEqual( ValueSome 2, ValueSome 2)
        Assert.AreEqual( ValueSome (ValueSome 2), ValueSome (ValueSome 2))
        Assert.AreNotEqual( ValueSome (ValueSome 2), ValueSome (ValueSome 1))
        Assert.AreNotEqual( ValueSome (ValueSome 0), ValueSome ValueNone)
        Assert.AreEqual( ValueSome (ValueNone: int voption), ValueSome (ValueNone: int voption))
        Assert.AreEqual( (ValueSome (ValueNone: int voption)).Value, (ValueNone: int voption))
        Assert.AreEqual( (ValueSome 1).Value, 1)
        Assert.AreEqual( (ValueSome (1,2)).Value, (1,2))
        Assert.AreEqual(defaultValueArg ValueNone 1, 1)
        Assert.AreEqual(defaultValueArg (ValueSome 3) 1, 3)

