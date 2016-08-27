// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Core.PropertyTests.Lists

open System
open System.Collections.Generic

open NUnit.Framework
open FsCheck

type ListProperties =
    static member ``Reverse of a reverse of a int list gives the original list``(xs:list<int>) = 
        List.rev(xs) = xs

[<Test>]
let ``List properties`` () =
    Check.All<ListProperties>(Config.Default)