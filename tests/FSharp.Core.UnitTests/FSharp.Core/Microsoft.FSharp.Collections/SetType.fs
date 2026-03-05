// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Collections.Set type

namespace FSharp.Core.UnitTests.Collections

open System
open System.Collections
open System.Collections.Generic
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit


/// This type is used to verify that Set.intersect preserves the object identity (reference) 
/// of the first set (S1). By overriding equality to only check the 'Id' field, 
/// we can create two objects that are "equal" but live in different memory locations.
[<CustomEquality; CustomComparison>]
type IntersectItem = 
    { Id: int; Tag: string }
    override x.Equals(obj) =
        match obj with
        | :? IntersectItem as other -> x.Id = other.Id
        | _ -> false
    override x.GetHashCode() = hash x.Id
    interface IComparable with
        member x.CompareTo(obj) =
            match obj with
            | :? IntersectItem as other -> compare x.Id other.Id
            | _ -> invalidArg "obj" "not an IntersectItem"

(*
[Test Strategy]
Make sure each method works on:
* Empty set
* Single-element set
* Sets with 4 or more elements
*)

type SetType() =

    // Interfaces
    [<Fact>]
    member this.IEnumerable() =
        // Legit IE
        let ie = (new Set<char>(['a'; 'b'; 'c'])) :> IEnumerable
        //let alphabet = new Set<char>([| 'a' .. 'z' |])
        let enum = ie.GetEnumerator()

        let testStepping() =
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'a')
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'b')
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'c')
            Assert.AreEqual(enum.MoveNext(), false)
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
    
        testStepping()
        enum.Reset()
        testStepping()
    
        // Empty IE
        let ie = (new Set<char>([])) :> IEnumerable  // Note no type args
        let enum = ie.GetEnumerator()
        
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        Assert.AreEqual(enum.MoveNext(), false)
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)

    [<Fact>]
    member this.IEnumerable_T() =        
        // Legit IE
        let ie =(new Set<char>(['a'; 'b'; 'c'])) :> IEnumerable<char>
        let enum = ie.GetEnumerator()
        
        let testStepping() =
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'a')
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'b')
            Assert.AreEqual(enum.MoveNext(), true)
            Assert.AreEqual(enum.Current, 'c')
            Assert.AreEqual(enum.MoveNext(), false)
            CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        
        testStepping()
        enum.Reset()
        testStepping()
    
        // Empty IE
        let ie = (new Set<int>([])) :> IEnumerable<int>  
        let enum = ie.GetEnumerator()
        
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        Assert.AreEqual(enum.MoveNext(), false)
        CheckThrowsInvalidOperationExn(fun () -> enum.Current |> ignore)
        
        
    [<Fact>]
    member this.ICollection() =        
        // Legit IC        
        let ic = (new Set<int>([1;2;3;4])) :> ICollection<int>
        let st = new Set<int>([1;2;3;4])        
        
        Assert.True(ic.Contains(3)) 
        let newArr = Array.create 5 0
        ic.CopyTo(newArr,0) 
        Assert.True(ic.IsReadOnly)       
            
        // Empty IC
        let ic = (new Set<string>([])) :> ICollection<string>
        Assert.False(ic.Contains("A") )     
        let newArr = Array.create 5 "a"
        ic.CopyTo(newArr,0) 

    [<Fact>]
    member this.IReadOnlyCollection() =        
        // Legit IROC
        let iroc = (new Set<int>([1;2;3;4])) :> IReadOnlyCollection<int>
        Assert.AreEqual(iroc.Count, 4)       
            
        // Empty IROC
        let iroc = (new Set<string>([])) :> IReadOnlyCollection<string>
        Assert.AreEqual(iroc.Count, 0)
    
    [<Fact>]
    member this.IComparable() =        
        // Legit IC
        let ic = (new Set<int>([1;2;3;4])) :> IComparable    
        Assert.AreEqual(ic.CompareTo(new Set<int>([1;2;3;4])),0) 
        
        // Empty IC
        let ic = (new Set<string>([])) :> IComparable   
        Assert.AreEqual(ic.CompareTo(Set.empty<string>),0)
        
        
    // Base class methods
    [<Fact>]
    member this.ObjectGetHashCode() =
        // Verify order added is independent
        let x = Set.ofList [1; 2; 3]
        let y = Set.ofList [3; 2; 1]
        Assert.AreEqual(x.GetHashCode(), y.GetHashCode())
    
    [<Fact>]
    member this.ObjectToString() =
        Assert.AreEqual("set [1; 2; 3; ... ]", (new Set<int>([1;2;3;4])).ToString())
        Assert.AreEqual("set []", (Set.empty).ToString())
        Assert.AreEqual("set [1; 3]", (new Set<decimal>([1M;3M])).ToString())
        
    
    [<Fact>]
    member this.ObjectEquals() =
        // All three are different references, but equality has been
        // provided by the F# compiler.
        let a = new Set<int>([1;2;3])
        let b = new Set<int>([1..3])
        let c = new Set<int>(seq{1..3})
        Assert.True( (a = b) )
        Assert.True( (b = c) )
        Assert.True( (c = a) )
        Assert.True( a.Equals(b) ); Assert.True( b.Equals(a) )
        Assert.True( b.Equals(c) ); Assert.True( c.Equals(b) )
        Assert.True( c.Equals(a) ); Assert.True( a.Equals(c) )

        // Equality between types
        let a = Set.empty<int>
        let b = Set.empty<string>
        Assert.False( b.Equals(a) )
        Assert.False( a.Equals(b) )
        
        // Co/contra variance not supported
        let a = Set.empty<string>
        let b = Set.empty
        Assert.False(a.Equals(b))
        Assert.False(b.Equals(a))
        
        // Self equality
        let a = new Set<int>([1])
        Assert.True( (a = a) )
        Assert.True(a.Equals(a))
        
        // Null
        Assert.False(a.Equals(null))  
        
        
    // Instance methods
    [<Fact>]
    member this.Add() =    
        let l = new Set<int>([1 .. 10])
        let ad = l.Add 88
        Assert.True(ad.Contains(88))
    
        let e : Set<string> = Set.empty<string>
        let ade = e.Add "A"
        Assert.True(ade.Contains("A"))
        
        let s = Set.singleton 168
        let ads = s.Add 100
        Assert.True(ads.Contains(100))
        
    [<Fact>]
    member this.Contains() =    
        let i = new Set<int>([1 .. 10])
        Assert.True(i.Contains(8))
    
        let e : Set<string> = Set.empty<string>
        Assert.False(e.Contains("A"))
        
        let s = Set.singleton 168
        Assert.True(s.Contains(168))
    
    [<Fact>]
    member this.Count() =    
        let l = new Set<int>([1 .. 10])
        Assert.AreEqual(l.Count, 10)
    
        let e : Set<string> = Set.empty<string>
        Assert.AreEqual(e.Count, 0)
        
        let s = Set.singleton 'a'
        Assert.AreEqual(s.Count, 1)        
        
    [<Fact>]
    member this.IsEmpty() =
        let i = new Set<int>([1 .. 10])
        Assert.False(i.IsEmpty)
    
        let e : Set<string> = Set.empty<string>
        Assert.True(e.IsEmpty)
        
        let s = Set.singleton 168
        Assert.False(s.IsEmpty)   
        
    [<Fact>]
    member this.IsSubsetOf() =
        let fir = new Set<int>([1 .. 20])
        let sec = new Set<int>([1 .. 10])
        Assert.True(sec.IsSubsetOf(fir))
        Assert.True(Set.isSubset sec fir)
    
        let e : Set<int> = Set.empty<int>
        Assert.True(e.IsSubsetOf(fir))
        Assert.True(Set.isSubset e fir)
        
        let s = Set.singleton 8
        Assert.True(s.IsSubsetOf(fir)) 
        Assert.True(Set.isSubset s fir)
        
        let s100 = set [0..100]
        let s101 = set [0..101]
        for i = 0 to 100 do 
            Assert.False( (set [-1..i]).IsSubsetOf s100)
            Assert.True( (set [0..i]).IsSubsetOf s100)
            Assert.True( (set [0..i]).IsProperSubsetOf s101)
           
        
    [<Fact>]
    member this.IsSupersetOf() =
        let fir = new Set<int>([1 .. 10])
        let sec = new Set<int>([1 .. 20])
        Assert.True(sec.IsSupersetOf(fir))
        Assert.True(Set.isSuperset sec fir)
    
        let e : Set<int> = Set.empty<int>
        Assert.False(e.IsSupersetOf(fir))
        Assert.False(Set.isSuperset e fir)
        
        let s = Set.singleton 168
        Assert.False(s.IsSupersetOf(fir))  
        Assert.False(Set.isSuperset s fir)

        let s100 = set [0..100]
        let s101 = set [0..101]
        for i = 0 to 100 do 
            Assert.False( s100.IsSupersetOf (set [-1..i]))
            Assert.True( s100.IsSupersetOf (set [0..i]))
            Assert.True( s101.IsSupersetOf (set [0..i]))
        
    [<Fact>]
    member this.Remove() =    
        let i = new Set<int>([1;2;3;4])
        Assert.AreEqual(i.Remove 3,(new Set<int>([1;2;4])))
    
        let e : Set<string> = Set.empty<string>
        Assert.AreEqual(e.Remove "A", e)
        
        let s = Set.singleton 168
        Assert.AreEqual(s.Remove 168, Set.empty<int>) 
        
        
    // Static methods
    [<Fact>]
    member this.Addition() =
        let fir = new Set<int>([1;3;5])
        let sec = new Set<int>([2;4;6])
        Assert.AreEqual(fir + sec, new Set<int>([1;2;3;4;5;6]))
        Assert.AreEqual(Set.op_Addition(fir,sec), new Set<int>([1;2;3;4;5;6]))
    
        let e : Set<int> = Set.empty<int>
        Assert.AreEqual(e + e, e)
        Assert.AreEqual(Set.op_Addition(e,e),e)
        
        let s1 = Set.singleton 8
        let s2 = Set.singleton 6
        Assert.AreEqual(s1 + s2, new Set<int>([8;6]))
        Assert.AreEqual(Set.op_Addition(s1,s2), new Set<int>([8;6]))
        

    [<Fact>]
    member this.Subtraction() =
        let fir = new Set<int>([1..6])
        let sec = new Set<int>([2;4;6])
        Assert.AreEqual(fir - sec, new Set<int>([1;3;5]))
        Assert.AreEqual(Set.difference fir sec, new Set<int>([1;3;5]))
        Assert.AreEqual(Set.op_Subtraction(fir,sec), new Set<int>([1;3;5]))
    
        let e : Set<int> = Set.empty<int>
        Assert.AreEqual(e - e, e)
        Assert.AreEqual(Set.difference e e, e)
        Assert.AreEqual(Set.op_Subtraction(e,e),e)
        
        let s1 = Set.singleton 8
        let s2 = Set.singleton 6
        Assert.AreEqual(s1 - s2, new Set<int>([8]))
        Assert.AreEqual(Set.difference s1 s2, new Set<int>([8]))
        Assert.AreEqual(Set.op_Subtraction(s1,s2), new Set<int>([8]))
        

    [<Fact>]
    member this.MinimumElement() =
        let fir = new Set<int>([1..6])
        let sec = new Set<int>([2;4;6])
        Assert.AreEqual(fir.MinimumElement, 1)
        Assert.AreEqual(sec.MinimumElement, 2)
        Assert.AreEqual(Set.minElement fir, 1)
        Assert.AreEqual(Set.minElement sec, 2)
        

    [<Fact>]
    member this.MaximumElement() =
        let fir = new Set<int>([1..6])
        let sec = new Set<int>([2;4;7])
        Assert.AreEqual(fir.MaximumElement, 6)
        Assert.AreEqual(sec.MaximumElement, 7)
        Assert.AreEqual(Set.maxElement fir, 6)
        Assert.AreEqual(Set.maxElement sec, 7)


    [<Fact>]
    member _.Intersect_PreservesIdentity() =
        // 1. [Test Strategy] Empty set
        let emptyA : Set<IntersectItem> = Set.empty
        let emptyB : Set<IntersectItem> = Set.empty
        Assert.True((Set.intersect emptyA emptyB).IsEmpty)
        Assert.True((Set.intersect (Set.singleton {Id=1; Tag="x"}) emptyB).IsEmpty)

        // 2. [Test Strategy] Single-element set & [Identity Property]
        let item1_S1 = { Id = 1; Tag = "From_S1" }
        let item1_S2 = { Id = 1; Tag = "From_S2" }
        
        let s1_single = Set.singleton item1_S1
        let s2_single = Set.singleton item1_S2
        
        let resSingle = Set.intersect s1_single s2_single
        Assert.Equal(1, resSingle.Count)
        
        // Identity Check: Must be EXACTLY the same object in memory as S1
        let singleResult = Set.minElement resSingle
        Assert.True(Object.ReferenceEquals(singleResult, item1_S1), "Single-element identity failed: Reference must come from S1")

        // 3. [Test Strategy] Sets with 4 or more elements (Optimized Path Check)
        let item5_S1 = { Id = 5; Tag = "ID5_From_S1" }
        let item5_S2 = { Id = 5; Tag = "ID5_From_S2" }

        // S1 is large: hits intersectionAuxFromSmall (The optimized path)
        let list1 = [ for i in 1..50 -> if i = 5 then item5_S1 else { Id = i; Tag = sprintf "L%d" i } ]
        let list2 = [ for i in 1..5 -> if i = 5 then item5_S2 else { Id = i; Tag = sprintf "R%d" i } ]
        
        let s1_large = Set.ofList list1
        let s2_small = Set.ofList list2
        
        let resMany = Set.intersect s1_large s2_small
        
        // Identity Check in Loop: 
        // We iterate through the result and ensure every common element 
        // has the physical reference from the first set (list1)
        resMany |> Set.iter (fun item ->
            let originalInS1 = list1 |> List.find (fun x -> x.Id = item.Id)
            if not (Object.ReferenceEquals(item, originalInS1)) then
                Assert.Fail(sprintf "Identity mismatch for ID %d: Expected reference from S1 (list1), but got another instance." item.Id)
        )

        // 4. [Reversed Size Case] (Standard Path Check)
        // S1 is small, S2 is large: hits intersectionAux (The standard path)
        let resReversed = Set.intersect s2_small s1_large
        
        // Find the specific item with ID 5 in the result
        let item5InResult = resReversed |> Set.filter (fun x -> x.Id = 5) |> Set.minElement
        
        // Identity Check: Since s2_small was the FIRST argument, 
        // the result MUST contain item5_S2 (from s2_small)
        Assert.True(Object.ReferenceEquals(item5InResult, item5_S2), "Reversed path identity failed: Reference must come from s2_small")

#if NET8_0_OR_GREATER

#nowarn "1204" // FS1204: This type/method is for compiler use and should not be used directly.

/// Tests for methods on the static, non-generic Set type.
module FSharpSet =
    [<Fact>]
    let ``Set.Create creates a set from a ReadOnlySpan`` () =
        let expected = set [1..10]
        let span = ReadOnlySpan [|1..10|]
        let actual = Set.Create span
        Assert.Equal<Set<int>>(expected, actual)
#endif
