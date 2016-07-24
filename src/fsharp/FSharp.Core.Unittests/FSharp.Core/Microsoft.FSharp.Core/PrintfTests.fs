// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for:
// Microsoft.FSharp.Core.ExtraTopLevelOperators.printf

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

[<TestFixture>]
type PrintfTests() =
    let test fmt arg (expected:string) =
        let actual = sprintf fmt arg
        Assert.AreEqual(expected, actual)

    [<Test>]
    member this.FormatAndPrecisionSpecifiers() =
        test "%10s"  "abc" "       abc"
        test "%-10s" "abc" "abc       "
        test "%10d"  123   "       123"
        test "%-10d" 123   "123       "
        test "%10c"  'a'   "         a"
        test "%-10c" 'a'   "a         "
