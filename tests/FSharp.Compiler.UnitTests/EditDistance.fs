// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Globalization
open Xunit
open FSharp.Test

module EditDistance =
    open Internal.Utilities.EditDistance

    [<Theory>]
    [<InlineData("RICK", "RICK", "1.000")>]
    [<InlineData("MARTHA", "MARHTA", "0.961")>]
    [<InlineData("DWAYNE", "DUANE", "0.840")>]
    [<InlineData("DIXON", "DICKSONX", "0.813")>]
    let JaroWinklerTest (str1 : string, str2 : string, expected : string) : unit =
        String.Format(CultureInfo.InvariantCulture, "{0:0.000}", JaroWinklerDistance str1 str2)
        |> Assert.shouldBe expected

    [<Theory>]
    [<InlineData("RICK", "RICK", 0)>]
    [<InlineData("MARTHA", "MARHTA", 1)>]
    [<InlineData("'T", "'u", 1)>]
    let EditDistanceTest (str1 : string, str2 : string, expected : int) : unit =
        CalcEditDistance(str1,str2)
        |> Assert.shouldBe expected
