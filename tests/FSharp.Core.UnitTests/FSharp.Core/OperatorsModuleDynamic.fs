// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Sync test content with OperatorsModule1.fs, OperatorsModule2.fs, and OperatorsModuleDynamic.fs

namespace FSharp.Core.UnitTests.Operators

open System
open Xunit

#nowarn "1204" // CompilerMessage: This function is for use by dynamic invocations of F# code and should not be used directly
module OperatorsModuleDynamic =
    
    /// Check that the lambda throws an exception of the given type. Otherwise
    /// calls Assert.Fail()
    // Sync implementation with FSharp.Core.UnitTests.LibraryTestFx.CheckThrowsExn
    let CheckThrowsExn<'a when 'a :> exn> (f : unit -> unit) =
        try
            let _ = f ()
            sprintf "Expected %O exception, got no exception" typeof<'a> |> Assert.Fail 
        with
        | :? 'a -> ()
        | :? Reflection.TargetInvocationException as r when (r.InnerException :? 'a) -> ()
        | e -> sprintf "Expected %O or TargetInvocationException containing it, got: %O" typeof<'a> e |> Assert.Fail
    let CheckThrowsOverflowException = CheckThrowsExn<OverflowException>

    module Operators =
        let byte<'T> = LanguagePrimitives.ExplicitDynamic<'T, byte>
        let char<'T> = LanguagePrimitives.ExplicitDynamic<'T, char>
        let double<'T> = LanguagePrimitives.ExplicitDynamic<'T, double>
        let decimal<'T> = LanguagePrimitives.ExplicitDynamic<'T, decimal>
        let float<'T> = LanguagePrimitives.ExplicitDynamic<'T, float>
        let float32<'T> = LanguagePrimitives.ExplicitDynamic<'T, float32>
        let nativeint<'T> = LanguagePrimitives.ExplicitDynamic<'T, nativeint>
        let int<'T> = LanguagePrimitives.ExplicitDynamic<'T, int>
        let int8<'T> = LanguagePrimitives.ExplicitDynamic<'T, int8>
        let int16<'T> = LanguagePrimitives.ExplicitDynamic<'T, int16>
        let int32<'T> = LanguagePrimitives.ExplicitDynamic<'T, int32>
        let int64<'T> = LanguagePrimitives.ExplicitDynamic<'T, int64>
        let sbyte<'T> = LanguagePrimitives.ExplicitDynamic<'T, sbyte>
        let single<'T> = LanguagePrimitives.ExplicitDynamic<'T, single>
        let uint<'T> = LanguagePrimitives.ExplicitDynamic<'T, uint>
        let uint8<'T> = LanguagePrimitives.ExplicitDynamic<'T, uint8>
        let uint16<'T> = LanguagePrimitives.ExplicitDynamic<'T, uint16>
        let uint32<'T> = LanguagePrimitives.ExplicitDynamic<'T, uint32>
        let uint64<'T> = LanguagePrimitives.ExplicitDynamic<'T, uint64>
        let unativeint<'T> = LanguagePrimitives.ExplicitDynamic<'T, unativeint>
        module Checked =
            let byte<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, byte>
            let char<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, char>
            let double<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, double>
            let decimal<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, decimal>
            let float<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, float>
            let float32<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, float32>
            let nativeint<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, nativeint>
            let int<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, int>
            let int8<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, int8>
            let int16<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, int16>
            let int32<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, int32>
            let int64<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, int64>
            let sbyte<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, sbyte>
            let single<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, single>
            let uint<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, uint>
            let uint8<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, uint8>
            let uint16<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, uint16>
            let uint32<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, uint32>
            let uint64<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, uint64>
            let unativeint<'T> = LanguagePrimitives.CheckedExplicitDynamic<'T, unativeint>
    
    [<Fact>]
    let byte() =
        // int type
        let intByte = Operators.byte 100
        Assert.AreEqual(100uy, intByte)
        
        // char type
        let charByte = Operators.byte '0'
        Assert.AreEqual(48uy, charByte)
        
        // boundary value
        let boundByte = Operators.byte 255.0
        Assert.AreEqual(255uy, boundByte)
        
        // Overflow
        let result = Operators.byte Int64.MaxValue
        Assert.AreEqual(Byte.MaxValue, result)
        
        // Overflow
        let result = Operators.byte Int64.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Single.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Single.MaxValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Double.MinValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte Double.MaxValue
        Assert.AreEqual(0uy, result)
        
        // Overflow
        let result = Operators.byte (Int64.MaxValue * 8L)
        Assert.AreEqual(248uy, result)      // bit-complement

        // Overflow
        let result = 255uy + 5uy
        Assert.AreEqual(4uy, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.byte Decimal.MinValue |> ignore)
        
    [<Fact>]
    let char() =
        // int type
        Assert.AreEqual('0', Operators.char 48)
        Assert.AreEqual('0', Operators.char 48u)
        Assert.AreEqual('0', Operators.char 48s)
        Assert.AreEqual('0', Operators.char 48us)
        Assert.AreEqual('0', Operators.char 48y)
        Assert.AreEqual('0', Operators.char 48uy)
        Assert.AreEqual('0', Operators.char 48L)
        Assert.AreEqual('0', Operators.char 48uL)
        Assert.AreEqual('0', Operators.char 48n)
        Assert.AreEqual('0', Operators.char 48un)
        Assert.AreEqual('0', Operators.char 48f)
        Assert.AreEqual('0', Operators.char 48.)
        Assert.AreEqual('0', Operators.char 48m)

        // Overflow
        Assert.AreEqual('\000', Operators.char Single.MinValue)
        Assert.AreEqual('\000', Operators.char Double.MinValue)
        Assert.AreEqual('\000', Operators.char Single.MaxValue)
        Assert.AreEqual('\000', Operators.char Double.MaxValue)
        CheckThrowsOverflowException(fun () -> Operators.char Decimal.MinValue |> ignore)
        
        // string type
        let stringchar = Operators.char " "
        Assert.AreEqual(' ', stringchar)

    
    [<Fact>]
    let decimal () =
        
        // int value
        let intdecimal = Operators.decimal (1)
        Assert.AreEqual(1M, intdecimal)
        
        // nativeint value
        let nativeintdecimal = Operators.decimal 1n
        Assert.AreEqual(1M, nativeintdecimal)
        
        // unativeint value
        let unativeintdecimal = Operators.decimal 1un
        Assert.AreEqual(1M, unativeintdecimal)
        
        // char value
        let chardecimal = Operators.decimal '\001'
        Assert.AreEqual(1M, chardecimal)
       
        // float value
        let floatdecimal = Operators.decimal (1.0)
        Assert.AreEqual(1M, floatdecimal)

    [<Fact>]
    let double() =
        // int type
        let intdouble = Operators.float 100
        Assert.AreEqual(100.0, intdouble)
        
        // char type
        let chardouble = Operators.float '0'
        Assert.AreEqual(48.0, chardouble)
        
        // decimal type
        let decimaldouble = Operators.float 100m
        Assert.AreEqual(100.0, decimaldouble)

    [<Fact>]
    let float() =
        // int type
        let intfloat = Operators.float 100
        Assert.AreEqual((float)100, intfloat)
        
        // char type
        let charfloat = Operators.float '0'
        Assert.AreEqual((float)48, charfloat)

        // decimal type
        let intfloat = Operators.float 100m
        Assert.AreEqual((float)100, intfloat)

    [<Fact>]
    let float32() =
        // int type
        let intfloat32 = Operators.float32 100
        Assert.AreEqual((float32)100, intfloat32)
        
        // char type
        let charfloat32 = Operators.float32 '0'
        Assert.AreEqual((float32)48, charfloat32)
        
        // decimal type
        let intfloat32 = Operators.float32 100m
        Assert.AreEqual((float32)100, intfloat32)

    [<Fact>]
    let int() =
        // int
        let result = Operators.int 10
        Assert.AreEqual(10, result)
        
        // string
        let result = Operators.int "10"
        Assert.AreEqual(10, result)
        
        // double
        let result = Operators.int 10.0
        Assert.AreEqual(10, result)
        
        // negative
        let result = Operators.int -10
        Assert.AreEqual(-10, result)
        
        // zero
        let result = Operators.int 0
        Assert.AreEqual(0, result)
        
        // Overflow
        let result = Operators.int Single.MaxValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int Single.MinValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int Double.MaxValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int Double.MinValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int Int64.MaxValue
        Assert.AreEqual(-1, result)
        
        // Overflow
        let result = Operators.int Int64.MinValue
        Assert.AreEqual(0, result)

        // Overflow
        let result = Int32.MaxValue + 1
        Assert.AreEqual(Int32.MinValue, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.int Decimal.MinValue |> ignore)
        
    [<Fact>]
    let int16() =
        // int
        let result = Operators.int16 10
        Assert.AreEqual(10s, result)
        
        // double
        let result = Operators.int16 10.0
        Assert.AreEqual(10s, result)
        
        // negative
        let result = Operators.int16 -10
        Assert.AreEqual(-10s, result)
        
        // zero
        let result = Operators.int16 0
        Assert.AreEqual(0s, result)
        
        // string
        let result = Operators.int16 "10"
        Assert.AreEqual(10s, result)
        
        // Overflow
        let result = Operators.int16 Single.MaxValue
        Assert.AreEqual(0s, result)

        // Overflow
        let result = Operators.int16 Single.MinValue
        Assert.AreEqual(0s, result)
        
        // Overflow
        let result = Operators.int16 Double.MaxValue
        Assert.AreEqual(0s, result)

        // Overflow
        let result = Operators.int16 Double.MinValue
        Assert.AreEqual(0s, result)

        let result = Operators.int16 Int64.MaxValue
        Assert.AreEqual(-1s, result)

        // Overflow
        let result = Operators.int16 Int64.MinValue
        Assert.AreEqual(0s, result)

        // Overflow
        let result = Int16.MaxValue + 1s
        Assert.AreEqual(Int16.MinValue, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.int16 Decimal.MinValue |> ignore)

    [<Fact>]
    let int32() =
        // int
        let result = Operators.int32 10
        Assert.AreEqual(10, result)
        
        // double
        let result = Operators.int32 10.0
        Assert.AreEqual(10, result)
        
        // negative
        let result = Operators.int32 -10
        Assert.AreEqual(-10, result)
        
        // zero
        let result = Operators.int32 0
        Assert.AreEqual(0, result)
        
        // string
        let result = Operators.int32 "10"
        Assert.AreEqual(10, result)
        
        // Overflow
        let result = Operators.int32 Single.MaxValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int32 Single.MinValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int32 Double.MaxValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int32 Double.MinValue
        Assert.AreEqual(Int32.MinValue, result)
        
        // Overflow
        let result = Operators.int32 Int64.MaxValue
        Assert.AreEqual(-1, result)
        
        // Overflow
        let result = Operators.int32 Int64.MinValue
        Assert.AreEqual(0, result)

        // Overflow
        let result = Int32.MaxValue + 5
        Assert.AreEqual(Int32.MinValue + 4, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.int32 Decimal.MinValue |> ignore)

    [<Fact>]
    let int64() =
        // int
        let result = Operators.int64 10
        Assert.AreEqual(10L, result)
        
        // double
        let result = Operators.int64 10.0
        Assert.AreEqual(10L, result)
        
        // negative
        let result = Operators.int64 -10
        Assert.AreEqual(-10L, result)
        
        // zero
        let result = Operators.int64 0
        Assert.AreEqual(0L, result)
        
        // string
        let result = Operators.int64 "10"
        Assert.AreEqual(10L, result)
        
        // Overflow.
        let result = Operators.int64 Single.MaxValue
        Assert.AreEqual(Int64.MinValue, result)

        // Overflow
        let result = Operators.int64 Single.MinValue
        Assert.AreEqual(Int64.MinValue, result)
        
        // Overflow.
        let result = Operators.int64 Double.MaxValue
        Assert.AreEqual(Int64.MinValue, result)

        // Overflow
        let result = Operators.int64 Double.MinValue
        Assert.AreEqual(Int64.MinValue, result)

        // Overflow
        let result = Operators.int64 UInt64.MaxValue
        Assert.AreEqual(-1L, result)

        // max and min value as literals (this breaks compilation if the lexer fails)
        Assert.AreEqual(-9223372036854775808L, Int64.MinValue)
        Assert.AreEqual(9223372036854775807L, Int64.MaxValue)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.int64 Decimal.MinValue |> ignore)

    
    [<Fact>]
    let nativeint() =
        // int
        let result = Operators.nativeint 10
        Assert.AreEqual(10n, result)
        
        // double
        let result = Operators.nativeint 10.0
        Assert.AreEqual(10n, result)
        
        // int64
        let result = Operators.nativeint 10L
        Assert.AreEqual(10n, result)
       
        // negative
        let result = Operators.nativeint -10
        Assert.AreEqual(-10n, result)
        
        // zero
        let result = Operators.nativeint 0
        Assert.AreEqual(0n, result)
        
        // Overflow Double.MaxValue is equal on 32 bits and 64 bits runtimes
        let result = Operators.nativeint Single.MaxValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot use -9223372036854775808, compiler doesn't allow it, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual(-9223372036854775807n - 1n, result)
        
        // Overflow (depends on pointer size)
        let result = Operators.nativeint Single.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot use -9223372036854775808, compiler doesn't allow it, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual(-9223372036854775807n - 1n, result)
        
        // Overflow Double.MaxValue is equal on 32 bits and 64 bits runtimes
        let result = Operators.nativeint Double.MaxValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot use -9223372036854775808, compiler doesn't allow it, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual(-9223372036854775807n - 1n, result)
        
        // Overflow (depends on pointer size)
        let result = Operators.nativeint Double.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot use -9223372036854775808, compiler doesn't allow it, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual(-9223372036854775807n - 1n, result)
        
        // Overflow (depends on pointer size)
        let result = Operators.nativeint Int64.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(0n, result)
        else
            // Cannot use -9223372036854775808, compiler doesn't allow it, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual(-9223372036854775807n - 1n, result)

        // Overflow (depends on pointer size)
        if Info.isX86Runtime then
            let result = nativeint Int32.MaxValue + 5n
            Assert.AreEqual(-2147483644n, result)
        else
            let result = nativeint Int64.MaxValue + 5n
            Assert.AreEqual(-9223372036854775804n, result)

        // Overflow (depends on pointer size)
        let result = Operators.nativeint System.Double.MaxValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot express this as a literal, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual("-9223372036854775808", string result)

        let result = Operators.nativeint System.Double.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(-2147483648n, result)
        else
            // Cannot express this as a literal, see https://github.com/dotnet/fsharp/issues/9524
            Assert.AreEqual("-9223372036854775808", string result)

        // Max and min value as literals (this breaks compilation if the lexer fails).
        // The following tests ensure that the proper value is parsed, which is similar to `nativeint Int64.MaxValue` etc.
        if Info.isX86Runtime then
            Assert.AreEqual("0", string -9223372036854775808n)      // same as int32 -9223372036854775808L
            Assert.AreEqual("-1", string 9223372036854775807n)      // same as int32 9223372036854775807L
        else
            Assert.AreEqual("-9223372036854775808", string -9223372036854775808n)
            Assert.AreEqual("9223372036854775807", string 9223372036854775807n)

    
    [<Fact>]
    let sbyte() =
        // int
        let result = Operators.sbyte 10
        Assert.AreEqual(10y, result)
        
        // double
        let result = Operators.sbyte 10.0
        Assert.AreEqual(10y, result)
        
        // negative
        let result = Operators.sbyte -10
        Assert.AreEqual(-10y, result)
        
        // zero
        let result = Operators.sbyte 0
        Assert.AreEqual(0y, result)

        // Overflow
        let result = Operators.sbyte Int64.MaxValue
        Assert.AreEqual(-1y, result)
        
        // Overflow
        let result = Operators.sbyte Int64.MinValue
        Assert.AreEqual(0y, result)
        
        // Overflow
        let result = Operators.sbyte Single.MinValue
        Assert.AreEqual(0y, result)
        
        // Overflow
        let result = Operators.sbyte Single.MaxValue
        Assert.AreEqual(0y, result)
        
        // Overflow
        let result = Operators.sbyte Double.MinValue
        Assert.AreEqual(0y, result)
        
        // Overflow
        let result = Operators.sbyte Double.MaxValue
        Assert.AreEqual(0y, result)
        
        // Overflow
        let result = Operators.sbyte (Int64.MaxValue * 8L)
        Assert.AreEqual(-8y, result)      // bit-complement
        
        // Overflow
        let result = 127y + 1y
        Assert.AreEqual(-128y, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.sbyte Decimal.MinValue |> ignore)
        
    [<Fact>]
    let single() =
        // int
        let result = Operators.float32 10
        Assert.AreEqual(10f, result)
        
        // double
        let result = Operators.float32 10.0
        Assert.AreEqual(10f, result)
        
        // string
        let result = Operators.float32 "10"
        Assert.AreEqual(10f, result)

    
    [<Fact>]
    let uint16() =
        // int
        let result = Operators.uint16 100
        Assert.AreEqual(100us, result)
        
        // double
        let result = Operators.uint16 (100.0:double)
        Assert.AreEqual(100us, result)
        
        // decimal
        let result = Operators.uint16 100M
        Assert.AreEqual(100us, result)
        
        // Overflow
        let result = Operators.uint16 Single.MaxValue
        Assert.AreEqual(0us, result)
        
        // Overflow
        let result = Operators.uint16 Single.MinValue
        Assert.AreEqual(0us, result)
        
        // Overflow
        let result = Operators.uint16 Double.MaxValue
        Assert.AreEqual(0us, result)
        
        // Overflow
        let result = Operators.uint16 Double.MinValue
        Assert.AreEqual(0us, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.uint16 Decimal.MinValue |> ignore)
        
    [<Fact>]
    let uint32() =
        // int
        let result = Operators.uint32 100
        Assert.AreEqual(100u, result)
        
        // double
        let result = Operators.uint32 (100.0:double)
        Assert.AreEqual(100u, result)
        
        // decimal
        let result = Operators.uint32 100M
        Assert.AreEqual(100u, result)
        
        // Overflow
        let result = Operators.uint32 Single.MaxValue
        Assert.AreEqual(0u, result)
        
        // Overflow
        let result = Operators.uint32 Single.MinValue
        Assert.AreEqual(0u, result)
        
        // Overflow
        let result = Operators.uint32 Double.MaxValue
        Assert.AreEqual(0u, result)
        
        // Overflow
        let result = Operators.uint32 Double.MinValue
        Assert.AreEqual(0u, result)
        
        // Overflow
        let result = Operators.uint32 Int64.MaxValue
        Assert.AreEqual(UInt32.MaxValue, result)
        
        // Overflow
        let result = Operators.uint32 Int64.MinValue
        Assert.AreEqual(0u, result)

        // Overflow
        let result = UInt32.MaxValue + 5u
        Assert.AreEqual(4u, result)

        // both 'u' and 'ul' are valid numeric suffixes for UInt32
        let result = 42u + 42ul
        Assert.AreEqual(84u, result)
        Assert.AreEqual(84ul, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.uint32 Decimal.MinValue |> ignore)

    [<Fact>]
    let uint64() =
        // int
        let result = Operators.uint64 100
        Assert.AreEqual(100UL, result)
        
        // double
        let result = Operators.uint64 100.0
        Assert.AreEqual(100UL, result)
        
        // decimal
        let result = Operators.uint64 100M
        Assert.AreEqual(100UL, result)

        // Overflow
        let result = Operators.uint64 Single.MaxValue
        Assert.AreEqual(0UL, result)
        
        // Overflow
        let result = Operators.uint64 Single.MinValue
        Assert.AreEqual(9223372036854775808UL, result)      // surprising, but true, 2^63 + 1
        
        // Overflow
        let result = Operators.uint64 Double.MaxValue
        Assert.AreEqual(0UL, result)
        
        // Overflow
        let result = Operators.uint64 Double.MinValue
        Assert.AreEqual(9223372036854775808UL, result)      // surprising, but true, 2^63 + 1
        
        // Overflow
        let result = Operators.uint64 Int64.MinValue
        Assert.AreEqual(9223372036854775808UL, result)

        // Overflow
        let result = Operators.uint64 SByte.MinValue
        Assert.AreEqual(UInt64.MaxValue - 127UL, result)

        // Overflow
        let result = UInt64.MaxValue + 5UL
        Assert.AreEqual(4UL, result)

        // OverflowException, from decimal is always checked
        CheckThrowsOverflowException(fun () -> Operators.uint64 Decimal.MinValue |> ignore)
        
    [<Fact>]
    let unativeint() =
        // int
        let result = Operators.unativeint 100
        let x: unativeint = 12un
        Assert.AreEqual(100un, result)
        
        // double
        let result = Operators.unativeint 100.0
        Assert.AreEqual(100un, result)
        
        // Overflow Single.MaxValue is equal on 32 bits and 64 bits runtimes
        let result = Operators.unativeint Single.MaxValue
        Assert.AreEqual(0un, result)
        
        // Overflow (depends on pointer size)
        let result = Operators.unativeint Single.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(0un, result)
        else
            Assert.AreEqual(9223372036854775808un, result)      // surprising, but true, 2^63 + 1
        
        // Overflow Double.MaxValue is equal on 32 bits and 64 bits runtimes
        let result = Operators.unativeint Double.MaxValue
        Assert.AreEqual(0un, result)
        
        // Overflow (depends on pointer size)
        let result = Operators.unativeint Double.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(0un, result)
        else
            Assert.AreEqual(9223372036854775808un, result)      // surprising, but true, 2^63 + 1
        
        // Overflow (depends on pointer size)
        let result = Operators.unativeint Int64.MinValue
        if Info.isX86Runtime then
            Assert.AreEqual(0un, result)
        else
            Assert.AreEqual(9223372036854775808un, result)

        // Overflow (depends on pointer size)
        let result = 0un - 1un
        if Info.isX86Runtime then
            Assert.AreEqual(4294967295un, result)
        else
            Assert.AreEqual(18446744073709551615un, result)

    open Operators.Checked
    
    [<Fact>]
    let Checkedbyte() =
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
    let Checkedchar() =

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
    let CheckedInt() =

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
    let CheckedInt16() =

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
    let CheckedInt32() =

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
    let CheckedInt64() =

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
    let CheckedNativeint() =

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
        CheckThrowsOverflowException(fun () ->
            if Info.isX86Runtime then
                Operators.Checked.nativeint 2147483648.0 |> ignore
            else
                Operators.Checked.nativeint 9223372036854775808.0 |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.nativeint Single.MaxValue |> ignore)
        CheckThrowsOverflowException(fun () -> Operators.Checked.nativeint Double.MaxValue |> ignore)

         
    [<Fact>]
    let Checkedsbyte() =

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
    let Checkeduint16() =

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
    let Checkeduint32() =

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
    let Checkeduint64() =

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
    let Checkedunativeint() =

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

    type A = A
    type B() =
        static member op_Equality(_: B, _: B) = false
        static member op_Inequality(_: B, _: B) = true
    type [<Struct>] C =
        static member op_Equality(_: C, _: C) = true
        static member op_Inequality(_: C, _: C) = true
        static member op_Explicit(_: A) = C() // Explicit from another type
        static member op_Explicit(_: C) = B() // Explicit to another type
        static member op_Implicit(_: D) = C() // Duplicated implicit conversion
        static member op_Explicit(_: C) = { D = 0 } // Duplicated explicit conversion
    and D = { D : int } with
        static member op_Implicit(_: A) = { D = 0 } // Implicit from another type
        static member op_Implicit(_: D) = B() // Implicit to another type
        static member op_Implicit(_: D) = C() // Duplicated implicit conversion
        static member op_Explicit(_: C) = { D = 0 } // Duplicated explicit conversion
    let [<Fact>] Equality_ExplicitDynamicTests() =
        Assert.False(LanguagePrimitives.EqualityDynamic(B())(B()) : bool)
        Assert.True(LanguagePrimitives.InequalityDynamic(B())(B()) : bool)
        Assert.True(LanguagePrimitives.EqualityDynamic(C())(C()) : bool)
        Assert.True(LanguagePrimitives.InequalityDynamic(C())(C()) : bool)
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(A) : C)
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(A) : C) // Explicit from another type
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(C()) : B) // Explicit to another type
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic({ D = 0 }) : C) // Duplicated implicit conversion
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(C()) : D) // Duplicated explicit conversion
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(A) : D) // Implicit from another type
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic({ D = 0 }) : B) // Implicit to another type
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic({ D = 0 }) : C) // Duplicated implicit conversion
        Assert.NotNull(LanguagePrimitives.ExplicitDynamic(C()) : D) // Duplicated explicit conversion