// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Lists

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

let chunkBySize_and_collect<'a when 'a : equality> (xs : 'a list) size =
    size > 0 ==> (lazy
        let a = List.chunkBySize size xs
        let b = List.collect id a
        b = xs)

[<Test>]
let ``chunkBySize is reversable with collect`` () =
    Check.QuickThrowOnFailure chunkBySize_and_collect<int>
    Check.QuickThrowOnFailure chunkBySize_and_collect<string>
    Check.QuickThrowOnFailure chunkBySize_and_collect<NormalFloat>

[<Test>]
let ``chunkBySize produces chunks exactly of size `chunkSize`, except the last one, which can be smaller, but not empty``() =
    let prop (a: _ list) (PositiveInt chunkSize) =   
        match a |> List.chunkBySize chunkSize |> Seq.toList with
        | [] -> a = []
        | h :: [] -> h.Length <= chunkSize
        | chunks ->
            let lastChunk = chunks |> List.last
            let headChunks = chunks |> Seq.take (chunks.Length - 1) |> Seq.toList

            headChunks |> List.forall (List.length >> (=) chunkSize)
            &&
            lastChunk <> []
            &&
            lastChunk.Length <= chunkSize

    Check.QuickThrowOnFailure prop

let splitInto_and_collect<'a when 'a : equality> (xs : 'a list) count =
    count > 0 ==> (lazy
        let a = List.splitInto count xs
        let b = List.collect id a
        b = xs)

[<Test>]
let ``splitInto is reversable with collect`` () =
    Check.QuickThrowOnFailure splitInto_and_collect<int>
    Check.QuickThrowOnFailure splitInto_and_collect<string>
    Check.QuickThrowOnFailure splitInto_and_collect<NormalFloat>


[<Test>]
let ``splitInto produces chunks exactly `count` chunks with equal size (+/- 1)``() =
    let prop (a: _ list) (PositiveInt count') =
        let count = min a.Length count'
        match a |> List.splitInto count' |> Seq.toList with
        | [] -> a = []
        | h :: [] -> (a.Length = 1 || count = 1) && h = a
        | chunks ->
            let lastChunk = chunks |> List.last
            let lastLength = lastChunk |> List.length

            chunks.Length = count
            &&
            chunks |> List.forall (fun c -> List.length c = lastLength || List.length c = lastLength + 1)

    Check.QuickThrowOnFailure prop

let sort_and_sortby (xs : list<float>) (xs2 : list<float>) =
    let a = List.sortBy id xs |> Seq.toArray 
    let b = List.sort xs |> Seq.toArray
    let result = ref true
    for i in 0 .. a.Length - 1 do
        if a.[i] <> b.[i] then
            if System.Double.IsNaN a.[i] <> System.Double.IsNaN b.[i] then
                result := false
    !result 

[<Test>]
let ``sort behaves like sortby id`` () =   
    Check.QuickThrowOnFailure sort_and_sortby

let filter_and_except<'a when 'a : comparison>  (xs : list<'a>) (itemsToExclude : Set<'a>) =
    let a = List.filter (fun x -> Set.contains x itemsToExclude |> not) xs |> List.distinct
    let b = List.except itemsToExclude xs
    a = b

[<Test>]
let ``filter and except work similar`` () =   
    Check.QuickThrowOnFailure filter_and_except<int>
    Check.QuickThrowOnFailure filter_and_except<string>
    Check.QuickThrowOnFailure filter_and_except<NormalFloat>


let distinct_works_like_set<'a when 'a : comparison> (xs : 'a list) =
    let a = List.distinct xs
    let b = Set.ofList xs

    let result = ref (a.Length = b.Count)
    for x in a do
        if Set.contains x b |> not then
            result := false

    for x in b do
        if List.exists ((=) x) a |> not then
            result := false
    !result

[<Test>]
let ``distinct creates same elements like a set`` () =
    Check.QuickThrowOnFailure distinct_works_like_set<int>
    Check.QuickThrowOnFailure distinct_works_like_set<string>
    Check.QuickThrowOnFailure distinct_works_like_set<NormalFloat>

let isStable sorted = sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let sortByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toList
    let sorted = indexed |> List.sortBy snd
    isStable sorted
    
[<Test>]
let ``List.sortBy is stable`` () =
    Check.QuickThrowOnFailure sortByStable<int>
    Check.QuickThrowOnFailure sortByStable<string>

let distinctByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toList
    let sorted = indexed |> List.distinctBy snd
    isStable sorted
    
[<Test>]
let ``List.distinctBy is stable`` () =
    Check.QuickThrowOnFailure distinctByStable<int>
    Check.QuickThrowOnFailure distinctByStable<string>