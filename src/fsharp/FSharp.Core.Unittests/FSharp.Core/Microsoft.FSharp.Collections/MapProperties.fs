// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Collections.MapProperties

open System
open System.Collections.Generic
open NUnit.Framework
open FsCheck
open Utils

let addRangeIsLikeMap<'a,'b when 'a : comparison and 'b : comparison> (map,elements : KeyValuePair<'a,'b> []) =
    let mutable addVersion = map
    for kv in elements do
        addVersion <- Map.add kv.Key kv.Value addVersion
    let addRangeVersion = Map.addRange elements map
    addVersion = addRangeVersion

[<Test>]
let ``Map.addRange works like series of Map.add calls`` () =
    Check.QuickThrowOnFailure addRangeIsLikeMap<int,string>
    Check.QuickThrowOnFailure addRangeIsLikeMap<int,int>
    Check.QuickThrowOnFailure addRangeIsLikeMap<string,string>