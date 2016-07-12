// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections.ArrayProperties

open System
open System.Collections.Generic
open NUnit.Framework
open FsCheck

let isStable sorted = sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)
    
let distinctByStable<'a when 'a : comparison> (xs : 'a []) =
    let indexed = xs |> Seq.indexed |> Seq.toArray
    let sorted = indexed |> Array.distinctBy snd
    isStable sorted
    
[<Test>]
let ``Array.distinctBy is stable`` () =
    Check.QuickThrowOnFailure distinctByStable<int>
    Check.QuickThrowOnFailure distinctByStable<string>
    
let blitWorksLikeCopy<'a when 'a : comparison> (source : 'a [], sourceIndex, target : 'a [], targetIndex, count) =
    let a = runAndCheckIfAnyError (fun () -> Array.blit source sourceIndex target targetIndex count)
    let b = runAndCheckIfAnyError (fun () -> Array.Copy(source, sourceIndex, target, targetIndex, count))
    a = b
    
[<Test>]
let ``Array.blit works like Array.Copy`` () =
    Check.QuickThrowOnFailure blitWorksLikeCopy<int>
    Check.QuickThrowOnFailure blitWorksLikeCopy<string>
