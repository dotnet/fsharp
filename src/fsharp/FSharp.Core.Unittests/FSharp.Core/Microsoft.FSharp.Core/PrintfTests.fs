// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for:
// Microsoft.FSharp.Core.ExtraTopLevelOperators.printf

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

type private PrivateNestedString =
    {
        InnerString : string
    }

type PublicNestedString =
    {
        InnerStringPublic : string
    }

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

    [<Test>]
    member __.``Standard characters are not escaped``() =
        test "%A" "\n" "\"\n\""
        test "%A" "\r" "\"\r\""
        test "%A" "\t" "\"\t\""
        test "%A" "\b" "\"\b\""

    [<Test>]
    member __.``Standard characters are correctly escaped with at flag``() =
        test "%@A" "Foo\nBar" "\"Foo\\nBar\""
        test "%@A" "Foo\rBar" "\"Foo\\rBar\""
        test "%@A" "\t" "\"\\t\""
        test "%@A" "\b" "\"\\b\""

    [<Test>]
    member __.``Quotation characters are not escaped``() =
        test "%A" "\"" "\"\\\"\""
        test "%A" '\'' "\'\\\'\'"

    [<Test>]
    member __.``Quotation characters are correctly escaped with at flag``() =
        test "%@A" "\"" "\"\\\"\""
        test "%@A" '\'' "\'\\\'\'"

    [<Test>]
    member __.``Control characters are not escaped``() =
        test "%A" "\0" "\"\000\""
        test "%A" "\10" "\'\010\'"

    [<Test>]
    member __.``Control characters are correctly escaped with at flag``() =
        test "%@A" "\0" "\"\\\000\""
        test "%@A" "\10" "\'\\\010\'"

    [<Test>]
    member __.``Path-like strings are formatted without escaping``() =
        test "%A" @"C:\Program Files\Some\Path.exe" "\"C:\\Program Files\\Some\\Path.exe\""
        test "%A" @"C:\" "\"C:\\\""

    [<Test>]
    member __.``Path-like strings are formatted as verbatim strings with at flag``() =
        test "%@A" @"C:\Program Files\Some\Path.exe" "@\"C:\\Program Files\\Some\\Path.exe\""
        test "%@A" @"C:\" "@\"C:\\\""

    [<Test>]
    member __.``Empty strings are not formatted as verbatim strings``() =
        test "%A" "" "\"\""
        test "%+A" "" "\"\""
        test "%@+A" "" "\"\""
        test "%+@A" "" "\"\""

    [<Test>]
    member __.``Object printing does not print private types`` () =
        let actual = sprintf "%A" { InnerString = "Lorem" }
        if actual.Contains "Lorem" then Assert.Fail()
        let actual = sprintf "%@A" { InnerString = "Lorem" }
        if actual.Contains "Lorem" then Assert.Fail()

    [<Test>]
    member __.``Object printing prints private types with plus flag`` () =
        test "%+A" { InnerString = "Lo\trem" } "{InnerString = \"Lo\trem\";}"

        // test escaping
        test "%+@A" { InnerString = "Lo\trem" } "{InnerString = \"Lo\\trem\";}"
        test "%@+A" { InnerString = "Lo\trem" } "{InnerString = \"Lo\\trem\";}"

    [<Test>]
    member __.``Object printing prints public types with plus flag`` () =
        test "%+A" { InnerStringPublic = "Lo\trem" } "{InnerStringPublic = \"Lo\trem\";}"
        test "%+@A" { InnerStringPublic = "Lo\trem" } "{InnerStringPublic = \"Lo\\trem\";}"
        test "%@+A" { InnerStringPublic = "Lo\trem" } "{InnerStringPublic = \"Lo\\trem\";}"