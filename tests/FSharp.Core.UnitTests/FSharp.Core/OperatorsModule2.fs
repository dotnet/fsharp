// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Core.Operators module

namespace FSharp.Core.UnitTests.Operators

open System
open System.Text
open System.Globalization
open System.Threading

open FSharp.Core.UnitTests.LibraryTestFx

open Xunit

#nowarn "3370"

/// floating point helpers for min/max tests
module FP =
    let shouldBe (v, test) result = Assert.True(test result, $"Operators.max/min expected a %s{v}.")
    let positive = "positive zero float", (Double.IsNegative >> not)
    let positivef = "positive zero float32", (Single.IsNegative >> not)
    let negative = "negative zero float", Double.IsNegative
    let negativef = "positive zero float32", Single.IsNegative
    let positiveNaN = "positive NaN float", Double.IsNegative
    let positiveNaNf = "positive NaN float32", Single.IsNegative
    let negativeNaN = "positive NaN float", Double.IsNegative
    let negativeNaNf = "positive NaN float32", Single.IsNegative


/// If this type compiles without error it is correct
/// Wrong if you see: FS0670 This code is not sufficiently generic. The type variable ^T could not be generalized because it would escape its scope.
type TestFs0670Error<'T> =
    | TestFs0670Error of 'T
    override this.ToString() =
        match this with
        | TestFs0670Error x -> 
            // This used to raise FS0670 because the type is generic, and 'string' was inline
            // See: https://github.com/dotnet/fsharp/issues/7958
            Operators.string x

type OperatorsModule2() =

    [<Fact>]
    member _.int() =
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
        
        // Overflow.
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
    member _.int16() =
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
        
        // Overflow.
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
    member _.int32() =
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
        
        // Overflow.
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
    member _.int64() =
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
    member _.invalidArg() =
        CheckThrowsArgumentException(fun () -> Operators.invalidArg  "A" "B" |>ignore )

        
    [<Fact>]
    member _.lock() =
        // lock
        printfn "test8 started"
        let syncRoot = System.Object()
        let mutable k = 0
        let comp _ = async { return lock syncRoot (fun () -> k <- k + 1
                                                             System.Threading.Thread.Sleep(1)
                                                             k ) }
        let arr = Async.RunSynchronously (Async.Parallel(Seq.map comp [1..50]))
        Assert.AreEqual([|1..50|], Array.sort arr)
        
        // without lock
        let syncRoot = System.Object()
        let mutable k = 0
        let comp _ = async { do k <- k + 1
                             do! Async.Sleep (10)
                             return k }
        let arr = Async.RunSynchronously (Async.Parallel(Seq.map comp [1..100]))
        Assert.AreNotEqual ([|1..100|], Array.sort arr)
        
    [<Fact>]
    member _.log() =
        // double
        let result = Operators.log 10.0
        Assert.AreEqual(2.3025850929940459, result)
        
        // negative
        let result = Operators.log -10.0
        Assert.AreEqual(Double.NaN, result)
        
        // zero
        let result = Operators.log 0.0
        Assert.AreEqual(Double.NegativeInfinity , result)
        
    [<Fact>]
    member _.log10() =
        // double
        let result = Operators.log10 10.0
        Assert.AreEqual(1.0, result)
        
        // negative
        let result = Operators.log10 -10.0
        Assert.AreEqual(System.Double.NaN, result)
        
        // zero
        let result = Operators.log10 0.0
        Assert.AreEqual(Double.NegativeInfinity, result)
        
    [<Fact>]
    member _.max() =
        let shouldEqual a b = Assert.AreEqual(a, b)
        // value type
        Operators.max 10 8 |> shouldEqual 10

        // negative
        Operators.max -10.0M -8.0M |> shouldEqual -8.0M
        
        // zero
        Operators.max 0 0 |> shouldEqual 0

        // reference type
        Operators.max "A" "ABC" |> shouldEqual "ABC"

        // floating point
        // negative zero
        Operators.max 0.0 -0.0 |> FP.shouldBe FP.positive
        Operators.max -0.0 0.0 |> FP.shouldBe FP.positive
        Operators.max -1.0 0.0 |> FP.shouldBe FP.positive
        Operators.max -1.0 -0.0 |> FP.shouldBe FP.negative
        Operators.max 0.0f -0.0f |> FP.shouldBe FP.positivef
        Operators.max -0.0f 0.0f |> FP.shouldBe FP.positivef
        Operators.max -1.0f 0.0f |> FP.shouldBe FP.positivef
        Operators.max -1.0f -0.0f |> FP.shouldBe FP.negativef
        Operators.max -1.0 -2.0 |> shouldEqual -1.0
        Operators.max infinity -infinity |> shouldEqual infinity
        Operators.max infinityf -infinityf |> shouldEqual infinityf

        // note that the default nan is negative, using -nan (which does change 
        // the binary representation), makes nan positive
        let posNan, negNan, posNanf, negNanf = -nan, nan, -nanf, nanf
        Operators.max negNan 1.0      |> FP.shouldBe FP.negativeNaN
        Operators.max 1.0 negNan      |> FP.shouldBe FP.negativeNaN
        Operators.max negNan -1.0     |> FP.shouldBe FP.negativeNaN 
        Operators.max -1.0 negNan     |> FP.shouldBe FP.negativeNaN
        Operators.max 1.0f negNanf    |> FP.shouldBe FP.negativeNaNf
        Operators.max negNanf 1.0f    |> FP.shouldBe FP.negativeNaNf

        // truth table
        Operators.max negNan negNan   |> FP.shouldBe FP.negativeNaN
        Operators.max negNan posNan   |> FP.shouldBe FP.negativeNaN       // Bug in BCL: Math.Max returns first arg if it is any NaN
        Operators.max posNan negNan   |> FP.shouldBe FP.positiveNaN       // Bug in BCL: Math.Max returns first arg if it is any NaN
        Operators.max posNan posNan   |> FP.shouldBe FP.positiveNaN

        Operators.max negNanf negNanf |> FP.shouldBe FP.negativeNaNf
        Operators.max negNanf posNanf |> FP.shouldBe FP.negativeNaNf      // Bug in BCL: Math.Max returns first arg if it is any NaN
        Operators.max posNanf negNanf |> FP.shouldBe FP.positiveNaNf      // Bug in BCL: Math.Max returns first arg if it is any NaN
        Operators.max posNanf posNanf |> FP.shouldBe FP.negativeNaNf
        
    [<Fact>]
    member _.min() =
        let shouldEqual a b = Assert.AreEqual(a, b)

        // value type
        Operators.min 10 8 |> shouldEqual 8
        
        // negative
        Operators.min -10.0M -8.0M |> shouldEqual -10M
        
        // zero
        Operators.min 0 0 |> shouldEqual 0
        
        // reference type
        Operators.min "A" "ABC" |> shouldEqual "A"

        // floating point
        // negative zero
        Operators.min 0.0 -0.0 |> FP.shouldBe FP.negative
        Operators.min -0.0 0.0 |> FP.shouldBe FP.negative
        Operators.min 1.0 0.0 |> FP.shouldBe FP.positive
        Operators.min 1.0 -0.0 |> FP.shouldBe FP.negative
        Operators.min 0.0f -0.0f |> FP.shouldBe FP.negativef
        Operators.min -0.0f 0.0f |> FP.shouldBe FP.negativef
        Operators.min 1.0f 0.0f |> FP.shouldBe FP.positivef
        Operators.min 1.0f -0.0f |> FP.shouldBe FP.negativef
        Operators.min -1.0 -2.0 |> shouldEqual -2.0
        Operators.min infinity -infinity |> shouldEqual -infinity
        Operators.min infinityf -infinityf |> shouldEqual -infinityf

        // note that the default nan is negative, using -nan (which does change 
        // the binary representation), makes nan positive
        let posNan, negNan, posNanf, negNanf = -nan, nan, -nanf, nanf
        Operators.min negNan 1.0      |> FP.shouldBe FP.negativeNaN
        Operators.min 1.0 negNan      |> FP.shouldBe FP.negativeNaN
        Operators.min negNan -1.0     |> FP.shouldBe FP.negativeNaN 
        Operators.min -1.0 negNan     |> FP.shouldBe FP.negativeNaN
        Operators.min 1.0f negNanf    |> FP.shouldBe FP.negativeNaNf
        Operators.min negNanf 1.0f    |> FP.shouldBe FP.negativeNaNf

        // truth table
        Operators.min negNan negNan   |> FP.shouldBe FP.negativeNaN
        Operators.min negNan posNan   |> FP.shouldBe FP.negativeNaN       // Math.Min works like IEEE totalOrder
        Operators.min posNan negNan   |> FP.shouldBe FP.negativeNaN       // Math.Min works like IEEE totalOrder
        Operators.min posNan posNan   |> FP.shouldBe FP.positiveNaN

        Operators.min negNanf negNanf |> FP.shouldBe FP.negativeNaNf
        Operators.min negNanf posNanf |> FP.shouldBe FP.negativeNaNf      // Math.Min works like IEEE totalOrder
        Operators.min posNanf negNanf |> FP.shouldBe FP.negativeNaNf      // Math.Min works like IEEE totalOrder
        Operators.min posNanf posNanf |> FP.shouldBe FP.positiveNaNf
 

    [<Fact>]
    member _.nan() =
        // value type
        let result = Operators.nan
        Assert.AreEqual(System.Double.NaN, nan)
        
    [<Fact>]
    member _.nanf() =
        // value type
        let result = Operators.nanf
        Assert.AreEqual(System.Single.NaN, result)
        
    [<Fact>]
    member _.nativeint() =
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
        
        // Overflow Single.MaxValue is equal on 32 bits and 64 bits runtimes
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
    member _.not() =
        let result = Operators.not true
        Assert.False(result)
        
        let result = Operators.not false
        Assert.True(result)
        
    [<Fact>]
    member _.nullArg() =
        CheckThrowsArgumentNullException(fun () -> Operators.nullArg "A" |> ignore)

        
    [<Fact>]
    member _.pown() =
        // int
        let result = Operators.pown 10 2
        Assert.AreEqual(100, result)
        
        // double
        let result = Operators.pown 10.0 2
        Assert.AreEqual(100.0, result)
        
        // int64
        let result = Operators.pown 10L 2
        Assert.AreEqual(100L, result)
        
        // decimal
        let result = Operators.pown 10M 2
        Assert.AreEqual(100M, result)
        
        // negative
        let result = Operators.pown -10 2
        Assert.AreEqual(100, result)
        
        // zero
        let result = Operators.pown 0 2
        Assert.AreEqual(0, result)
        
        // overflow
        let result = Operators.pown System.Double.MaxValue System.Int32.MaxValue
        Assert.AreEqual(Double.PositiveInfinity, result)
        
        CheckThrowsOverflowException(fun () -> Operators.pown System.Int32.MaxValue System.Int32.MaxValue |>ignore)
        
    [<Fact>]
    member _.raise() =
        CheckThrowsArgumentException(fun () -> Operators.raise <| new ArgumentException("Invalid Argument ")  |> ignore)
        
    
    [<Fact>]
    member _.ref() =
        // value type
        let result = Operators.ref 0
        let funInt (x:int) =
            result := !result + x
            ()
        Array.iter funInt [|1..10|]
        Assert.AreEqual(!result,55)
        
        // reference type
        let result = Operators.ref ""
        let funStr (x : string) =
            result := (!result) + x
            ()
        Array.iter funStr [|"A";"B";"C";"D"|]
        Assert.AreEqual(!result,"ABCD")
        
    [<Fact>]
    member _.reraise() =
        // nothing to reraise should not trigger exception
        try
            ()
        with
        | _ ->    Operators.reraise()
        
    [<Fact>]
    member _.round() =
        // double
        let result = Operators.round 10.0
        Assert.AreEqual(10.0, result)
        
        // double
        let result = Operators.round 0.6640367702678489
        Assert.AreEqual(1.0, result)
        
        // double
        let result = Operators.round 0.6640367702678489e4
        Assert.AreEqual(6640.0, result)
        
        // double, show half-to-even
        let result = Operators.round 0.6640500000e4
        Assert.AreEqual(6640.0, result)
        
        // double, show half-to-even
        let result = Operators.round 0.6639500000e4
        Assert.AreEqual(6640.0, result)
        
        // double, show half-to-even
        let result = Operators.round 0.6641500000e4
        Assert.AreEqual(6642.0, result)
        
        // double, show rounding up if anything follows '5'
        let result = Operators.round 0.66405000001e4
        Assert.AreEqual(6641.0, result)
        
        // decimal
        let result = Operators.round 10M
        Assert.AreEqual(10M, result)
        
        // decimal, show half-to-even
        let result = Operators.round 1233.5M
        Assert.AreEqual(1234M, result)
        
        // decimal, show half-to-even
        let result = Operators.round 1234.5M
        Assert.AreEqual(1234M, result)
        
        // decimal, show half-to-even
        let result = Operators.round 1235.5M
        Assert.AreEqual(1236M, result)
        
        // decimal, show rounding up if anything follows '5'
        let result = Operators.round 1234.500000000001M
        Assert.AreEqual(1235M, result)
        
        // decimal, round up
        let result = Operators.round 1234.6M
        Assert.AreEqual(1235M, result)
        
    [<Fact>]
    member _.sbyte() =
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
    member _.sign() =
        // int
        let result = Operators.sign 10
        Assert.AreEqual(1, result)
        
        // negative int
        let result = Operators.sign -10
        Assert.AreEqual(-1, result)
        
        // zero int
        let result = Operators.sign 0
        Assert.AreEqual(0, result)

        // double
        let result = Operators.sign 10.0
        Assert.AreEqual(1, result)
        
        // double max
        let result = Operators.sign Double.MaxValue
        Assert.AreEqual(1, result)
        
        // double min
        let result = Operators.sign Double.MinValue
        Assert.AreEqual(-1, result)
        
        // double epsilon positive
        let result = Operators.sign Double.Epsilon
        Assert.AreEqual(1, result)
        
        // double epsilon negative
        let result = Operators.sign (-Double.Epsilon)
        Assert.AreEqual(-1, result)
        
        // double inf
        let result = Operators.sign Double.PositiveInfinity
        Assert.AreEqual(1, result)
                
        // double -inf
        let result = Operators.sign Double.NegativeInfinity
        Assert.AreEqual(-1, result)

        // float32
        let result = Operators.sign 10.0f
        Assert.AreEqual(1, result)
        
        // float32 max
        let result = Operators.sign Single.MaxValue
        Assert.AreEqual(1, result)
        
        // float32 min
        let result = Operators.sign Single.MinValue
        Assert.AreEqual(-1, result)
        
        // float32 epsilon positive
        let result = Operators.sign Single.Epsilon
        Assert.AreEqual(1, result)
        
        // float32 epsilon negative
        let result = Operators.sign (-Single.Epsilon)
        Assert.AreEqual(-1, result)
        
        // float32 inf
        let result = Operators.sign Single.PositiveInfinity
        Assert.AreEqual(1, result)
                
        // float32 -inf
        let result = Operators.sign Single.NegativeInfinity
        Assert.AreEqual(-1, result)

        // double nan
        CheckThrowsArithmeticException(fun () -> Operators.sign Double.NaN |> ignore)

        // float32 nan
        CheckThrowsArithmeticException(fun () -> Operators.sign Single.NaN |> ignore)
        
    [<Fact>]
    member _.sin() =
        
        let result = Operators.sin 0.5
        Assert.AreNearEqual(0.479425538604203, result)

        let result = Operators.sin Double.NaN
        Assert.AreEqual(Double.NaN, result)
        
        let result = Operators.sin Double.PositiveInfinity
        Assert.AreEqual(Double.NaN, result)
        
        let result = Operators.sin Double.NegativeInfinity
        Assert.AreEqual(Double.NaN, result)
        
    [<Fact>]
    member _.single() =
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
    member _.sinh() =
     
        let result = Operators.sinh 1.0
        Assert.AreNearEqual(1.1752011936438014, result)
        
        let result = Operators.sinh 0.0
        Assert.AreNearEqual(0.0, result)

        let result = Operators.sinh Double.PositiveInfinity
        Assert.AreNearEqual(Double.PositiveInfinity, result)

        let result = Operators.sinh Double.NegativeInfinity
        Assert.AreNearEqual(Double.NegativeInfinity, result)

        let result = Operators.sinh Double.NaN
        Assert.AreNearEqual(Double.NaN, result)

    [<Fact>]
    member _.sizeof() =
        // value type
        let result = Operators.sizeof<int>
        Assert.AreEqual(4, result)
        
        // System.Int64
        let result = Operators.sizeof<System.Int64>
        Assert.AreEqual(8, result)
        
        // custom struct
        let result = Operators.sizeof<System.ConsoleKeyInfo>
        Assert.AreEqual(12, result)
        
        // reference type should have the same size as the IntPtr
        let result = Operators.sizeof<string>
        Assert.AreEqual(IntPtr.Size, result)
        
        // null should have the same size as the IntPtr
        let result = Operators.sizeof<unit>
        Assert.AreEqual(IntPtr.Size, result)
        
    [<Fact>]
    member _.snd() =
        // value type
        let result = Operators.snd ("ABC",100)
        Assert.AreEqual(100, result)
        
        // reference type
        let result = Operators.snd (100,"ABC")
        Assert.AreEqual("ABC", result)
        
        // null
        let result = Operators.snd (100,null)
        Assert.AreEqual(null, result)
        
    [<Fact>]
    member _.sqrt() =
        // double
        let result = Operators.sqrt 100.0
        Assert.AreEqual(10.0, result)
        
        let result = Operators.sqrt -2.0
        Assert.AreEqual(Double.NaN, result)
        
    [<Fact>]
    member _.stderr() =
        let result = Operators.stderr
        Assert.AreEqual(null, result.WriteLine("go"))
        
    [<Fact>]
    member _.stdin() =
        let result = Operators.stdin
        Assert.AreEqual(null, result.Dispose())
        
    [<Fact>]
    member _.stdout() =
        let result = Operators.stdout
        Assert.AreEqual(null, result.WriteLine("go"))
        
    [<Fact>]
    member _.string() =

        let result = Operators.string null
        Assert.AreEqual("", result)

        let nullStr:string = null
        let result = Operators.string nullStr
        Assert.AreEqual("", result)

        let result = Operators.string null
        Assert.AreEqual("", result)

        let result = Operators.string (null:string)
        Assert.AreEqual("", result)

        let result = Operators.string (null:StringBuilder)
        Assert.AreEqual("", result)

        let result = Operators.string (null:IFormattable)
        Assert.AreEqual("", result)

        // value type
        let result = Operators.string 100
        Assert.AreEqual("100", result)
        
        // reference type
        let result = Operators.string "ABC"
        Assert.AreEqual("ABC", result)

        // reference type without a `ToString()` overload
        let result = Operators.string (obj())
        Assert.AreEqual("System.Object", result)

        let result = Operators.string 1un
        Assert.AreEqual("1", result)

        let result = Operators.string (obj())
        Assert.AreEqual("System.Object", result)

        let result = Operators.string 123.456M
        Assert.AreEqual("123.456", result)

        // Following tests ensure that InvariantCulture is used if type implements IFormattable
        
        // safe current culture, then switch culture
        let currentCI = Thread.CurrentThread.CurrentCulture
        Thread.CurrentThread.CurrentCulture <- CultureInfo.GetCultureInfo("de-DE")

        // make sure the culture switch happened, and verify
        let wrongResult = 123.456M.ToString()
        Assert.AreEqual("123,456", wrongResult)

        // test that culture has no influence on decimals with `string`
        let correctResult = Operators.string 123.456M
        Assert.AreEqual("123.456", correctResult)

        // make sure that the German culture is indeed selected for DateTime
        let dttm = DateTime(2020, 6, 23)
        let wrongResult = dttm.ToString()
        Assert.AreEqual("23.06.2020 00:00:00", wrongResult)

        // test that culture has no influence on DateTime types when used with `string`
        let correctResult = Operators.string dttm
        Assert.AreEqual("06/23/2020 00:00:00", correctResult)

        // reset the culture
        Thread.CurrentThread.CurrentCulture <- currentCI



    [<Fact>]
    member _.``string: don't raise FS0670 anymore``() =
        // The type used here, when compiled, should not raise this error:
        // "FS0670 This code is not sufficiently generic. The type variable ^T could not be generalized because it would escape its scope."
        // See: https://github.com/dotnet/fsharp/issues/7958
        let result = TestFs0670Error 32uy |> Operators.string
        Assert.AreEqual("32", result)
        
    [<Fact>]
    member _.tan() =
        // double
        let result = Operators.tan 1.0
        Assert.AreNearEqual(1.5574077246549023, result)
        
    [<Fact>]
    member _.tanh() =
        // The x86 runtime uses 64 bit precision, whereas the x64 runtime uses SSE instructions with 80 bit precision
        // details can be found here: https://github.com/dotnet/fsharp/issues/9522
        let result = Operators.tanh 0.8
        Assert.AreNearEqual(0.66403677026784902, result)

        let result = Operators.tanh 19.06154
        Assert.AreNearEqual(1.0, result)        // can be 0.99999999999999989

        let result = tanh 0.0
        Assert.AreEqual(0.0, result)

        let result = tanh infinity
        Assert.AreEqual(1.0, result)

        let result = tanh -infinity
        Assert.AreEqual(-1.0, result)
        
    [<Fact>]
    member _.truncate() =
        // double
        let result = Operators.truncate 10.101
        Assert.AreEqual(10.0, result)
        
        // decimal
        let result = Operators.truncate 10.101M
        Assert.AreEqual(10M, result)
        
        // zero
        let result = Operators.truncate 0.101
        Assert.AreEqual(0.0, result)
        
    [<Fact>]
    member _.typedefof() =
        // value type
        let result = Operators.typedefof<int>
        Assert.AreEqual("System.Int32", result.FullName)
        
        // reference type
        let result = Operators.typedefof<string>
        Assert.AreEqual("System.String", result.FullName)
        
        // unit
        let result = Operators.typedefof<unit>
        Assert.AreEqual("Microsoft.FSharp.Core.Unit", result.FullName)
        
    [<Fact>]
    member _.typeof() =
        // value type
        let result = Operators.typeof<int>
        Assert.AreEqual("System.Int32", result.FullName)
        
        // reference type
        let result = Operators.typeof<string>
        Assert.AreEqual("System.String", result.FullName)
        
        // unit
        let result = Operators.typeof<unit>
        Assert.AreEqual("Microsoft.FSharp.Core.Unit", result.FullName)
        
    [<Fact>]
    member _.uint16() =
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
    member _.uint32() =
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
    member _.uint64() =
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
    member _.unativeint() =
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

    [<Fact>]
    member _.unbox() =
        // value type
        let oint = box 100
        let result = Operators.unbox oint
        Assert.AreEqual(100, result)
        
        // reference type
        let ostr = box "ABC"
        let result = Operators.unbox ostr
        Assert.AreEqual("ABC", result)
        
        // null
        let onull = box null
        let result = Operators.unbox onull
        Assert.AreEqual(null, result)

        // None == null
        let onone = box None
        let result = Operators.unbox<int option> onone
        Assert.AreEqual(None, result)
        Assert.AreEqual(null, result)
        
    [<Fact>]
    member _.using() =
        let sr = new System.IO.StringReader("ABCD")
        Assert.AreEqual(sr.ReadToEnd(),"ABCD")
        let _ = Operators.using sr (fun x -> x.ToString())
        CheckThrowsObjectDisposedException(fun () -> sr.ReadToEnd() |> ignore)
    