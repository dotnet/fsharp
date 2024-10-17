// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open System


module ``Cast Tests`` =

    let inline isTypeOf<'T> x =
        Assert.True(typeof<'T>.IsInstanceOfType(x))

    [<Fact>]
    let ``Cast precedence over expression forms``() =
        // Regression test for FSHARP1.0:1247
        // Precedence of type annotations :> and :?> over preceeding expression forms, e.g. if-then-else etc.

        isTypeOf<Object> (2 :> Object)
        isTypeOf<Object list> [(2 :> Object)]
        isTypeOf<Object list> [2 :> Object]