// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Compiler.CheckExpressions

module ``Static parameter formatting`` =

    [<Fact>]
    let ``formatAvailableNames truncates at 5`` () =
        let names = [| "A"; "B"; "C"; "D"; "E"; "F"; "G" |]
        let result = formatAvailableNames names
        Assert.Equal("A, B, C, D, E, ...", result)

    [<Fact>]
    let ``formatAvailableNames with fewer than 5`` () =
        let names = [| "X"; "Y" |]
        let result = formatAvailableNames names
        Assert.Equal("X, Y", result)

    [<Fact>]
    let ``formatAvailableNames with exactly 5`` () =
        let names = [| "A"; "B"; "C"; "D"; "E" |]
        let result = formatAvailableNames names
        Assert.Equal("A, B, C, D, E", result)

    [<Fact>]
    let ``formatAvailableNames with empty`` () =
        let names = [||]
        let result = formatAvailableNames names
        Assert.Equal("", result)

    [<Fact>]
    let ``formatAvailableNames with single name`` () =
        let names = [| "OnlyOne" |]
        let result = formatAvailableNames names
        Assert.Equal("OnlyOne", result)
