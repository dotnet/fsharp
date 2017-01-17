// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Unittests

open System
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler

[<TestFixture>]
module EditDistance =
    open Internal.Utilities.EditDistance

    [<Test>]
    [<TestCase("RICK", "RICK", ExpectedResult = "1.000")>]
    [<TestCase("MARTHA", "MARHTA", ExpectedResult = "0.961")>]
    [<TestCase("DWAYNE", "DUANE", ExpectedResult = "0.840")>]
    [<TestCase("DIXON", "DICKSONX", ExpectedResult = "0.813")>]
    let JaroWinklerTest (str1 : string, str2 : string) : string =
        String.Format("{0:0.000}", JaroWinklerDistance str1 str2)

    [<Test>]
    [<TestCase("RICK", "RICK", ExpectedResult = 0)>]
    [<TestCase("MARTHA", "MARHTA", ExpectedResult = 1)>]
    [<TestCase("'T", "'u", ExpectedResult = 1)>]
    let EditDistanceTest (str1 : string, str2 : string) : int =
        CalcEditDistance(str1,str2)