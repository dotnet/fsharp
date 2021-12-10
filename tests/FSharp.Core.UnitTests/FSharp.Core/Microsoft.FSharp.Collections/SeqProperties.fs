// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Core.UnitTests.Collections

open System
open System.Collections.Generic
open Xunit
open FsCheck
open Utils

type SeqProperties () =
    inherit TestClassWithSimpleNameAppDomainResolver ()

    member _.sortByStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed
        let sorted = indexed |> Seq.sortBy snd
        isStable sorted

    [<Fact>]
    member this.``Seq.sortBy is stable`` () =
        Check.QuickThrowOnFailure this.sortByStable<int>
        Check.QuickThrowOnFailure this.sortByStable<string>

    member _.sortWithStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed |> Seq.toList
        let sorted = indexed |> Seq.sortWith (fun x y -> compare (snd x) (snd y))
        isStable sorted
    
    [<Fact>]
    member this.``Seq.sortWithStable is stable`` () =
        Check.QuickThrowOnFailure this.sortWithStable<int>
        Check.QuickThrowOnFailure this.sortWithStable<string>
    
    member _.distinctByStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed
        let sorted = indexed |> Seq.distinctBy snd
        isStable sorted
    
    [<Fact>]
    member this.``Seq.distinctBy is stable`` () =
        Check.QuickThrowOnFailure this.distinctByStable<int>
        Check.QuickThrowOnFailure this.distinctByStable<string>
