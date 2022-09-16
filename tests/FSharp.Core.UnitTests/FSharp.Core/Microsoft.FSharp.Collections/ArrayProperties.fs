// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Core.UnitTests.Collections

open System
open System.Collections.Generic
open System.Reflection
open Xunit
open FsCheck
open Utils

type ArrayProperties() =
    inherit TestClassWithSimpleNameAppDomainResolver()

    member _.isStable sorted = sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

    member this.distinctByStable<'a when 'a : comparison> (xs : 'a array) =
       let indexed = xs |> Seq.indexed |> Seq.toArray
       let sorted = indexed |> Array.distinctBy snd
       this.isStable sorted

    member _.blitWorksLikeCopy<'a when 'a : comparison> (source : 'a array, sourceIndex, target : 'a array, targetIndex, count) =
        let target1 = Array.copy target
        let target2 = Array.copy target
        let a = runAndCheckIfAnyError (fun () -> Array.blit source sourceIndex target1 targetIndex count)
        let b = runAndCheckIfAnyError (fun () -> Array.Copy(source, sourceIndex, target2, targetIndex, count))
        a = b && target1 = target2
 
    [<Fact>]
    member this.``Array.distinctBy is stable`` () =
        Check.QuickThrowOnFailure this.distinctByStable<int>
        Check.QuickThrowOnFailure this.distinctByStable<string>
 
    [<Fact>]
    member this.``Array.blit works like Array.Copy`` () =
        Check.QuickThrowOnFailure this.blitWorksLikeCopy<int>
        Check.QuickThrowOnFailure this.blitWorksLikeCopy<string>
