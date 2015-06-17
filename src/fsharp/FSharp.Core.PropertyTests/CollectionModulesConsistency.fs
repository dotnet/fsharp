// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.CollectionModulesConsistency

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

type Result<'a> = 
| Success of 'a
| Error of string

let run f = 
    try
        Success(f())
    with
    | exn -> Error(exn.Message)

let runAndCheckErrorType f = 
    try
        Success(f())
    with
    | exn -> Error(exn.GetType().ToString())

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

let averageFloat (xs : NormalFloat []) =
    let xs = xs |> Array.map float
    let s = run (fun () -> xs |> Seq.average)
    let l = run (fun () -> xs |> List.ofArray |> List.average)
    let a = run (fun () -> xs |> Array.average)
    s = a && l = a

[<Test>]
let ``average is consistent`` () =
    Check.QuickThrowOnFailure averageFloat

let averageBy (xs : float []) f =
    let xs = xs |> Array.map float
    let f x = (f x : NormalFloat) |> float
    let s = run (fun () -> xs |> Seq.averageBy f)
    let l = run (fun () -> xs |> List.ofArray |> List.averageBy f)
    let a = run (fun () -> xs |> Array.averageBy f)
    s = a && l = a

[<Test>]
let ``averageBy is consistent`` () =
    Check.QuickThrowOnFailure averageBy

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
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``choose is consistent`` () =
    Check.QuickThrowOnFailure contains<int>
    Check.QuickThrowOnFailure contains<string>
    Check.QuickThrowOnFailure contains<float>

let chunkBySize<'a when 'a : equality> (xs : 'a []) size =
    let s = run (fun () -> xs |> Seq.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    let a = run (fun () -> xs |> Array.chunkBySize size |> Seq.map Seq.toArray |> Seq.toArray)
    s = a && l = a

[<Test>]
let ``chunkBySize is consistent`` () =
    Check.QuickThrowOnFailure chunkBySize<int>
    Check.QuickThrowOnFailure chunkBySize<string>
    Check.QuickThrowOnFailure chunkBySize<NormalFloat>

let collect<'a> (xs : 'a []) f  =
    let s = xs |> Seq.collect f
    let l = xs |> List.ofArray |> List.collect (fun x -> f x |> List.ofArray)
    let a = xs |> Array.collect f
    Seq.toArray s = a && List.toArray l = a

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
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``concat is consistent`` () =
    Check.QuickThrowOnFailure concat<int>
    Check.QuickThrowOnFailure concat<string>
    Check.QuickThrowOnFailure concat<NormalFloat>

let countBy<'a> (xs : 'a []) f =
    let s = xs |> Seq.countBy f
    let l = xs |> List.ofArray |> List.countBy f
    let a = xs |> Array.countBy f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``countBy is consistent`` () =
    Check.QuickThrowOnFailure countBy<int>
    Check.QuickThrowOnFailure countBy<string>
    Check.QuickThrowOnFailure countBy<float>

let distinct<'a when 'a : comparison> (xs : 'a []) =
    let s = xs |> Seq.distinct 
    let l = xs |> List.ofArray |> List.distinct
    let a = xs |> Array.distinct
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``distinct is consistent`` () =
    Check.QuickThrowOnFailure distinct<int>
    Check.QuickThrowOnFailure distinct<string>
    Check.QuickThrowOnFailure distinct<NormalFloat>

let distinctBy<'a when 'a : equality> (xs : 'a []) f =
    let s = xs |> Seq.distinctBy f
    let l = xs |> List.ofArray |> List.distinctBy f
    let a = xs |> Array.distinctBy f
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``distinctBy is consistent`` () =
    Check.QuickThrowOnFailure distinctBy<int>
    Check.QuickThrowOnFailure distinctBy<string>
    Check.QuickThrowOnFailure distinctBy<NormalFloat>

let exactlyOne<'a when 'a : comparison> (xs : 'a []) =
    let s = runAndCheckErrorType (fun () -> xs |> Seq.exactlyOne)
    let l = runAndCheckErrorType (fun () -> xs |> List.ofArray |> List.exactlyOne)
    let a = runAndCheckErrorType (fun () -> xs |> Array.exactlyOne)
    s = a && l = a

[<Test>]
let ``exactlyOne is consistent`` () =
    Check.QuickThrowOnFailure exactlyOne<int>
    Check.QuickThrowOnFailure exactlyOne<string>
    Check.QuickThrowOnFailure exactlyOne<NormalFloat>

let except<'a when 'a : equality> (xs : 'a []) (itemsToExclude: 'a []) =
    let s = xs |> Seq.except itemsToExclude
    let l = xs |> List.ofArray |> List.except itemsToExclude
    let a = xs |> Array.except itemsToExclude
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``except is consistent`` () =
    Check.QuickThrowOnFailure except<int>
    Check.QuickThrowOnFailure except<string>
    Check.QuickThrowOnFailure except<NormalFloat>

let sort<'a when 'a : comparison> (xs : 'a []) =
    let s = xs |> Seq.sort 
    let l = xs |> List.ofArray |> List.sort
    let a = xs |> Array.sort
    Seq.toArray s = a && List.toArray l = a

[<Test>]
let ``sort is consistent`` () =
    Check.QuickThrowOnFailure sort<int>
    Check.QuickThrowOnFailure sort<string>
    Check.QuickThrowOnFailure sort<NormalFloat>

let splitInto<'a when 'a : equality> (xs : 'a []) count =
    let s = run (fun () -> xs |> Seq.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    let l = run (fun () -> xs |> List.ofArray |> List.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    let a = run (fun () -> xs |> Array.splitInto count |> Seq.map Seq.toArray |> Seq.toArray)
    s = a && l = a

[<Test>]
let ``splitInto is consistent`` () =
    Check.QuickThrowOnFailure splitInto<int>
    Check.QuickThrowOnFailure splitInto<string>
    Check.QuickThrowOnFailure splitInto<NormalFloat>