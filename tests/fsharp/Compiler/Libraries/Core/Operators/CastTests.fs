// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open System

[<TestFixture>]
module ``Cast Tests`` =

    [<Test>]
    let ``Cast precedence over expression forms``() =
        // Regression test for FSHARP1.0:1247
        // Precedence of type annotations :> and :?> over preceeding expression forms, e.g. if-then-else etc.

        Assert.IsInstanceOf<Object> (2 :> Object)
        Assert.IsInstanceOf<Object list> [(2 :> Object)]
        Assert.IsInstanceOf<Object list> [2 :> Object]