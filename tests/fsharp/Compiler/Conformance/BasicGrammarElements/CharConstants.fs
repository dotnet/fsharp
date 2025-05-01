// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test


module ``Char Constants`` =

    [<Theory>]
    [<InlineData('\a', 7 )>] // alert
    [<InlineData('\b', 8 )>] // backspace
    [<InlineData('\t', 9 )>] // horizontal tab
    [<InlineData('\n', 10)>] // new line
    [<InlineData('\v', 11)>] // vertical tab
    [<InlineData('\f', 12)>] // form feed
    [<InlineData('\r', 13)>] // return
    [<InlineData('\"', 34)>] // double quote
    [<InlineData('\'', 39)>] // single quote
    [<InlineData('\\', 92)>] // backslash
    let ``Escape characters`` character value =
        Assert.areEqual character (char value)