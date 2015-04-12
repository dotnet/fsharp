// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Array

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

type SeqProperties =

    static member ``sort behaves like sortby id``(xs:float[]) =   
          let a = Seq.sortBy id xs |> Seq.toArray 
          let b = Seq.sort xs |> Seq.toArray
          let result = ref true
          for i in 0 .. a.Length - 1 do
            if a.[i] <> b.[i] then
                if System.Double.IsNaN a.[i] <> System.Double.IsNaN b.[i] then
                    result := false
          !result          

type ArrayProperties =

    static member ``sort behaves like sortby id``(xs:float[]) =   
          let a = Array.sortBy id xs
          let b = Array.sort xs 
          let result = ref true
          for i in 0 .. a.Length - 1 do
            if a.[i] <> b.[i] then
                if System.Double.IsNaN a.[i] <> System.Double.IsNaN b.[i] then
                    result := false
          !result          


[<Test>]
let ``Seq properties`` () =
    Check.All<SeqProperties>(Config.Default)

[<Test>]
let ``Array properties`` () =
    Check.All<ArrayProperties>(Config.Default)