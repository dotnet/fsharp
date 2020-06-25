// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Abs Tests`` =

    [<Test>]
    let  ``Abs of integral types``() =
        // Regression test for FSHARP1.0:3470 - exception on abs of native integer

        Assert.areEqual (abs -1y) 1y   // signed byte
        Assert.areEqual (abs -1s) 1s   // int16
        Assert.areEqual (abs -1l) 1l   // int32
        Assert.areEqual (abs -1n) 1n   // nativeint
        Assert.areEqual (abs -1L) 1L   // int64
        Assert.areEqual (abs -1I) 1I   // bigint