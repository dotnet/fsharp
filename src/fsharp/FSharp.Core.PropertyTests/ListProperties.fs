// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Lists

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck


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
