// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

module ``String Format Tests`` =
    
    [<Test>]
    let ``sprintf with %d format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %d does not work correctly with UInt64 values

        Assert.areEqual ((sprintf "%d" System.UInt64.MaxValue), "18446744073709551615") |> ignore
        Assert.areEqual ((sprintf "%d" System.UInt64.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%d" System.UInt32.MaxValue), "4294967295") |> ignore
        Assert.areEqual ((sprintf "%d" System.UInt32.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%d" System.UInt16.MaxValue), "65535") |> ignore
        Assert.areEqual ((sprintf "%d" System.UInt16.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%d" System.Byte.MaxValue), "255") |> ignore
        Assert.areEqual ((sprintf "%d" System.Byte.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int64.MaxValue), "9223372036854775807") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int64.MinValue), "-9223372036854775808") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int32.MaxValue), "2147483647") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int32.MinValue), "-2147483648") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int16.MaxValue), "32767") |> ignore
        Assert.areEqual ((sprintf "%d" System.Int16.MinValue), "-32768") |> ignore
        Assert.areEqual ((sprintf "%d" System.SByte.MaxValue), "127") |> ignore
        Assert.areEqual ((sprintf "%d" System.SByte.MinValue), "-128") |> ignore
        Assert.areEqual ((sprintf "%d" 1un), "1") |> ignore
        Assert.areEqual ((sprintf "%d" -1n), "-1") |> ignore

    [<Test>]
    let ``sprintf with %i format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %i does not work correctly with UInt64 values

        Assert.areEqual ((sprintf "%i" System.UInt64.MaxValue), "18446744073709551615") |> ignore
        Assert.areEqual ((sprintf "%i" System.UInt64.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%i" System.UInt32.MaxValue), "4294967295") |> ignore
        Assert.areEqual ((sprintf "%i" System.UInt32.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%i" System.UInt16.MaxValue), "65535") |> ignore
        Assert.areEqual ((sprintf "%i" System.UInt16.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%i" System.Byte.MaxValue), "255") |> ignore
        Assert.areEqual ((sprintf "%i" System.Byte.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int64.MaxValue), "9223372036854775807") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int64.MinValue), "-9223372036854775808") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int32.MaxValue), "2147483647") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int32.MinValue), "-2147483648") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int16.MaxValue), "32767") |> ignore
        Assert.areEqual ((sprintf "%i" System.Int16.MinValue), "-32768") |> ignore
        Assert.areEqual ((sprintf "%i" System.SByte.MaxValue), "127") |> ignore
        Assert.areEqual ((sprintf "%i" System.SByte.MinValue), "-128") |> ignore
        Assert.areEqual ((sprintf "%i" 1un), "1") |> ignore
        Assert.areEqual ((sprintf "%i" -1n), "-1") |> ignore

    [<Test>]
    let ``sprintf with %u format specifier``() =
        // Regression test for FSHARP1.0:4120
        // format specifier %u does not work correctly with UInt64 values

        Assert.areEqual ((sprintf "%u" System.UInt64.MaxValue), "18446744073709551615") |> ignore
        Assert.areEqual ((sprintf "%u" System.UInt64.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%u" System.UInt32.MaxValue), "4294967295") |> ignore
        Assert.areEqual ((sprintf "%u" System.UInt32.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%u" System.UInt16.MaxValue), "65535") |> ignore
        Assert.areEqual ((sprintf "%u" System.UInt16.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%u" System.Byte.MaxValue), "255") |> ignore
        Assert.areEqual ((sprintf "%u" System.Byte.MinValue), "0") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int64.MaxValue), "9223372036854775807") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int64.MinValue), "9223372036854775808") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int32.MaxValue), "2147483647") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int32.MinValue), "2147483648") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int16.MaxValue), "32767") |> ignore
        Assert.areEqual ((sprintf "%u" System.Int16.MinValue), "32768") |> ignore
        Assert.areEqual ((sprintf "%u" System.SByte.MaxValue), "127") |> ignore
        Assert.areEqual ((sprintf "%u" System.SByte.MinValue), "128") |> ignore
        Assert.areEqual ((sprintf "%u" 1un), "1") |> ignore
        Assert.areEqual ((sprintf "%u" -1n), if System.IntPtr.Size = 4 then "4294967295" else "18446744073709551615") |> ignore
