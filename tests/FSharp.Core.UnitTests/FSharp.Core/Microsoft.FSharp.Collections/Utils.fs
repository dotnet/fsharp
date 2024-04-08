// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.Collections.Utils

open Xunit

let run f = 
    try
        Ok(f())
    with
    | exn -> Error(exn.Message)

let runAndCheckErrorType f = 
    try
        Ok(f())
    with
    | exn -> Error(exn.GetType().ToString())

let runAndCheckIfAnyError f = 
    try
        Ok(f())
    with
    | exn -> Error("")

let isStable sorted = sorted |> Seq.pairwise |> Seq.forall (fun ((ia, a),(ib, b)) -> if a = b then ia < ib else true)

let isSorted sorted = sorted |> Seq.pairwise |> Seq.forall (fun (a,b) -> a <= b)

let haveSameElements (xs:seq<_>) (ys:seq<_>) =
    let xsHashSet = new System.Collections.Generic.HashSet<_>(xs)
    let ysHashSet = new System.Collections.Generic.HashSet<_>(ys)
    xsHashSet.SetEquals(ysHashSet)

let shouldEqual arr1 arr2 = if arr1 <> arr2 then Assert.Fail()