// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open System


module ``Cast Tests`` =

    [<Fact>]
    let ``Cast precedence over expression forms``() =
        // Regression test for FSHARP1.0:1247
        // Precedence of type annotations :> and :?> over preceeding expression forms, e.g. if-then-else etc.

        Assert.IsAssignableFrom<Object> (2 :> Object) |> ignore
        Assert.IsAssignableFrom<Object list> [(2 :> Object)] |> ignore
        Assert.IsAssignableFrom<Object list> [2 :> Object] |> ignore