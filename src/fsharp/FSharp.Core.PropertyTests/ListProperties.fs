// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Lists

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

  
let ListContains<'a when 'a : equality> (xs : list<'a>) x =
    x :: xs |> List.contains x
        
let ListDistinct<'a when 'a : equality> (xs : list<'a>) =
    List.distinct xs |> List.countBy id |> List.forall (snd >> (=) 1)

let ListDistinctBy<'a when 'a : equality> (xs : list<'a>) =
    List.distinctBy id xs |> List.countBy id |> List.forall (snd >> (=) 1)

let ListIndexed<'a> (xs : list<'a>) =
    let indices = List.indexed xs |> List.map fst
    indices = (indices |> List.sort) && 
    indices |> List.pairwise |> List.forall (fun (a,b) -> b - a = 1)

let ListPairwise<'a when 'a : equality> (xs : list<'a>) =
    xs.Length > 1 ==> lazy
        (xs |> List.pairwise = ((xs |> List.rev |> List.tail |> List.rev, xs |> List.skip 1) ||> List.zip))

let ListSplitAt<'a> (xs : list<'a>) index =
    (index >= 0 && index < xs.Length) ==> lazy
    let a,b = xs |> List.splitAt index
    a.Length = index && b.Length = xs.Length - a.Length
        
let ListSplitInto<'a when 'a : equality> (xs : list<'a>) count =
    (count > 0) ==> lazy
    let xss = xs |> List.splitInto count
    let lengths = xss |> List.map (fun x -> x.Length)
    (xss.Length <= count) && (List.concat xss = xs) && 
    (if xss.Length > 0 then List.max lengths - List.min lengths <= 1 else true)
        
let ListMapFold<'a> (xs : list<'a>) mapF foldF state =
    let mapFoldF state next =
        mapF next, foldF state next
    let result, state' = xs |> List.mapFold mapFoldF state
    result = (xs |> List.map mapF) &&
    state' = (xs |> List.fold foldF state)

type ListProperties =
//    static member ``Reverse of a reverse of a int list gives the original list``(xs:list<int>) = 
//        List.rev(xs) = xs
      
    static member ListContainsInt xs x = ListContains<int> xs x
    static member ListDistinctInt xs = ListDistinct<int> xs
    static member ListDistinctByInt xs = ListDistinctBy<int> xs
    static member ListIndexedInt xs = ListIndexed<int> xs
    static member ListPairwiseInt xs = ListPairwise<int> xs
    static member ListSplitAtInt xs x = ListSplitAt<int> xs x
    static member ListSplitIntoInt xs x = ListSplitInto<int> xs x
    static member ListMapFoldInt xs mapF foldF state = ListMapFold<int> xs mapF foldF state

    static member ListContainsString xs x = ListContains<string> xs x
    static member ListDistinctString xs = ListDistinct<string> xs
    static member ListDistinctByString xs = ListDistinctBy<string> xs
    static member ListIndexedString xs = ListIndexed<string> xs
    static member ListPairwiseString xs = ListPairwise<string> xs
    static member ListSplitAtString xs x = ListSplitAt<string> xs x
    static member ListSplitIntoString xs x = ListSplitInto<string> xs x
    static member ListMapFoldString xs mapF foldF state = ListMapFold<string> xs mapF foldF state

    static member ListContainsFloat xs x = ListContains<NormalFloat> xs x
    static member ListDistinctFloat xs = ListDistinct<float> xs
    static member ListDistinctByFloat xs = ListDistinctBy<float> xs
    static member ListIndexedFloat xs = ListIndexed<float> xs
   // static member ListPairwiseFloat xs = ListPairwise<float> xs
    static member ListSplitAtFloat xs x = ListSplitAt<float> xs x
   // static member ListSplitIntoFloat xs x = ListSplitInto<float> xs x
    static member ListMapFoldFloat xs mapF foldF state = ListMapFold<float> xs mapF foldF state


[<Test>]
let ``List properties`` () =
    Check.QuickThrowOnFailureAll<ListProperties>()