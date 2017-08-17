// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections.ISeqProperties

open System
open System.Collections.Generic
open NUnit.Framework
open FsCheck
open Utils

let sortByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs|> ISeq.ofSeq |> ISeq.indexed
    let sorted = indexed |> ISeq.sortBy snd
    isStable sorted

[<Test>]
let ``ISeq.sortBy is stable`` () =
    Check.QuickThrowOnFailure sortByStable<int>
    Check.QuickThrowOnFailure sortByStable<string>

let sortWithStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> ISeq.ofSeq |> ISeq.indexed |> Seq.toList
    let sorted = indexed |> ISeq.ofSeq |> ISeq.sortWith (fun x y -> compare (snd x) (snd y))
    isStable sorted
    
[<Test>]
let ``ISeq.sortWithStable is stable`` () =
    Check.QuickThrowOnFailure sortWithStable<int>
    Check.QuickThrowOnFailure sortWithStable<string>
    
let distinctByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs|> ISeq.ofSeq |> ISeq.indexed
    let sorted = indexed |> ISeq.distinctBy snd
    isStable sorted
    
[<Test>]
let ``ISeq.distinctBy is stable`` () =
    Check.QuickThrowOnFailure distinctByStable<int>
    Check.QuickThrowOnFailure distinctByStable<string>
