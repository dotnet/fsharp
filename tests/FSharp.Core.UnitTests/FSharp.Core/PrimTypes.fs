// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Core.LanguagePrimitives module

namespace FSharp.Core.UnitTests

open System
open System.Numerics 
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

[<Measure>]
type m

type LanguagePrimitivesModule() =

    [<Fact>]
    member _.CastingUint () =
        let expected = 12u
        let actual = uint 12
        Assert.AreEqual(expected, actual)

    [<Fact>]
    member this.CastingUnits() =
        let f = 2.5
        Assert.AreEqual(f, f |> LanguagePrimitives.FloatWithMeasure<m> |> float)

        let ff= 2.5f
        Assert.AreEqual(ff, ff |> LanguagePrimitives.Float32WithMeasure<m> |> float32)

        let d = 2.0m
        Assert.AreEqual(d, d |> LanguagePrimitives.DecimalWithMeasure<m> |> decimal)

        let i = 2
        Assert.AreEqual(i, i |> LanguagePrimitives.Int32WithMeasure<m> |> int)

        let l = 2L
        Assert.AreEqual(l, l |> LanguagePrimitives.Int64WithMeasure<m> |> int64)

        let s = 2s
        Assert.AreEqual(s, s |> LanguagePrimitives.Int16WithMeasure<m> |> int16)

        let y = 2y
        Assert.AreEqual(y, y |> LanguagePrimitives.SByteWithMeasure<m> |> sbyte)
        
        let n = 2n
        Assert.AreEqual(n, n |> LanguagePrimitives.IntPtrWithMeasure<m> |> nativeint)
        
        let i = 2u
        Assert.AreEqual(i, i |> LanguagePrimitives.UInt32WithMeasure<m> |> uint)

        let l = 2UL
        Assert.AreEqual(l, l |> LanguagePrimitives.UInt64WithMeasure<m> |> uint64)

        let s = 2us
        Assert.AreEqual(s, s |> LanguagePrimitives.UInt16WithMeasure<m> |> uint16)
        
        let uy = 2uy
        Assert.AreEqual(uy, uy |> LanguagePrimitives.ByteWithMeasure<m> |> byte)

        let n = 2un
        Assert.AreEqual(n, n |> LanguagePrimitives.UIntPtrWithMeasure<m> |> unativeint)

    [<Fact>]
    member _.MeasurableAliases() =
        let f (x: int<m>) y: int32<m> = x + y   // should be: `int<m> -> int<m> -> int32<m>`
        let g (x: int<m>) (y:int32<m>) = x * y  // should be: `int<m> -> int32<m> -> int<m^2>`
        let h (x: int<m>) y = x * y
        let i (x: int32<m>) y = x * y
        
        let tres = 3<m>
        let ocho : int32<m> = 8<m>
        
        Assert.Equal(ocho, f tres 5<m>)
        Assert.Equal(64<m^2>, g ocho ocho)
        Assert.Equal(h ocho tres, i tres ocho)

    [<Fact>]
    member this.MaxMinNan() =
        Assert.True(Double.IsNaN(max nan 1.0))
        Assert.True(Double.IsNaN(max 1.0 nan))
        Assert.True(Double.IsNaN(max nan nan))

        Assert.True(Single.IsNaN(max Single.NaN 1.0f))
        Assert.True(Single.IsNaN(max 1.0f Single.NaN))
        Assert.True(Single.IsNaN(max Single.NaN Single.NaN))
        
        Assert.True(Double.IsNaN(min nan 1.0))
        Assert.True(Double.IsNaN(min 1.0 nan))
        Assert.True(Double.IsNaN(min nan nan))

        Assert.True(Single.IsNaN(min Single.NaN 1.0f))
        Assert.True(Single.IsNaN(min 1.0f Single.NaN))
        Assert.True(Single.IsNaN(min Single.NaN Single.NaN))

    [<Fact>]
    member this.DivideByInt() =
        // float32 
        let resultFloat32 = LanguagePrimitives.DivideByInt 3.0f 3
        Assert.AreEqual(1.0f, resultFloat32)
        
        // double 
        let resultDouble = LanguagePrimitives.DivideByInt 3.9 3
        Assert.AreEqual(1.3, resultDouble)
        
        // decimal 
        let resultDecimal = LanguagePrimitives.DivideByInt 3.9M 3
        Assert.AreEqual(1.3M, resultDecimal)   

    [<Fact>]
    member this.EnumOfValue() =  
        let monday = System.DayOfWeek.Monday
        let result = LanguagePrimitives.EnumOfValue<int,System.DayOfWeek>(1)
        
        Assert.AreEqual(monday, result)
    
    [<Fact>]
    member this.EnumToValue() =
        let monday = System.DayOfWeek.Monday
        let result = LanguagePrimitives.EnumToValue monday

        Assert.AreEqual(1, result)
        
    [<Fact>]
    member this.GuidToString() =
        let s = "F99D95E0-2A5E-47c4-9B92-6661D65AE6B3"
        let guid = new Guid(s)
        Assert.AreEqual((string guid).ToLowerInvariant(), s.ToLowerInvariant())

    [<Fact>]
    member this.GenericComparison() =
        // value type
        let resultValue = LanguagePrimitives.GenericComparison 1 1
        Assert.AreEqual(0, resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericComparison "ABC" "ABCDE"
        Assert.AreEqual(-1, sign resultRef)
        
        // null reference
        let resultRef = LanguagePrimitives.GenericComparison "ABC" null
        Assert.AreEqual(1, resultRef)


#if NETSTANDARD1_6 || NETCOREAPP
// TODO named #define ?
#else  
    [<Fact>]
    member this.GenericComparisonBiModal() =
        // value type
        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 100 1
        Assert.AreEqual(1, resultValue)

        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 1 1
        Assert.AreEqual(0, resultValue)

        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 1 200
        Assert.AreEqual(-1, resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABCDE" "ABC"
        Assert.AreEqual(1, sign resultRef)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABC" "ABC"
        Assert.AreEqual(0, sign resultRef)
        
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "abc" "abcde"
        Assert.AreEqual(-1, sign resultRef)
        
        // null reference
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABC" null
        Assert.AreEqual(1,sign resultRef)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default null null
        Assert.AreEqual(0, sign resultRef)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default null "abc"
        Assert.AreEqual(-1, sign resultRef)
        
#endif
        
    [<Fact>]
    member this.GenericEquality() =
        // value type
        let resultValue = LanguagePrimitives.GenericEquality 1 1
        Assert.True(resultValue)

        let resultValue = LanguagePrimitives.GenericEquality 1 2
        Assert.False(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericEquality "ABC" "ABC"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericEquality "ABC" "ABCDE"
        Assert.False(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericEquality null null
        Assert.True(resultNul)

        let resultNul = LanguagePrimitives.GenericEquality "ABC" null
        Assert.False(resultNul)
        
    [<Fact>]
    member this.GenericGreaterOrEqual() =
        // value type
        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 1 1
        Assert.True(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 2 1
        Assert.True(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 1 2
        Assert.False(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "abcde" "abc"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "ABCDE" "ABCDE"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "ABC" "ABCDE"
        Assert.False(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericGreaterOrEqual null null
        Assert.True(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterOrEqual null "ABCDE"
        Assert.False(resultNul)
        
        
    [<Fact>]
    member this.GenericGreaterThan() =
        // value type
        let resultValue = LanguagePrimitives.GenericGreaterThan 1 1
        Assert.False(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterThan 2 1
        Assert.True(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericGreaterThan "ABC" "ABCDE"
        Assert.False(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterThan "ABCDE" "ABCDE"
        Assert.False(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterThan "abc" "a"
        Assert.True(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericGreaterThan null null
        Assert.False(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterThan null "ABC"
        Assert.False(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterThan "ABC" null
        Assert.True(resultNul)
        
        
    [<Fact>]
    member this.GenericHash() =
        // value type
        let resultValue = LanguagePrimitives.GenericHash 1 
        Assert.AreEqual(1, resultValue)

         // using standard .NET GetHashCode as oracle
        let resultValue = LanguagePrimitives.GenericHash 1000 
        let x = 1000
        Assert.AreEqual(x.GetHashCode(), resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericHash "ABC" 
        Assert.AreEqual("ABC".GetHashCode(), resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericHash null 
        Assert.AreEqual(0, resultNul)
        
        
    [<Fact>]
    member this.GenericLessOrEqual() =
        // value type
        let resultValue = LanguagePrimitives.GenericLessOrEqual 1 1
        Assert.True(resultValue)

        let resultValue = LanguagePrimitives.GenericLessOrEqual 1 2
        Assert.True(resultValue)

        let resultValue = LanguagePrimitives.GenericLessOrEqual -1 0
        Assert.True(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericLessOrEqual "ABC" "ABCDE"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericLessOrEqual "ABCDE" "ABCDE"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericLessOrEqual "abcde" "abc"
        Assert.False(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericLessOrEqual null null
        Assert.True(resultNul)

        let resultNul = LanguagePrimitives.GenericLessOrEqual null "abc"
        Assert.True(resultNul)

        let resultNul = LanguagePrimitives.GenericLessOrEqual "abc" null
        Assert.False(resultNul)
        
        
    [<Fact>]
    member this.GenericLessThan() =
        // value type
        let resultValue = LanguagePrimitives.GenericLessThan 1 1
        Assert.False(resultValue)

        let resultValue = LanguagePrimitives.GenericLessThan -2 -4
        Assert.False(resultValue)

        let resultValue = LanguagePrimitives.GenericLessThan 1 2
        Assert.True(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericLessThan "ABC" "ABCDE"
        Assert.True(resultRef)

        let resultRef = LanguagePrimitives.GenericLessThan "ABC" "ABC"
        Assert.False(resultRef)

        let resultRef = LanguagePrimitives.GenericLessThan "abc" "a"
        Assert.False(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericLessThan null "abc"
        Assert.True(resultNul)

        let resultNul = LanguagePrimitives.GenericLessThan "aa" null
        Assert.False(resultNul)

        let resultNul = LanguagePrimitives.GenericLessThan null null
        Assert.False(resultNul)

    [<Fact>]
    member this.GenericMaximum() =
        // value type
        let resultValue = LanguagePrimitives.GenericMaximum 8 9
        Assert.AreEqual(9, resultValue)

        let resultValue = LanguagePrimitives.GenericMaximum -800 -900
        Assert.AreEqual(-800, resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericMaximum "ABC" "ABCDE"
        Assert.AreEqual("ABCDE", resultRef) 
        
        let resultRef = LanguagePrimitives.GenericMaximum "ABCDE" "ABC" 
        Assert.AreEqual("ABCDE", resultRef)
        
        
        // null reference
        let resultNul = LanguagePrimitives.GenericMaximum null null
        Assert.Null(resultNul)

        let resultNul = LanguagePrimitives.GenericMaximum null "ABCDE"
        Assert.AreEqual("ABCDE", resultNul)

        let resultNul = LanguagePrimitives.GenericMaximum "ABCDE" null
        Assert.AreEqual("ABCDE", resultNul)
        
    [<Fact>]
    member this.GenericMinimum() =
        // value type
        let resultValue = LanguagePrimitives.GenericMinimum 8 9
        Assert.AreEqual(8, resultValue)

        let resultValue = LanguagePrimitives.GenericMinimum -800 -900
        Assert.AreEqual(-900, resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericMinimum "ABC" "ABCDE"
        Assert.AreEqual("ABC", resultRef)

        let resultRef = LanguagePrimitives.GenericMinimum "abcde" "abc"
        Assert.AreEqual("abc", resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericMinimum null null
        Assert.Null(resultNul)

        let resultNul = LanguagePrimitives.GenericMinimum null "ABC"
        Assert.Null(resultNul)

        let resultNul = LanguagePrimitives.GenericMinimum "ABC" null
        Assert.Null(resultNul)
        
    [<Fact>]
    member this.GenericOne() =
        // int type
        let resultValue = LanguagePrimitives.GenericOne<int> 
        Assert.AreEqual(1, resultValue)

        // float type
        let resultValue = LanguagePrimitives.GenericOne<float> 
        Assert.AreEqual(1., resultValue)

        // bigint type
        let resultValue = LanguagePrimitives.GenericOne<bigint> 
        Assert.AreEqual(1I, resultValue)
        
    [<Fact>]
    member this.GenericZero() =
        // int type
        let resultValue = LanguagePrimitives.GenericZero<int> 
        Assert.AreEqual(0, resultValue)

        // float type
        let resultValue = LanguagePrimitives.GenericZero<float> 
        Assert.AreEqual(0., resultValue)

        // bigint type
        let resultValue = LanguagePrimitives.GenericZero<bigint> 
        Assert.AreEqual(0I, resultValue)
        
    [<Fact>]
    member this.ParseInt32() =
        let resultValue = LanguagePrimitives.ParseInt32 "100" 
        Assert.AreEqual(typeof<int>, resultValue.GetType())
        Assert.AreEqual(100, resultValue)    

        let resultValue = LanguagePrimitives.ParseInt32 "-123" 
        Assert.AreEqual(-123, resultValue)    

        let resultValue = LanguagePrimitives.ParseInt32 "0" 
        Assert.AreEqual(0, resultValue)    

        
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseInt32 "-100000000000000000" |> ignore)

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseInt32 "" |> ignore)    
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseInt32 null  |> ignore)
        
    [<Fact>]
    member this.ParseInt64() =
        let resultValue = LanguagePrimitives.ParseInt64 "100" 
        Assert.AreEqual(typeof<int64>, resultValue.GetType())    
        Assert.AreEqual(100L, resultValue)   

        let resultValue = LanguagePrimitives.ParseInt64 "-100000000000000000" 
        Assert.AreEqual(-100000000000000000L, resultValue)   

        let resultValue = LanguagePrimitives.ParseInt64 "0" 
        Assert.AreEqual(0L, resultValue)


        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseInt64 "9223372036854775808" |> ignore)

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseInt64 "" |> ignore)    
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseInt64 null  |> ignore)

    [<Fact>]
    member this.ParseBinaryInt64() =
        let resultValue = LanguagePrimitives.ParseInt64 "0b1100100" 
        Assert.AreEqual(typeof<int64>, resultValue.GetType())    
        Assert.AreEqual(100L, resultValue)   

        let resultValue = LanguagePrimitives.ParseInt64 "-0b101100011010001010111100001011101100010100000000000000000"
        Assert.AreEqual(-100000000000000000L, resultValue)

        let resultValue = LanguagePrimitives.ParseInt64 "0b1111111010011100101110101000011110100010011101100000000000000000"
        Assert.AreEqual(-100000000000000000L, resultValue)

        let resultValue = LanguagePrimitives.ParseInt64 "0b0"
        Assert.AreEqual(0L, resultValue)

        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseInt64 "0b10000000000000000000000000000000000000000000000000000000000000000" |> ignore)

    [<Fact>]
    member this.ParseOctalInt64() =
        let resultValue = LanguagePrimitives.ParseInt64 "0o144"
        Assert.AreEqual(typeof<int64>, resultValue.GetType())
        Assert.AreEqual(100L, resultValue)   

        let resultValue = LanguagePrimitives.ParseInt64 "-0o5432127413542400000"
        Assert.AreEqual(-100000000000000000L, resultValue)

        let resultValue = LanguagePrimitives.ParseInt64 "0o1772345650364235400000"
        Assert.AreEqual(-100000000000000000L, resultValue)

        let resultValue = LanguagePrimitives.ParseInt64 "0o0"
        Assert.AreEqual(0L, resultValue)

        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseInt64 "0o2000000000000000000000" |> ignore)

    [<Fact>]
    member this.ParseUInt32() =
        let resultValue = LanguagePrimitives.ParseUInt32 "100" 
        Assert.AreEqual(typeof<uint32>, resultValue.GetType())   
        Assert.AreEqual(100ul, resultValue)        
        
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt32 "-1" |> ignore)
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseUInt32 null  |> ignore)
        
    [<Fact>]
    member this.ParseUInt64() =
        let resultValue = LanguagePrimitives.ParseUInt64 "100" 
        Assert.AreEqual(typeof<uint64>, resultValue.GetType()) 
        Assert.AreEqual(100UL, resultValue)        

        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt64 "-1" |> ignore)
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt64 "18446744073709551616" |> ignore)
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseUInt64 null  |> ignore)

    [<Fact>]
    member this.ParseBinaryUInt64() =
        let resultValue = LanguagePrimitives.ParseUInt64 "0b1100100" 
        Assert.AreEqual(typeof<uint64>, resultValue.GetType()) 
        Assert.AreEqual(100UL, resultValue)        

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseUInt64 "-0b1" |> ignore)
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt64 "0b10000000000000000000000000000000000000000000000000000000000000000" |> ignore)

    [<Fact>]
    member this.ParseOctalUInt64() =
        let resultValue = LanguagePrimitives.ParseUInt64 "0o144" 
        Assert.AreEqual(typeof<uint64>, resultValue.GetType()) 
        Assert.AreEqual(100UL, resultValue)        

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseUInt64 "-0o1" |> ignore)
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt64 "0o2000000000000000000000" |> ignore)

    [<Fact>]
    member this.ParseStringViaConversionOps() =
        let s : string = null
        CheckThrowsArgumentNullException2 "sbyte" (fun () -> sbyte s |> ignore)
        CheckThrowsArgumentNullException2 "byte" (fun () -> byte s |> ignore)
        CheckThrowsArgumentNullException2 "int16" (fun () -> int16 s |> ignore)
        CheckThrowsArgumentNullException2 "uint16 " (fun () -> uint16 s |> ignore)
        CheckThrowsArgumentNullException2 "int" (fun () -> int s |> ignore)
        CheckThrowsArgumentNullException2 "int32" (fun () -> int32 s |> ignore)
        CheckThrowsArgumentNullException2 "uint32" (fun () -> uint32 s |> ignore)
        CheckThrowsArgumentNullException2  "int64" (fun () -> int64 s |> ignore)
        CheckThrowsArgumentNullException2 "uint64" (fun () -> uint64 s |> ignore)
        CheckThrowsArgumentNullException2 "float32" (fun () -> float32 s |> ignore)
        CheckThrowsArgumentNullException2 "float" (fun () -> float s |> ignore)
        CheckThrowsArgumentNullException2 "decimal" (fun () -> decimal s |> ignore)
        CheckThrowsArgumentNullException2 "char" (fun () -> char s |> ignore)

    [<Fact>]
    member this.PhysicalEquality() =
        // revordtype
        let ref1 = ref 8
        let ref2 = ref 8
        let resultValue = LanguagePrimitives.PhysicalEquality ref1 ref2
        Assert.False(resultValue)
        Assert.True(LanguagePrimitives.PhysicalEquality ref1 ref1)
        Assert.True(LanguagePrimitives.PhysicalEquality ref2 ref2)
        
        // reference type
        let resultRef0 = LanguagePrimitives.PhysicalEquality "ABC" "ABC"
        Assert.True(resultRef0)
        
        let resultRef1 = LanguagePrimitives.PhysicalEquality "ABC" "DEF"
        Assert.False(resultRef1)
        
        // object type
        let resultRef2 = LanguagePrimitives.PhysicalEquality (obj()) (obj())
        Assert.False(resultRef2)
        
        // object type
        let o = obj()
        let resultRef3 = LanguagePrimitives.PhysicalEquality o o 
        Assert.True(resultRef3)
        
        // System.ValueType type
        let resultRef4 = LanguagePrimitives.PhysicalEquality (1 :> System.ValueType) (1 :> System.ValueType)
        Assert.False(resultRef4)
        
        // System.ValueType type
        let resultRef5 = LanguagePrimitives.PhysicalEquality (1 :> System.ValueType) (2 :> System.ValueType)
        Assert.False(resultRef5)
        
        // null reference
        let resultNul = LanguagePrimitives.PhysicalEquality null null
        Assert.True(resultNul)


type HashCompareModule() = // this module is internal/obsolete, but contains code reachable from many public APIs
    member inline this.ComparisonsFor< ^T when ^T : comparison>(x : ^T, y : ^T) =
        Assert.True( x < y )
        Assert.True( y > x ) 
        Assert.True( (x = x) )
        Assert.False( y < x )
        Assert.False( x > y )
        Assert.False( (x = y) )

    [<Fact>]
    member this.ComparisonsForArraysOfNativeInts() =
        this.ComparisonsFor( [|0n|], [|1n|] )
        this.ComparisonsFor( [|0un|], [|1un|] )

    [<Fact>]
    member this.ComparisonsForArraysOfFloatingPoints() =
        this.ComparisonsFor( [|0.0|], [|1.0|] )
        this.ComparisonsFor( [|0.0f|], [|1.0f|] )
        Assert.False( [| System.Double.NaN |] = [| System.Double.NaN |] )
        Assert.False( [| System.Single.NaN |] = [| System.Single.NaN |] )
        Assert.False( [| System.Double.NaN |] < [| System.Double.NaN |] )
        Assert.False( [| System.Single.NaN |] < [| System.Single.NaN |] )

    [<Fact>]
    member this.ComparisonsForOtherArrays() =
        this.ComparisonsFor( [|0uy|], [|1uy|] )
        this.ComparisonsFor( [|'a'|], [|'b'|] )
        this.ComparisonsFor( [|0UL|], [|1UL|] )

    [<Fact>]
    member this.ComparisonsForStrings() =
        this.ComparisonsFor( "bar", "foo" )
        this.ComparisonsFor( [| "bar" |], [| "foo" |] )

    [<Fact>]
    member this.ComparisonsForMultidimensionalIntArrays() =
        let N = 10
        let M = 100
        let Z = 9999
        let x = Array2D.init 3 3 (fun x y -> N*x + y)
        let y = Array2D.init 3 3 (fun x y -> N*x + y)
        Assert.AreEqual(hash x, hash y)

        y.[2,2] <- Z
        this.ComparisonsFor( x, y )

        let x = Array3D.init 3 3 3 (fun x y z -> M*x + N*y + z)
        let y = Array3D.init 3 3 3 (fun x y z -> M*x + N*y + z)
        Assert.AreEqual(hash x, hash y)

        y.[2,2,2] <- Z
        this.ComparisonsFor( x, y )

    [<Fact>]
    member this.ComparisonsForMultidimensionalInt64Arrays() =
        let N = 10L
        let M = 100L
        let Z = 9999L
        let x = Array2D.init 3 3 (fun x y -> N*(int64 x) + (int64 y))
        let y = Array2D.init 3 3 (fun x y -> N*(int64 x) + (int64 y))
        Assert.AreEqual(hash x, hash y)

        y.[2,2] <- Z
        this.ComparisonsFor( x, y )

        let x = Array3D.init 3 3 3 (fun x y z -> M*(int64 x) + N*(int64 y) + (int64 z))
        let y = Array3D.init 3 3 3 (fun x y z -> M*(int64 x) + N*(int64 y) + (int64 z))
        Assert.AreEqual(hash x, hash y)

        y.[2,2,2] <- Z
        this.ComparisonsFor( x, y )
    
    [<Fact>]
    member this.MonsterTuple() =
        let mt = 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        let mt2 = 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        Assert.AreEqual(mt,mt2)


type UnitType() =

    // interface
    [<Fact>]
    member this.IComparable() =        
        let u:Unit = ()
        // value type
        let ic = u :> IComparable   
        CheckThrowsNullRefException(fun() ->ic.CompareTo(3) |>ignore) 
        
    // Base class methods
    [<Fact>]
    member this.ObjectGetHashCode() =
        let u:Unit = ()
        CheckThrowsNullRefException(fun() ->u.GetHashCode() |>ignore) 
        
    [<Fact>]
    member this.ObjectEquals() =
        let u:Unit = ()
        CheckThrowsNullRefException(fun() ->u.Equals(null) |>ignore) 

#if NETSTANDARD1_6 || NETCOREAPP
// TODO named #define ?
#else
type SourceConstructFlagsEnum() =

    [<Fact>]
    member this.Getvalue() =
        let names = [| "None";"SumType";"RecordType";"ObjectType";"Field";
                       "Exception";"Closure";"Module";"UnionCase";"Value";
                       "KindMask";"NonPublicRepresentation" |]
        Assert.AreEqual(names, SourceConstructFlags.GetNames(typeof<SourceConstructFlags>))


type CompilationRepresentationFlagsEnum() =

    [<Fact>]
    member this.Getvalue() =
        let names = [| "None";"Static";"Instance";"ModuleSuffix";"UseNullAsTrueValue";"Event" |]
        Assert.AreEqual(names, SourceConstructFlags.GetNames(typeof<CompilationRepresentationFlags>))
#endif


type MiscStuff() =

    [<Fact>]
    member this.ListToString() =
        Assert.AreEqual("[]", [].ToString())
        Assert.AreEqual("[1]", [1].ToString())
        Assert.AreEqual("[1; 2]", [1;2].ToString())
        Assert.AreEqual("[1; 2; 3]", [1;2;3].ToString())
        Assert.AreEqual("[1; 2; 3; ... ]", [1;2;3;4].ToString())

    [<Fact>]
    member this.Refs() =
        let x = ref 0
        incr x
        incr x
        decr x
        Assert.True( 1 = !x )


type UnboxAndOptionStuff() =
    [<Fact>]
    member this.TryUnbox() =
        Assert.True( tryUnbox (box ([] : int list)) = Some ([]: int list))
        Assert.True( tryUnbox (box ([1] : int list)) = Some ([1]: int list))
        Assert.True( tryUnbox (box ([] : string list)) = (None : int list option)) // Option uses 'null' as representation
        Assert.True( tryUnbox<int list> (box ([] : string list)) = None)
        Assert.True( tryUnbox (box (None : int option)) = Some (None: int option))
        Assert.True( tryUnbox (box (None : string option)) = Some (None: string option))
        Assert.True( tryUnbox (box (None : string option)) = Some (None: int option)) // Option uses 'null' as representation
        Assert.True( tryUnbox (box "") = Some "")
        Assert.True( tryUnbox<int option> (box null) = Some None) // Option uses 'null' as representation
        Assert.True( tryUnbox<int list> (box null) = None)
        Assert.True( tryUnbox<int> (box null) = None)
        Assert.True( tryUnbox<int> (box "1") = None)
        Assert.True( tryUnbox<int> (box 1) = Some 1)
        Assert.True( tryUnbox<string> (box "") = Some "")
        Assert.True( tryUnbox<string> (box 1) = None)

    [<Fact>]
    member this.IsNull() =
        Assert.True( isNull (null : string))
        Assert.True( isNull (null : string[]))
        Assert.True( isNull (null : int[]))
        Assert.True( not (isNull [| |]))
        Assert.True( not (isNull ""))
        Assert.True( not (isNull "1"))


module internal RangeTestsHelpers =
    // strictly speaking, this is just undefined behaviour, but at some point the F# library decided that
    // it was an exception, so we are ensuring that such behaviour is retained
    let inline regressionExceptionBeforeStartSingleStepRangeEnumerator zero one = 
        let sequence = seq { zero .. one .. one }
        let enumerator = sequence.GetEnumerator()
        enumerator.Current |> ignore

    let inline regressionExceptionBeforeStartVariableStepIntegralRange zero two = 
        let sequence = seq { zero .. two .. two }
        let enumerator = sequence.GetEnumerator()
        enumerator.Current |> ignore

    // strictly speaking, this is just undefined behaviour, but at some point the F# library decided that
    // it was an exception, so we are ensuring that such behaviour is retained
    let inline regressionExceptionAfterEndSingleStepRangeEnumerator zero one = 
        let sequence = seq { zero .. one .. one }
        let enumerator = sequence.GetEnumerator()
        while enumerator.MoveNext () do ignore ()
        enumerator.Current |> ignore

    let inline regressionExceptionAfterEndVariableStepIntegralRange zero two = 
        let sequence = seq { zero .. two .. two }
        let enumerator = sequence.GetEnumerator()
        while enumerator.MoveNext () do ignore ()
        enumerator.Current |> ignore

    let inline exceptions zero one two =
        Assert.Throws (typeof<System.ArgumentException>, (fun () -> [one .. zero .. two] |> List.length |> ignore)) |> ignore

        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionBeforeStartSingleStepRangeEnumerator zero one)) |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionBeforeStartVariableStepIntegralRange zero two)) |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionAfterEndSingleStepRangeEnumerator zero one))    |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionAfterEndVariableStepIntegralRange zero two))    |> ignore

    let inline common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three) =
        Assert.AreEqual ([min0 ..          min3], [min0; min1; min2; min3])
        Assert.AreEqual ([min0 .. one   .. min3], [min0; min1; min2; min3])
        Assert.AreEqual ([min0 .. two   .. min3], [min0; min2])
        Assert.AreEqual ([min0 .. three .. min3], [min0; min3])

        Assert.AreEqual ([max3 ..          max0], [max3; max2; max1; max0])
        Assert.AreEqual ([max3 .. one   .. max0], [max3; max2; max1; max0])
        Assert.AreEqual ([max3 .. two   .. max0], [max3; max1])
        Assert.AreEqual ([max3 .. three .. max0], [max3; max0])

        Assert.AreEqual ([max0 ..          min0], [])
        Assert.AreEqual ([max0 .. one   .. min0], [])
        Assert.AreEqual ([max0 .. two   .. min0], [])
        Assert.AreEqual ([max0 .. three .. min0], [])

        exceptions zero one two

        // tests for singleStepRangeEnumerator, as it only is used if start and/or end are not the
        // minimum or maximum of the number range and it is counting by 1s
        Assert.AreEqual ([min1 .. min3], [min1; min2; min3])
        Assert.AreEqual ([max3 .. max1], [max3; max2; max1])

    let inline signed min0 max0 =
        let zero  = LanguagePrimitives.GenericZero
        let one   = LanguagePrimitives.GenericOne
        let two   = one + one
        let three = two + one

        let min1 = min0 + one
        let min2 = min1 + one
        let min3 = min2 + one

        let max1 = max0 - one
        let max2 = max1 - one
        let max3 = max2 - one

        common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three)

        Assert.AreEqual ([min0 .. max0 .. max0], [ min0; min0 + max0; min0 + max0 + max0 ])
        Assert.AreEqual ([min0 .. max1 .. max0], [ min0; min0 + max1; min0 + max1 + max1 ])
        Assert.AreEqual ([min0 .. max2 .. max0], [ min0; min0 + max2; min0 + max2 + max2 ])
        Assert.AreEqual ([min0 .. max3 .. max0], [ min0; min0 + max3; min0 + max3 + max3 ])

        Assert.AreEqual ([min3 .. -one   .. min0], [min3; min2; min1; min0])
        Assert.AreEqual ([min3 .. -two   .. min0], [min3; min1])
        Assert.AreEqual ([min3 .. -three .. min0], [min3; min0])

        Assert.AreEqual ([max0 .. -one   .. max3], [max0; max1; max2; max3])
        Assert.AreEqual ([max0 .. -two   .. max3], [max0; max2])
        Assert.AreEqual ([max0 .. -three .. max3], [max0; max3])

        Assert.AreEqual ([min0 .. -one   .. max0], [])
        Assert.AreEqual ([min0 .. -two   .. max0], [])
        Assert.AreEqual ([min0 .. -three .. max0], [])

        Assert.AreEqual ([max0 .. min0 .. min0], [max0; max0 + min0])
        Assert.AreEqual ([max0 .. min1 .. min0], [max0; max0 + min1; max0 + min1 + min1 ])
        Assert.AreEqual ([max0 .. min2 .. min0], [max0; max0 + min2; max0 + min2 + min2 ])
        Assert.AreEqual ([max0 .. min3 .. min0], [max0; max0 + min3; max0 + min3 + min3 ])

    let inline unsigned min0 max0 =
        let zero  = LanguagePrimitives.GenericZero
        let one   = LanguagePrimitives.GenericOne
        let two   = one + one
        let three = two + one

        let min1 = min0 + one
        let min2 = min1 + one
        let min3 = min2 + one

        let max1 = max0 - one
        let max2 = max1 - one
        let max3 = max2 - one

        common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three)

        Assert.AreEqual ([min0 .. max0 .. max0], [min0; min0 + max0])
        Assert.AreEqual ([min0 .. max1 .. max0], [min0; min0 + max1])
        Assert.AreEqual ([min0 .. max2 .. max0], [min0; min0 + max2])
        Assert.AreEqual ([min0 .. max3 .. max0], [min0; min0 + max3])


type RangeTests() =
    [<Fact>] member _.``Range.SByte``  () = RangeTestsHelpers.signed   System.SByte.MinValue  System.SByte.MaxValue
    [<Fact>] member _.``Range.Byte``   () = RangeTestsHelpers.unsigned System.Byte.MinValue   System.Byte.MaxValue
    [<Fact>] member _.``Range.Int16``  () = RangeTestsHelpers.signed   System.Int16.MinValue  System.Int16.MaxValue
    [<Fact>] member _.``Range.UInt16`` () = RangeTestsHelpers.unsigned System.UInt16.MinValue System.UInt16.MaxValue
    [<Fact>] member _.``Range.Int32``  () = RangeTestsHelpers.signed   System.Int32.MinValue  System.Int32.MaxValue
    [<Fact>] member _.``Range.UInt32`` () = RangeTestsHelpers.unsigned System.UInt32.MinValue System.UInt32.MaxValue
    [<Fact>] member _.``Range.Int64``  () = RangeTestsHelpers.signed   System.Int64.MinValue  System.Int64.MaxValue
    [<Fact>] member _.``Range.UInt64`` () = RangeTestsHelpers.unsigned System.UInt64.MinValue System.UInt64.MaxValue

    [<Fact>]
    member _.``Range.IntPtr`` () =
        if System.IntPtr.Size >= 4 then RangeTestsHelpers.signed (System.IntPtr System.Int32.MinValue) (System.IntPtr System.Int32.MaxValue)
        if System.IntPtr.Size >= 8 then RangeTestsHelpers.signed (System.IntPtr System.Int64.MinValue) (System.IntPtr System.Int64.MaxValue)
        
    [<Fact>]
    member _.``Range.UIntPtr`` () =
        if System.UIntPtr.Size >= 4 then RangeTestsHelpers.unsigned (System.UIntPtr System.UInt32.MinValue) (System.UIntPtr System.UInt32.MaxValue)
        if System.UIntPtr.Size >= 8 then RangeTestsHelpers.unsigned (System.UIntPtr System.UInt64.MinValue) (System.UIntPtr System.UInt64.MaxValue)
        

open NonStructuralComparison


type NonStructuralComparisonTests() =

    [<Fact>]
    member _.CompareFloat32() = // https://github.com/dotnet/fsharp/pull/4493

        let x = 32 |> float32
        let y = 32 |> float32
        let comparison = compare x y
        Assert.AreEqual(0, comparison)
