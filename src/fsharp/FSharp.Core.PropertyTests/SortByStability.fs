// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.SortByStability

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

let isStable sorted = sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let sortByStableSeq<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed
    let sorted = indexed |> Seq.sortBy snd
    isStable sorted

let sortByStableList<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toList
    let sorted = indexed |> List.sortBy snd
    isStable sorted

[<Test>]
let ``Seq.sortBy is stable`` () =
    Check.QuickThrowOnFailure sortByStableSeq<int>
    Check.QuickThrowOnFailure sortByStableSeq<string>
    
[<Test>]
let ``List.sortBy is stable`` () =
    Check.QuickThrowOnFailure sortByStableList<int>
    Check.QuickThrowOnFailure sortByStableSeq<string>