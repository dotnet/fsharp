// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module ``String Format Tests`` =

    [<Test>]
    let ``sprintf with %d format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %d does not work correctly with UInt64 values

        Assert.areEqual (sprintf "%d" System.UInt64.MaxValue) "18446744073709551615"
        Assert.areEqual (sprintf "%d" System.UInt64.MinValue) "0"
        Assert.areEqual (sprintf "%d" System.UInt32.MaxValue) "4294967295"
        Assert.areEqual (sprintf "%d" System.UInt32.MinValue) "0"
        Assert.areEqual (sprintf "%d" System.UInt16.MaxValue) "65535"
        Assert.areEqual (sprintf "%d" System.UInt16.MinValue) "0"
        Assert.areEqual (sprintf "%d" System.Byte.MaxValue) "255"
        Assert.areEqual (sprintf "%d" System.Byte.MinValue) "0"
        Assert.areEqual (sprintf "%d" System.Int64.MaxValue) "9223372036854775807"
        Assert.areEqual (sprintf "%d" System.Int64.MinValue) "-9223372036854775808"
        Assert.areEqual (sprintf "%d" System.Int32.MaxValue) "2147483647"
        Assert.areEqual (sprintf "%d" System.Int32.MinValue) "-2147483648"
        Assert.areEqual (sprintf "%d" System.Int16.MaxValue) "32767"
        Assert.areEqual (sprintf "%d" System.Int16.MinValue) "-32768"
        Assert.areEqual (sprintf "%d" System.SByte.MaxValue) "127"
        Assert.areEqual (sprintf "%d" System.SByte.MinValue) "-128"
        Assert.areEqual (sprintf "%d" 1un) "1"
        Assert.areEqual (sprintf "%d" -1n) "-1"

    [<Test>]
    let ``sprintf with %i format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %i does not work correctly with UInt64 values

        Assert.areEqual (sprintf "%i" System.UInt64.MaxValue) "18446744073709551615"
        Assert.areEqual (sprintf "%i" System.UInt64.MinValue) "0"
        Assert.areEqual (sprintf "%i" System.UInt32.MaxValue) "4294967295"
        Assert.areEqual (sprintf "%i" System.UInt32.MinValue) "0"
        Assert.areEqual (sprintf "%i" System.UInt16.MaxValue) "65535"
        Assert.areEqual (sprintf "%i" System.UInt16.MinValue) "0"
        Assert.areEqual (sprintf "%i" System.Byte.MaxValue) "255"
        Assert.areEqual (sprintf "%i" System.Byte.MinValue) "0"
        Assert.areEqual (sprintf "%i" System.Int64.MaxValue) "9223372036854775807"
        Assert.areEqual (sprintf "%i" System.Int64.MinValue) "-9223372036854775808"
        Assert.areEqual (sprintf "%i" System.Int32.MaxValue) "2147483647"
        Assert.areEqual (sprintf "%i" System.Int32.MinValue) "-2147483648"
        Assert.areEqual (sprintf "%i" System.Int16.MaxValue) "32767"
        Assert.areEqual (sprintf "%i" System.Int16.MinValue) "-32768"
        Assert.areEqual (sprintf "%i" System.SByte.MaxValue) "127"
        Assert.areEqual (sprintf "%i" System.SByte.MinValue) "-128"
        Assert.areEqual (sprintf "%i" 1un) "1"
        Assert.areEqual (sprintf "%i" -1n) "-1"

    [<Test>]
    let ``sprintf with %u format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %u does not work correctly with UInt64 values

        Assert.areEqual (sprintf "%u" System.UInt64.MaxValue) "18446744073709551615"
        Assert.areEqual (sprintf "%u" System.UInt64.MinValue) "0"
        Assert.areEqual (sprintf "%u" System.UInt32.MaxValue) "4294967295"
        Assert.areEqual (sprintf "%u" System.UInt32.MinValue) "0"
        Assert.areEqual (sprintf "%u" System.UInt16.MaxValue) "65535"
        Assert.areEqual (sprintf "%u" System.UInt16.MinValue) "0"
        Assert.areEqual (sprintf "%u" System.Byte.MaxValue) "255"
        Assert.areEqual (sprintf "%u" System.Byte.MinValue) "0"
        Assert.areEqual (sprintf "%u" System.Int64.MaxValue) "9223372036854775807"
        Assert.areEqual (sprintf "%u" System.Int64.MinValue) "9223372036854775808"
        Assert.areEqual (sprintf "%u" System.Int32.MaxValue) "2147483647"
        Assert.areEqual (sprintf "%u" System.Int32.MinValue) "2147483648"
        Assert.areEqual (sprintf "%u" System.Int16.MaxValue) "32767"
        Assert.areEqual (sprintf "%u" System.Int16.MinValue) "32768"
        Assert.areEqual (sprintf "%u" System.SByte.MaxValue) "127"
        Assert.areEqual (sprintf "%u" System.SByte.MinValue) "128"
        Assert.areEqual (sprintf "%u" 1un) "1"
        Assert.areEqual (sprintf "%u" -1n) (if System.IntPtr.Size = 4 then "4294967295" else "18446744073709551615")

    [<Test>]
    let ``string constructor``() =
        // Regression test for FSHARP1.0:5894

        Assert.areEqual (string 1.0f) "1"
        Assert.areEqual (string 1.00001f) "1.00001"
        Assert.areEqual (string -1.00001f) "-1.00001"
        Assert.areEqual (string 1.0) "1"
        Assert.areEqual (string  1.00001) "1.00001"
        Assert.areEqual (string  -1.00001) "-1.00001"
        Assert.areEqual (string  System.SByte.MaxValue) "127"
        Assert.areEqual (string  System.SByte.MinValue) "-128"
        Assert.areEqual (string  0y) "0"
        Assert.areEqual (string  -1y) "-1"
        Assert.areEqual (string  1y) "1"
        Assert.areEqual (string  System.Byte.MaxValue) "255"
        Assert.areEqual (string  System.Byte.MinValue) "0"
        Assert.areEqual (string  0uy) "0"
        Assert.areEqual (string  1uy) "1"
        Assert.areEqual (string  System.Int16.MaxValue) "32767"
        Assert.areEqual (string  System.Int16.MinValue) "-32768"
        Assert.areEqual (string  0s) "0"
        Assert.areEqual (string  -10s) "-10"
        Assert.areEqual (string  10s) "10"
        Assert.areEqual (string  System.UInt16.MaxValue) "65535"
        Assert.areEqual (string  System.UInt16.MinValue) "0"
        Assert.areEqual (string  0us) "0"
        Assert.areEqual (string  110us) "110"
        Assert.areEqual (string  System.Int32.MaxValue) "2147483647"
        Assert.areEqual (string  System.Int32.MinValue) "-2147483648"
        Assert.areEqual (string  0) "0"
        Assert.areEqual (string  -10) "-10"
        Assert.areEqual (string  10) "10"
        Assert.areEqual (string  System.UInt32.MaxValue) "4294967295"
        Assert.areEqual (string  System.UInt32.MinValue) "0"
        Assert.areEqual (string  0u) "0"
        Assert.areEqual (string  10u) "10"
        Assert.areEqual (string  System.Int64.MaxValue) "9223372036854775807"
        Assert.areEqual (string  System.Int64.MinValue) "-9223372036854775808"
        Assert.areEqual (string  0L) "0"
        Assert.areEqual (string  -10L) "-10"
        Assert.areEqual (string  10L) "10"
        Assert.areEqual (string  System.UInt64.MaxValue) "18446744073709551615"
        Assert.areEqual (string  System.UInt64.MinValue) "0"
        Assert.areEqual (string  0UL) "0"
        Assert.areEqual (string  10UL) "10"
        Assert.areEqual (string  System.Decimal.MaxValue) "79228162514264337593543950335"
        Assert.areEqual (string  System.Decimal.MinValue) "-79228162514264337593543950335"
        Assert.areEqual (string  System.Decimal.Zero) "0"
        Assert.areEqual (string  12345678M) "12345678"
        Assert.areEqual (string  -12345678M) "-12345678"
        Assert.areEqual (string  -infinity) "-Infinity"
        Assert.areEqual (string  infinity) "Infinity"
        Assert.areEqual (string  nan) "NaN"
        Assert.areEqual (string  -infinityf) "-Infinity"
        Assert.areEqual (string  infinityf) "Infinity"
        Assert.areEqual (string  nanf) "NaN"
        Assert.areEqual (string (new System.Guid("210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"))) "210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"

    [<Test>]
    let ``string constructor in FSI``() =
        // Regression test for FSHARP1.0:5894

        CompilerAssert.RunScriptWithOptions [| "--langversion:5.0" |]
            """
let assertEqual a b =
    if a <> b then failwithf "Expected '%s', but got '%s'" a b
    ()

assertEqual (string 1.0f) "1"
assertEqual (string 1.00001f) "1.00001"
assertEqual (string -1.00001f) "-1.00001"
assertEqual (string 1.0) "1"
assertEqual (string  1.00001) "1.00001"
assertEqual (string  -1.00001) "-1.00001"
assertEqual (string  System.SByte.MaxValue) "127"
assertEqual (string  System.SByte.MinValue) "-128"
assertEqual (string  0y) "0"
assertEqual (string  -1y) "-1"
assertEqual (string  1y) "1"
assertEqual (string  System.Byte.MaxValue) "255"
assertEqual (string  System.Byte.MinValue) "0"
assertEqual (string  0uy) "0"
assertEqual (string  1uy) "1"
assertEqual (string  System.Int16.MaxValue) "32767"
assertEqual (string  System.Int16.MinValue) "-32768"
assertEqual (string  0s) "0"
assertEqual (string  -10s) "-10"
assertEqual (string  10s) "10"
assertEqual (string  System.UInt16.MaxValue) "65535"
assertEqual (string  System.UInt16.MinValue) "0"
assertEqual (string  0us) "0"
assertEqual (string  110us) "110"
assertEqual (string  System.Int32.MaxValue) "2147483647"
assertEqual (string  System.Int32.MinValue) "-2147483648"
assertEqual (string  0) "0"
assertEqual (string  -10) "-10"
assertEqual (string  10) "10"
assertEqual (string  System.UInt32.MaxValue) "4294967295"
assertEqual (string  System.UInt32.MinValue) "0"
assertEqual (string  0u) "0"
assertEqual (string  10u) "10"
assertEqual (string  System.Int64.MaxValue) "9223372036854775807"
assertEqual (string  System.Int64.MinValue) "-9223372036854775808"
assertEqual (string  0L) "0"
assertEqual (string  -10L) "-10"
assertEqual (string  10L) "10"
assertEqual (string  System.UInt64.MaxValue) "18446744073709551615"
assertEqual (string  System.UInt64.MinValue) "0"
assertEqual (string  0UL) "0"
assertEqual (string  10UL) "10"
assertEqual (string  System.Decimal.MaxValue) "79228162514264337593543950335"
assertEqual (string  System.Decimal.MinValue) "-79228162514264337593543950335"
assertEqual (string  System.Decimal.Zero) "0"
assertEqual (string  12345678M) "12345678"
assertEqual (string  -12345678M) "-12345678"
assertEqual (string  -infinity) "-Infinity"
assertEqual (string  infinity) "Infinity"
assertEqual (string  nan) "NaN"
assertEqual (string  -infinityf) "-Infinity"
assertEqual (string  infinityf) "Infinity"
assertEqual (string  nanf) "NaN"
assertEqual (string (new System.Guid("210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"))) "210f4d6b-cb42-4b09-baa1-f1aa8e59d4b0"
            """
            []
