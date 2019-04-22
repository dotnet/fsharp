// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Globalization
open System.Text
open NUnit.Framework
open FSharp.Compiler

[<TestFixture>]
module VersionCompare =
    open FSharp.Compiler.DotNetFrameworkDependencies

    [<TestCase("1.0.0", "1.0.1", ExpectedResult = -1)>]                 // 1.0.0 < 1.0.1
    [<TestCase("1.0.0", "1.0.0", ExpectedResult = 0)>]                  // 1.0.0 = 1.0.0
    [<TestCase("1.0.1", "1.0.0", ExpectedResult = 1)>]                  // 1.0.1 > 1.0.1
    [<TestCase("0.0.9", "1.0.0-Suffix1", ExpectedResult = -1)>]         // 0.0.9 < 1.0.0-Suffix1
    [<TestCase("1.0.0", "1.0.0-Suffix1", ExpectedResult = 1)>]          // 1.0.0 > 1.0.0-Suffix1
    [<TestCase("1.0.0-Suffix1", "1.0.0", ExpectedResult = -1)>]         // 1.0.0-Suffix1 < 1.0.0
    [<TestCase("1.0.0-Suffix1", "1.0.0-Suffix2", ExpectedResult = -1)>] // 1.0.0-Suffix1 < 1.0.0-Suffix2
    [<TestCase("1.0.0-Suffix2", "1.0.0-Suffix1", ExpectedResult = 1)>]  // 1.0.0-Suffix2 > 1.0.0-Suffix1
    [<TestCase("1.0.0-Suffix1", "1.0.0-Suffix1", ExpectedResult = 0)>]  // 1.0.0-Suffix1 > 1.0.0-Suffix2
    [<TestCase("1.0.1", "1.0.0-Suffix1", ExpectedResult = 1)>]          // 1.0.1 > 1.0.0-Suffix1
    [<TestCase("1.0.0-Suffix1", "1.0.1", ExpectedResult = -1)>]        // 1.0.0-Suffix1 < 1.0.1
    let VersionCompareTest (str1: string, str2: string) : int =
        versionCompare str1 str2


    [<Test>]
    [<TestCase("", ExpectedResult = "3.0.0-preview4-27610-06")>]
    let VersionCompareSortArrayHighestPreview _: string =
        let versions = [|
            "1.0.0-preview4-20000-01"
            "3.0.0-preview4-27610-06"
            "1.0.0-preview4-20000-02"
            "3.0.0-preview4-27610-05"
            "3.0.0-preview4-27609-10"
        |]
        versions |> Array.sortWith (versionCompare) |> Array.last

    [<Test>]
    [<TestCase("", ExpectedResult = "3.0.0")>]
    let VersionCompareSortArrayHighestRelease _: string =
        let versions = [|
            "1.0.0-preview4-20000-01"
            "3.0.0"
            "3.0.0-preview4-27610-06"
            "1.0.0-preview4-20000-02"
            "3.0.0-preview4-27610-05"
            "3.0.0-preview4-27609-10"
        |]
        versions |> Array.sortWith (versionCompare) |> Array.last

    [<Test>]
    [<TestCase("", ExpectedResult = "3.0.1")>]
    let VersionCompareSortArrayEvenHighestRelease _: string =
        let versions = [|
            "3.0.1"
            "1.0.0-preview4-20000-01"
            "3.0.0"
            "3.0.0-preview4-27610-06"
            "1.0.0-preview4-20000-02"
            "3.0.0-preview4-27610-05"
            "3.0.0-preview4-27609-10"
        |]
        versions |> Array.sortWith (versionCompare) |> Array.last
