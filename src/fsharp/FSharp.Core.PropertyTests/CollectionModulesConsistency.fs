// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.CollectionModulesConsistency

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

let append<'a when 'a : equality> (xs : list<'a>) (xs2 : list<'a>) =
    let s = xs |> Seq.append xs2 
    let l = xs |> List.append xs2
    let a = xs |> Seq.toArray |> Array.append (Seq.toArray xs2)
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``append is consistent`` () =
    Check.QuickThrowOnFailure append<int>
    Check.QuickThrowOnFailure append<string>
    Check.QuickThrowOnFailure append<NormalFloat>

let contains<'a when 'a : equality> (xs : 'a []) x  =
    let s = xs |> Seq.contains x
    let l = xs |> List.ofArray |> List.contains x
    let a = xs |> Array.contains x
    s = a && l = a

[<Test>]
let ``contains is consistent`` () =
    Check.QuickThrowOnFailure contains<int>
    Check.QuickThrowOnFailure contains<string>
    Check.QuickThrowOnFailure contains<float>

let choose<'a when 'a : equality> (xs : 'a []) f  =
    let s = xs |> Seq.choose f
    let l = xs |> List.ofArray |> List.choose f
    let a = xs |> Array.choose f
    Seq.toArray s = a &&  List.toArray l = a

[<Test>]
let ``choose is consistent`` () =
    Check.QuickThrowOnFailure contains<int>
    Check.QuickThrowOnFailure contains<string>
    Check.QuickThrowOnFailure contains<float>

let collect<'a> (xs : 'a []) f  =
    let s = xs |> Seq.collect f
    let l = xs |> List.ofArray |> List.collect (fun x -> f x |> List.ofArray)
    let a = xs |> Array.collect f
    Seq.toArray s = a &&  List.toArray l = a

[<Test>]
let ``collect is consistent`` () =
    Check.QuickThrowOnFailure collect<int>
    Check.QuickThrowOnFailure collect<string>
    Check.QuickThrowOnFailure collect<float>

let compareWith<'a>(xs : 'a []) (xs2 : 'a []) f  =
    let s = (xs, xs2) ||> Seq.compareWith f
    let l = (List.ofArray xs, List.ofArray xs2) ||> List.compareWith f
    let a = (xs, xs2) ||> Array.compareWith f
    s = a && l = a

[<Test>]
let ``compareWith is consistent`` () =
    Check.QuickThrowOnFailure compareWith<int>
    Check.QuickThrowOnFailure compareWith<string>
    Check.QuickThrowOnFailure compareWith<float>
        
let concat<'a when 'a : equality> (xs : 'a [][]) =
    let s = xs |> Seq.concat
    let l = xs |> List.ofArray |> List.map List.ofArray |> List.concat
    let a = xs |> Array.concat
    Seq.toArray s = a &&  List.toArray l = a

[<Test>]
let ``concat is consistent`` () =
    Check.QuickThrowOnFailure concat<int>
    Check.QuickThrowOnFailure concat<string>
    Check.QuickThrowOnFailure concat<NormalFloat>

let countBy<'a> (xs : 'a []) f =
    let s = xs |> Seq.countBy f
    let l = xs |> List.ofArray |> List.countBy f
    let a = xs |> Array.countBy f
    Seq.toArray s = a &&  List.toArray l = a

[<Test>]
let ``countBy is consistent`` () =
    Check.QuickThrowOnFailure countBy<int>
    Check.QuickThrowOnFailure countBy<string>
    Check.QuickThrowOnFailure countBy<float>
    