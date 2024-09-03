﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit


module ``Sprintf Tests`` =

    type MyR = {c:int;b:int;a:int}

    [<Fact>]
    let ``Sprintf %A of record type``() =
        // Regression test for FSHARP1.0:5113

        let s1 = sprintf "%A" {a=1;b=2;c=3}
        let s2 = sprintf "%A" {c=3;b=2;a=1}

        Assert.areEqual s1 s2

    type MyT = MyC of int * string * bool

    [<Fact>]
    let ``Sprintf %A of discriminated union type``() =
        // Regression test for FSHARP1.0:5113

        let DU =  MyC (1,"2",true)
        Assert.areEqual "MyC (1, \"2\", true)" (sprintf "%A" DU)