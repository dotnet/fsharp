// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Char Constants`` =

    [<TestCase('\a', 7 )>] // alert
    [<TestCase('\b', 8 )>] // backspace
    [<TestCase('\t', 9 )>] // horizontal tab
    [<TestCase('\n', 10)>] // new line
    [<TestCase('\v', 11)>] // vertical tab
    [<TestCase('\f', 12)>] // form feed
    [<TestCase('\r', 13)>] // return
    [<TestCase('\"', 34)>] // double quote
    [<TestCase('\'', 39)>] // single quote
    [<TestCase('\\', 92)>] // backslash
    let ``Escape characters`` character value =
        Assert.areEqual character (char value)