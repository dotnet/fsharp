// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the Checked module

namespace FSharp.Core.UnitTests.Operators

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open Microsoft.FSharp.Core.Operators.Checked

type OperatorsModuleChecked() =

    [<Fact>]
    member _.Checkedbyte() =
        // int type
        let intByte = Operators.Checked.byte 100
        Assert.AreEqual(100uy, intByte)
 
        // char type
        let charByte = Operators.Checked.byte '0'
        Assert.AreEqual(48uy, charByte)

        // boundary value
        let boundByte = Operators.Checked.byte 255.0
        Assert.AreEqual(255uy, boundByte)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.byte 256 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.byte 256f |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.byte 256. |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> 255uy + 1uy |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> 0uy - 1uy |> ignore)

    [<Fact>]
    member _.Checkedchar() =

        // number
        let numberChar = Operators.Checked.char 48
        Assert.AreEqual('0', numberChar)
        
        // letter
        let letterChar = Operators.Checked.char 65
        Assert.AreEqual('A', letterChar)
        
        // boundary value
        let boundchar = Operators.Checked.char 126
        Assert.AreEqual('~', boundchar)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.char (int64 Char.MaxValue + 1L) |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.char Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.char Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> '\uFFFF' + '\u0001' |> ignore)

        
    [<Fact>]
    member _.CheckedInt() =

        // char
        let charInt = Operators.Checked.int '0'
        Assert.AreEqual(48, charInt)
        
        // float
        let floatInt = Operators.Checked.int 10.0
        Assert.AreEqual(10, floatInt)

        // boundary value
        let boundInt = Operators.Checked.int 32767.0
        Assert.AreEqual(32767, boundInt)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.int 2147483648.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int32.MaxValue + 1 |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int32.MinValue - 1 |> ignore)

    [<Fact>]
    member _.CheckedInt16() =

        // char
        let charInt16 = Operators.Checked.int16 '0'
        Assert.AreEqual(48s, charInt16)
        
        // float
        let floatInt16 = Operators.Checked.int16 10.0
        Assert.AreEqual(10s, floatInt16)
        
        // boundary value
        let boundInt16 = Operators.Checked.int16 32767.0
        Assert.AreEqual(32767s, boundInt16)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.int16 32768.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int16 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int16 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int16.MaxValue + 1s |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int16.MinValue - 1s |> ignore)

    [<Fact>]
    member _.CheckedInt32() =

        // char
        let charInt32 = Operators.Checked.int32 '0'
        Assert.AreEqual(48, charInt32)
        
        // float
        let floatInt32 = Operators.Checked.int32 10.0
        Assert.AreEqual(10, floatInt32)
        
        // boundary value
        let boundInt32 = Operators.Checked.int32 2147483647.0
        Assert.AreEqual(2147483647, boundInt32)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.int32 2147483648.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int32 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int32 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int32.MaxValue + 1 |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int32.MinValue - 1 |> ignore)

    [<Fact>]
    member _.CheckedInt64() =

        // char
        let charInt64 = Operators.Checked.int64 '0'
        Assert.AreEqual(48L, charInt64)
        
        // float
        let floatInt64 = Operators.Checked.int64 10.0
        Assert.AreEqual(10L, floatInt64)
        
        // boundary value
        let boundInt64 = Operators.Checked.int64 9223372036854775807I
        let _  = 9223372036854775807L
        Assert.AreEqual(9223372036854775807L, boundInt64)
        
        // boundary value
        let boundInt64 = Operators.Checked.int64 -9223372036854775808I
        let _  = -9223372036854775808L
        Assert.AreEqual(-9223372036854775808L, boundInt64)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.int64 (float Int64.MaxValue + 1.0) |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int64 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.int64 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int64.MaxValue + 1L |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Int64.MinValue - 1L |> ignore)

    [<Fact>]
    member _.CheckedNativeint() =

        // char
        let charnativeint = Operators.Checked.nativeint '0'
        Assert.AreEqual(48n, charnativeint)
        
        // float
        let floatnativeint = Operators.Checked.nativeint 10.0
        Assert.AreEqual(10n, floatnativeint)
        
        // boundary value
        let boundnativeint = Operators.Checked.nativeint 32767.0
        Assert.AreEqual(32767n, boundnativeint)
        
        // overflow exception (depends on pointer size)
        CheckThrowsOverflowException(fun() ->
            if Info.isX86Runtime then
                Operators.Checked.nativeint 2147483648.0 |> ignore
            else
                Operators.Checked.nativeint 9223372036854775808.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.nativeint Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.nativeint Double.MaxValue |> ignore)

         
    [<Fact>]
    member _.Checkedsbyte() =

        // char
        let charsbyte = Operators.Checked.sbyte '0'
        Assert.AreEqual(48y, charsbyte)
        
        // float
        let floatsbyte = Operators.Checked.sbyte -10.0
        Assert.AreEqual(-10y, floatsbyte)
        
        // boundary value
        let boundsbyte = Operators.Checked.sbyte -127.0
        Assert.AreEqual(-127y, boundsbyte)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.sbyte -256 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.sbyte Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.sbyte Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> SByte.MaxValue + 1y |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> SByte.MinValue - 1y |> ignore)

    [<Fact>]
    member _.Checkeduint16() =

        // char
        let charuint16 = Operators.Checked.uint16 '0'
        Assert.AreEqual(48us, charuint16)
        
        // float
        let floatuint16 = Operators.Checked.uint16 10.0
        Assert.AreEqual(10us, floatuint16)
        
        // boundary value
        let bounduint16 = Operators.Checked.uint16 65535.0
        Assert.AreEqual(65535us, bounduint16)
        
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint16 65536.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint16 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint16 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt16.MaxValue + 1us |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt16.MinValue - 1us |> ignore)

    [<Fact>]
    member _.Checkeduint32() =

        // char
        let charuint32 = Operators.Checked.uint32 '0'
        Assert.AreEqual(48u, charuint32)
        
        // float
        let floatuint32 = Operators.Checked.uint32 10.0
        Assert.AreEqual(10u, floatuint32)
        
        // boundary value
        let bounduint32 = Operators.Checked.uint32 429496729.0
        Assert.AreEqual(429496729u, bounduint32)

        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint32 (float UInt32.MaxValue + 1.0) |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint32 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint32 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt32.MaxValue + 1u |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt32.MinValue - 1u |> ignore)

    [<Fact>]
    member _.Checkeduint64() =

        // char
        let charuint64 = Operators.Checked.uint64 '0'
        Assert.AreEqual(48UL, charuint64)
        
        // float
        let floatuint64 = Operators.Checked.uint64 10.0
        Assert.AreEqual(10UL, floatuint64)
        
        // boundary value
        let bounduint64 = Operators.Checked.uint64 429496729.0
        Assert.AreEqual(429496729UL, bounduint64)
        
        // overflow exception
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint64 (float System.UInt64.MaxValue + 1.0) |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint64 Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.uint64 Double.MaxValue |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt64.MaxValue + 1UL |> ignore)

        // overflow exception
        CheckThrowsOverflowException(fun () -> UInt64.MinValue - 1UL |> ignore)

    [<Fact>]
    member _.Checkedunativeint() =

        // char
        let charunativeint = Operators.Checked.unativeint '0'
        Assert.AreEqual(48un, charunativeint)
        
        // float
        let floatunativeint = Operators.Checked.unativeint 10.0
        Assert.AreEqual(10un, floatunativeint)
        
        // boundary value (dependent on pointer size)
        if Info.isX86Runtime then
            let boundunativeint = Operators.Checked.unativeint 4294967295.0
            Assert.AreEqual(4294967295un, boundunativeint)
        else
            let boundnativeint = Operators.Checked.unativeint 1.84467440737095505E+19  // 64 bit max value cannot be expressed exactly as double
            Assert.AreEqual(18446744073709549568un, boundnativeint)
        
        // overflow exception (depends on pointer size)
        CheckThrowsOverflowException(fun () -> 
            if Info.isX86Runtime then
                Operators.Checked.unativeint (float UInt32.MaxValue + 1.0) |> ignore
            else 
                Operators.Checked.unativeint (float UInt64.MaxValue + 1.0) |> ignore
        )
        CheckThrowsOverflowException(fun () -> Operators.Checked.unativeint Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.unativeint Double.MaxValue |> ignore)


