// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections.IConsumableSeqProperties

open System
open System.Collections.Generic
open NUnit.Framework
open FsCheck
open Utils

#if IConsumableSeqIsPublic

let sortByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs|> IConsumableSeq.ofSeq |> IConsumableSeq.indexed
    let sorted = indexed |> IConsumableSeq.sortBy snd
    isStable sorted

[<Test>]
let ``IConsumableSeq.sortBy is stable`` () =
    Check.QuickThrowOnFailure sortByStable<int>
    Check.QuickThrowOnFailure sortByStable<string>

let sortWithStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> IConsumableSeq.ofSeq |> IConsumableSeq.indexed |> Seq.toList
    let sorted = indexed |> IConsumableSeq.ofSeq |> IConsumableSeq.sortWith (fun x y -> compare (snd x) (snd y))
    isStable sorted
    
[<Test>]
let ``IConsumableSeq.sortWithStable is stable`` () =
    Check.QuickThrowOnFailure sortWithStable<int>
    Check.QuickThrowOnFailure sortWithStable<string>
    
let distinctByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs|> IConsumableSeq.ofSeq |> IConsumableSeq.indexed
    let sorted = indexed |> IConsumableSeq.distinctBy snd
    isStable sorted
    
[<Test>]
let ``IConsumableSeq.distinctBy is stable`` () =
    Check.QuickThrowOnFailure distinctByStable<int>
    Check.QuickThrowOnFailure distinctByStable<string>

#endif