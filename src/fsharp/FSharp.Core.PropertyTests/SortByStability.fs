// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.SortByStability

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

let sortByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed
    let sorted = indexed |> Seq.sortBy snd
    sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let sortByStableArray<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toArray
    let sorted = indexed |> Array.sortBy snd
    sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let sortByStableList<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toList
    let sorted = indexed |> List.sortBy snd
    sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let sortByStableInt xs = sortByStable<int> xs
let sortByStableArrayInt xs = sortByStableArray<int> xs
let sortByStableListInt xs = sortByStableList<int> xs

[<Test>]
let ``sortBy stability`` () = 
    Check.QuickThrowOnFailure(sortByStableInt)
    Check.QuickThrowOnFailure(sortByStableListInt)
    Check.QuickThrowOnFailure(sortByStableArrayInt)
