// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.List type

namespace FSharp.Core.UnitTests.Collections

open System
open System.Collections
open System.Collections.Generic
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

(*
[Test Strategy]
Make sure each method works on:
* Integer List (value type)
* String List (reference type)
* Empty List (0 elements)
*)

type ListType() =

    // Interfaces
    [<Fact>]
    member this.IEnumerable() =
        
        // Legit IE
        let ie = ['a'; 'b'; 'c'] :> IEnumerable
        let enum = ie.GetEnumerator()
        
        let testStepping() =
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
            Assert.AreEqual(true, enum.MoveNext())
            Assert.AreEqual('a', enum.Current)
            Assert.AreEqual(true, enum.MoveNext())
            Assert.AreEqual('b', enum.Current)
            Assert.AreEqual(true, enum.MoveNext())
            Assert.AreEqual('c', enum.Current)
            Assert.AreEqual(false, enum.MoveNext())
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
    
        testStepping()
        enum.Reset()
        testStepping()
    
        // Empty IE
        let ie = [] :> IEnumerable  // Note no type args
        let enum = ie.GetEnumerator()
        
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        Assert.AreEqual(false, enum.MoveNext())
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)

    [<Fact>]
    member this.IEnumerable_T() =
        
        // Legit IE
        let ie = ['a'; 'b'; 'c'] :> IEnumerable<char>
        let enum = ie.GetEnumerator()
        
        let testStepping() =
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
            Assert.AreEqual(true,   enum.MoveNext())
            Assert.AreEqual('a',    enum.Current)
            Assert.AreEqual(true,   enum.MoveNext())
            Assert.AreEqual('b',    enum.Current)
            Assert.AreEqual(true,   enum.MoveNext())
            Assert.AreEqual('c',    enum.Current)
            Assert.AreEqual(false, enum.MoveNext())
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        
        testStepping()
        enum.Reset()
        testStepping()
    
        // Empty IE
        let ie = [] :> IEnumerable<int>  // Note no type args
        let enum = ie.GetEnumerator()
        
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        Assert.AreEqual(false, enum.MoveNext())
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
 
    [<Fact>]
    member this.IReadOnlyCollection_T() =
        
        // Legit IReadOnlyCollection_T
        let c = ['a'; 'b'; 'c'] :> IReadOnlyCollection<char>

        Assert.AreEqual(c.Count, Seq.length c)

        let c = [1..10] :> IReadOnlyCollection<int>

        Assert.AreEqual(c.Count, 10)

        // Empty IReadOnlyCollection_T
        let c = [] :> IReadOnlyCollection<int>

        Assert.AreEqual(c.Count, 0)

    [<Fact>]
    member this.IReadOnlyList_T() =

        let c = ['a'; 'b'; 'c'] :> IReadOnlyList<char>

        Assert.AreEqual(c.[1], 'b')

        let c = [1..10] :> IReadOnlyList<int>

        Assert.AreEqual(c.[5], 6)

        let c = [] :> IReadOnlyList<int>

        CheckThrowsArgumentException(fun () -> c.[0] |> ignore)

    // Base class methods
    [<Fact>]
    member this.ObjectToString() =
        Assert.AreEqual("[1; 2; 3]", [1; 2; 3].ToString())
        Assert.AreEqual("[]", [].ToString())
        Assert.AreEqual("[]", ([] : decimal list list).ToString())

    [<Fact>]
    member this.HashCodeNotThrowingStackOverflow() = 
        let l = 1 :: 2 :: [0.. 35_000]
        let hash = l.GetHashCode()

        let l2 = [1;2] @ [0.. 35_000]
        let hash2 = l.GetHashCode()

        Assert.AreEqual(hash,hash2)

    [<Fact>]
    member this.HashCodeDoesNotThrowOnListOfNullStrings() = 
        let l = ["1";"2";null;null]
        Assert.AreEqual(l.GetHashCode(),l.GetHashCode())

    [<Fact>]
    member this.HashCodeIsDifferentForListsWithSamePrefix() = 
        let sharedPrefix = [0..500]
        let l1 = sharedPrefix @ [1]
        let l2 = sharedPrefix @ [2]       

        Assert.AreNotEqual(l1.GetHashCode(),l2.GetHashCode())
    
    [<Fact>]
    member this.ObjectEquals() =
        // All three are different references, but equality has been
        // provided by the F# compiler.
        let a = [1; 2; 3]
        let b = [1 .. 3]
        let c = 1 :: [2; 3]
        Assert.True( (a = b) )
        Assert.True( (b = c) )
        Assert.True( (c = a) )
        Assert.True( a.Equals(b) ); Assert.True( b.Equals(a) )
        Assert.True( b.Equals(c) ); Assert.True( c.Equals(b) )
        Assert.True( c.Equals(a) ); Assert.True( a.Equals(c) )

        // Equality between types
        let a = [] : int list
        let b = [] : string list
        Assert.False( b.Equals(a) )
        Assert.False( a.Equals(b) )
        
        // Co/contra variance not supported
        let a = [] : string list
        let b = [] : obj list
        Assert.False(a.Equals(b))
        Assert.False(b.Equals(a))
        
        // Self equality
        let a = [1]
        Assert.True( (a = a) )
        Assert.True(a.Equals(a))
        
        // Null
        Assert.False(a.Equals(null))
    
    // Instance methods
    [<Fact>]
    member this.Length() =
    
        let l = [1 .. 10]
        Assert.AreEqual(l.Length, 10)
    
        let e : int list list = List.empty
        Assert.AreEqual(e.Length, 0)
        
    [<Fact>]
    member this.IsEmpty() =
    
        let l = [1 .. 10]
        Assert.False(l.IsEmpty)
    
        let e = Microsoft.FSharp.Collections.List.Empty : string list
        Assert.True(e.IsEmpty)
        
        Assert.True( ([] @ []).IsEmpty )
        
    [<Fact>]
    member this.Head() =
        
        let l = ['a'; 'e'; 'i'; 'o'; 'u']
        Assert.AreEqual('a', l.Head)
        
        CheckThrowsInvalidOperationExn(fun () -> ([] : string list).Head |> ignore)
        
    [<Fact>]
    member this.Tail() =
        
        let l = ['a'; 'e'; 'i'; 'o'; 'u']
        Assert.AreEqual(['e'; 'i'; 'o'; 'u'], l.Tail)
        
        CheckThrowsInvalidOperationExn(fun () -> ([] : string list).Tail |> ignore)
    
    [<Fact>]
    member this.Item() =

        let mutable l = [1]
        Assert.AreEqual(1, l.[0])
        l <- l @ l
        Assert.AreEqual(1, l.[1])
        
        for testidx = 0 to 20 do
            let l = [0 .. testidx]
            for i = 0 to l.Length - 1 do
                Assert.AreEqual(i, l.[i])
                Assert.AreEqual(i, l.Item(i))
        
        // Invalid index
        let l = [1 .. 10]
        CheckThrowsArgumentException(fun () -> l.[ -1 ] |> ignore)
        CheckThrowsArgumentException(fun () -> l.[1000] |> ignore)
        
    
    // Static methods
    
    [<Fact>]
    member this.Empty() =
        let emptyList =  Microsoft.FSharp.Collections.List.Empty
        if List.length emptyList <> 0 then Assert.Fail()    
        
        let c : int list   = Microsoft.FSharp.Collections.List.Empty
        Assert.True( (c = []) )
        
        let d : string list = Microsoft.FSharp.Collections.List.Empty
        Assert.True( (d = []) )
        
        ()


    [<Fact>]
    member this.Cons() =
        // integer List
        let intList =  Microsoft.FSharp.Collections.List.Cons (1, [ 2;3; 4 ]) 
        if intList <> [ 1; 2; 3; 4 ] then Assert.Fail()
        
        // string List
        let strList = Microsoft.FSharp.Collections.List.Cons ( "this", [ "is";"str"; "list" ])
        if strList <> [ "this"; "is" ;"str"; "list" ] then Assert.Fail()

        // empty List
        let emptyList = Microsoft.FSharp.Collections.List.Cons (2,[])
        if emptyList <> [2] then Assert.Fail()
        ()


    [<Fact>] 
    member this.SlicingUnboundedEnd() = 
        let lst = [1;2;3;4;5;6]

        Assert.AreEqual(lst.[-1..], lst)
        Assert.AreEqual(lst.[0..], lst)
        Assert.AreEqual(lst.[1..], [2;3;4;5;6])
        Assert.AreEqual(lst.[2..], [3;4;5;6])
        Assert.AreEqual(lst.[5..], [6])
        Assert.AreEqual(lst.[6..], ([]: int list))

    
    [<Fact>] 
    member this.SlicingUnboundedStart() = 
        let lst = [1;2;3;4;5;6]

        Assert.AreEqual(lst.[..(-1)], ([]: int list))
        Assert.AreEqual(lst.[..0], [1])
        Assert.AreEqual(lst.[..1], [1;2])
        Assert.AreEqual(lst.[..2], [1;2;3])
        Assert.AreEqual(lst.[..3], [1;2;3;4])
        Assert.AreEqual(lst.[..4], [1;2;3;4;5])
        Assert.AreEqual(lst.[..5], [1;2;3;4;5;6])


    [<Fact>]
    member this.SlicingBoundedStartEnd() =
        let lst = [1;2;3;4;5;6]

        Assert.AreEqual(lst.[*], lst)

        Assert.AreEqual(lst.[0..0], [1])
        Assert.AreEqual(lst.[0..1], [1;2])
        Assert.AreEqual(lst.[0..2], [1;2;3])
        Assert.AreEqual(lst.[0..3], [1;2;3;4])
        Assert.AreEqual(lst.[0..4], [1;2;3;4;5])
        Assert.AreEqual(lst.[0..5], [1;2;3;4;5;6])

        Assert.AreEqual(lst.[1..1], [2])
        Assert.AreEqual(lst.[1..2], [2;3])
        Assert.AreEqual(lst.[1..3], [2;3;4])
        Assert.AreEqual(lst.[1..4], [2;3;4;5])
        Assert.AreEqual(lst.[1..5], [2;3;4;5;6])

        Assert.AreEqual(lst.[0..1], [1;2])
        Assert.AreEqual(lst.[1..1], [2])
        Assert.AreEqual(lst.[2..1], ([]: int list))
        Assert.AreEqual(lst.[3..1], ([]: int list))
        Assert.AreEqual(lst.[4..1], ([]: int list))


    [<Fact>]
    member this.SlicingEmptyList() = 

        let empty : obj list = List.empty
        Assert.AreEqual(empty.[*], ([]: obj list))
        Assert.AreEqual(empty.[5..3], ([]: obj list))
        Assert.AreEqual(empty.[0..], ([]: obj list))
        Assert.AreEqual(empty.[0..0], ([]: obj list))
        Assert.AreEqual(empty.[0..1], ([]: obj list))
        Assert.AreEqual(empty.[3..5], ([]: obj list))


    [<Fact>]
    member this.SlicingOutOfBounds() = 
        let lst = [1;2;3;4;5;6]
       
        Assert.AreEqual(lst.[..6], [1;2;3;4;5;6])
        Assert.AreEqual(lst.[6..], ([]: int list))

        Assert.AreEqual(lst.[0..(-1)], ([]: int list))
        Assert.AreEqual(lst.[1..(-1)], ([]: int list))
        Assert.AreEqual(lst.[1..0], ([]: int list))
        Assert.AreEqual(lst.[0..6], [1;2;3;4;5;6])
        Assert.AreEqual(lst.[1..6], [2;3;4;5;6])

        Assert.AreEqual(lst.[-1..1], [1;2])
        Assert.AreEqual(lst.[-3..(-4)], ([]: int list))
        Assert.AreEqual(lst.[-4..(-3)], ([]: int list))

