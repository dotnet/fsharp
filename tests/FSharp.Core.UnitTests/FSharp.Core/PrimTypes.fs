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
    member _.GenericEqualityForNans() = 
        Assert.DoesNotContain(true,
            [| LanguagePrimitives.GenericEquality nan nan
               LanguagePrimitives.GenericEquality [nan] [nan]
               LanguagePrimitives.GenericEquality [|nan|] [|nan|]
               LanguagePrimitives.GenericEquality (Set.ofList [nan]) (Set.ofList [nan])       
               LanguagePrimitives.GenericEquality (Map.ofList [1,nan]) (Map.ofList [1,nan])
               LanguagePrimitives.GenericEquality (Map.ofList [nan,1]) (Map.ofList [nan,1])
               LanguagePrimitives.GenericEquality (Map.ofList [nan,nan]) (Map.ofList [nan,nan])
               
               LanguagePrimitives.GenericEquality nanf nanf
               LanguagePrimitives.GenericEquality [nanf] [nanf]
               LanguagePrimitives.GenericEquality [|nanf|] [|nanf|]
               LanguagePrimitives.GenericEquality (Set.ofList [nanf]) (Set.ofList [nanf])          
               LanguagePrimitives.GenericEquality (Map.ofList [1,nanf]) (Map.ofList [1,nanf])
               LanguagePrimitives.GenericEquality (Map.ofList [nanf,1]) (Map.ofList [nanf,1])
               LanguagePrimitives.GenericEquality (Map.ofList [nanf,nanf]) (Map.ofList [nanf,nanf])|])

    [<Fact>]
    member _.GenericEqualityER() = 
        Assert.DoesNotContain(false,
            [| LanguagePrimitives.GenericEqualityER nan nan
               LanguagePrimitives.GenericEqualityER [nan] [nan]
               LanguagePrimitives.GenericEqualityER [|nan|] [|nan|]
               LanguagePrimitives.GenericEqualityER (Set.ofList [nan]) (Set.ofList [nan])        
               LanguagePrimitives.GenericEqualityER (Map.ofList [1,nan]) (Map.ofList [1,nan])
               LanguagePrimitives.GenericEqualityER (Map.ofList [nan,1]) (Map.ofList [nan,1])
               LanguagePrimitives.GenericEqualityER (Map.ofList [nan,nan]) (Map.ofList [nan,nan])
               
               LanguagePrimitives.GenericEqualityER nanf nanf
               LanguagePrimitives.GenericEqualityER [nanf] [nanf]
               LanguagePrimitives.GenericEqualityER [|nanf|] [|nanf|]
               LanguagePrimitives.GenericEqualityER (Set.ofList [nanf]) (Set.ofList [nanf])        
               LanguagePrimitives.GenericEqualityER (Map.ofList [1,nanf]) (Map.ofList [1,nanf])
               LanguagePrimitives.GenericEqualityER (Map.ofList [nanf,1]) (Map.ofList [nanf,1])
               LanguagePrimitives.GenericEqualityER (Map.ofList [nanf,nanf]) (Map.ofList [nanf,nanf])|])
        
        
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
        // recordtype
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
        Assert.Throws (typeof<System.ArgumentException>, (fun () -> seq {one .. zero .. two} |> Seq.length |> ignore)) |> ignore
        Assert.Throws (typeof<System.ArgumentException>, (fun () -> [one .. zero .. two] |> List.length |> ignore)) |> ignore
        Assert.Throws (typeof<System.ArgumentException>, (fun () -> [|one .. zero .. two|] |> Array.length |> ignore)) |> ignore

        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionBeforeStartSingleStepRangeEnumerator zero one)) |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionBeforeStartVariableStepIntegralRange zero two)) |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionAfterEndSingleStepRangeEnumerator zero one))    |> ignore
        Assert.Throws (typeof<System.InvalidOperationException>, (fun () -> regressionExceptionAfterEndVariableStepIntegralRange zero two))    |> ignore

    let inline common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three) =
        Assert.AreEqual (seq {yield min0; yield min1; yield min2; yield min3}, seq {min0 ..          min3})
        Assert.AreEqual (seq {min0; min1; min2; min3}, seq {min0 .. one   .. min3})
        Assert.AreEqual (seq {min0; min2}, seq {min0 .. two   .. min3})
        Assert.AreEqual (seq {min0; min3}, seq {min0 .. three .. min3})

        Assert.AreEqual ([min0; min1; min2; min3], [min0 ..          min3])
        Assert.AreEqual ([min0; min1; min2; min3], [min0 .. one   .. min3])
        Assert.AreEqual ([min0; min2], [min0 .. two   .. min3])
        Assert.AreEqual ([min0; min3], [min0 .. three .. min3])

        Assert.AreEqual ([|min0; min1; min2; min3|], [|min0 ..          min3|])
        Assert.AreEqual ([|min0; min1; min2; min3|], [|min0 .. one   .. min3|])
        Assert.AreEqual ([|min0; min2|], [|min0 .. two   .. min3|])
        Assert.AreEqual ([|min0; min3|], [|min0 .. three .. min3|])

        Assert.AreEqual (seq {yield max3; yield max2; yield max1; yield max0}, seq {max3 ..          max0})
        Assert.AreEqual (seq {max3; max2; max1; max0}, seq {max3 .. one   .. max0})
        Assert.AreEqual (seq {max3; max1}, seq {max3 .. two   .. max0})
        Assert.AreEqual (seq {max3; max0}, seq {max3 .. three .. max0})

        Assert.AreEqual ([max3; max2; max1; max0], [max3 ..          max0])
        Assert.AreEqual ([max3; max2; max1; max0], [max3 .. one   .. max0])
        Assert.AreEqual ([max3; max1], [max3 .. two   .. max0])
        Assert.AreEqual ([max3; max0], [max3 .. three .. max0])

        Assert.AreEqual ([|max3; max2; max1; max0|], [|max3 ..          max0|])
        Assert.AreEqual ([|max3; max2; max1; max0|], [|max3 .. one   .. max0|])
        Assert.AreEqual ([|max3; max1|], [|max3 .. two   .. max0|])
        Assert.AreEqual ([|max3; max0|], [|max3 .. three .. max0|])

        Assert.AreEqual (Seq.empty, seq {max0 ..          min0})
        Assert.AreEqual (Seq.empty, seq {max0 .. one   .. min0})
        Assert.AreEqual (Seq.empty, seq {max0 .. two   .. min0})
        Assert.AreEqual (Seq.empty, seq {max0 .. three .. min0})

        Assert.AreEqual ([], [max0 ..          min0])
        Assert.AreEqual ([], [max0 .. one   .. min0])
        Assert.AreEqual ([], [max0 .. two   .. min0])
        Assert.AreEqual ([], [max0 .. three .. min0])

        Assert.AreEqual ([||], [|max0 ..          min0|])
        Assert.AreEqual ([||], [|max0 .. one   .. min0|])
        Assert.AreEqual ([||], [|max0 .. two   .. min0|])
        Assert.AreEqual ([||], [|max0 .. three .. min0|])

        exceptions zero one two

        // tests for singleStepRangeEnumerator, as it only is used if start and/or end are not the
        // minimum or maximum of the number range and it is counting by 1s
        Assert.AreEqual (seq {min1; min2; min3}, seq {min1 .. min3})
        Assert.AreEqual (seq {max3; max2; max1}, seq {max3 .. max1})

        Assert.AreEqual ([min1; min2; min3], [min1 .. min3])
        Assert.AreEqual ([max3; max2; max1], [max3 .. max1])

        Assert.AreEqual ([|min1; min2; min3|], [|min1 .. min3|])
        Assert.AreEqual ([|max3; max2; max1|], [|max3 .. max1|])

    let inline signed zero one min0 max0 =
        let two   = one + one
        let three = two + one

        let min1 = min0 + one
        let min2 = min1 + one
        let min3 = min2 + one

        let max1 = max0 - one
        let max2 = max1 - one
        let max3 = max2 - one

        common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three)

        Assert.AreEqual (seq { min0; min0 + max0; min0 + max0 + max0 }, seq {min0 .. max0 .. max0})
        Assert.AreEqual (seq { min0; min0 + max1; min0 + max1 + max1 }, seq {min0 .. max1 .. max0})
        Assert.AreEqual (seq { min0; min0 + max2; min0 + max2 + max2 }, seq {min0 .. max2 .. max0})
        Assert.AreEqual (seq { min0; min0 + max3; min0 + max3 + max3 }, seq {min0 .. max3 .. max0})

        Assert.AreEqual ([ min0; min0 + max0; min0 + max0 + max0 ], [min0 .. max0 .. max0])
        Assert.AreEqual ([ min0; min0 + max1; min0 + max1 + max1 ], [min0 .. max1 .. max0])
        Assert.AreEqual ([ min0; min0 + max2; min0 + max2 + max2 ], [min0 .. max2 .. max0])
        Assert.AreEqual ([ min0; min0 + max3; min0 + max3 + max3 ], [min0 .. max3 .. max0])

        Assert.AreEqual ([| min0; min0 + max0; min0 + max0 + max0 |], [|min0 .. max0 .. max0|])
        Assert.AreEqual ([| min0; min0 + max1; min0 + max1 + max1 |], [|min0 .. max1 .. max0|])
        Assert.AreEqual ([| min0; min0 + max2; min0 + max2 + max2 |], [|min0 .. max2 .. max0|])
        Assert.AreEqual ([| min0; min0 + max3; min0 + max3 + max3 |], [|min0 .. max3 .. max0|])

        Assert.AreEqual (seq {min3; min2; min1; min0}, seq {min3 .. -one   .. min0})
        Assert.AreEqual (seq {min3; min1}, seq {min3 .. -two   .. min0})
        Assert.AreEqual (seq {min3; min0}, seq {min3 .. -three .. min0})

        Assert.AreEqual ([min3; min2; min1; min0], [min3 .. -one   .. min0])
        Assert.AreEqual ([min3; min1], [min3 .. -two   .. min0])
        Assert.AreEqual ([min3; min0], [min3 .. -three .. min0])

        Assert.AreEqual ([|min3; min2; min1; min0|], [|min3 .. -one   .. min0|])
        Assert.AreEqual ([|min3; min1|], [|min3 .. -two   .. min0|])
        Assert.AreEqual ([|min3; min0|], [|min3 .. -three .. min0|])

        Assert.AreEqual (seq {max0; max1; max2; max3}, seq {max0 .. -one   .. max3})
        Assert.AreEqual (seq {max0; max2}, seq {max0 .. -two   .. max3})
        Assert.AreEqual (seq {max0; max3}, seq {max0 .. -three .. max3})

        Assert.AreEqual ([max0; max1; max2; max3], [max0 .. -one   .. max3])
        Assert.AreEqual ([max0; max2], [max0 .. -two   .. max3])
        Assert.AreEqual ([max0; max3], [max0 .. -three .. max3])

        Assert.AreEqual ([|max0; max1; max2; max3|], [|max0 .. -one   .. max3|])
        Assert.AreEqual ([|max0; max2|], [|max0 .. -two   .. max3|])
        Assert.AreEqual ([|max0; max3|], [|max0 .. -three .. max3|])

        Assert.AreEqual (Seq.empty, seq {min0 .. -one   .. max0})
        Assert.AreEqual (Seq.empty, seq {min0 .. -two   .. max0})
        Assert.AreEqual (Seq.empty, seq {min0 .. -three .. max0})

        Assert.AreEqual ([], [min0 .. -one   .. max0])
        Assert.AreEqual ([], [min0 .. -two   .. max0])
        Assert.AreEqual ([], [min0 .. -three .. max0])

        Assert.AreEqual ([||], [|min0 .. -one   .. max0|])
        Assert.AreEqual ([||], [|min0 .. -two   .. max0|])
        Assert.AreEqual ([||], [|min0 .. -three .. max0|])

        Assert.AreEqual (seq {max0; max0 + min0}, seq {max0 .. min0 .. min0})
        Assert.AreEqual (seq {max0; max0 + min1; max0 + min1 + min1 }, seq {max0 .. min1 .. min0})
        Assert.AreEqual (seq {max0; max0 + min2; max0 + min2 + min2 }, seq {max0 .. min2 .. min0})
        Assert.AreEqual (seq {max0; max0 + min3; max0 + min3 + min3 }, seq {max0 .. min3 .. min0})

        Assert.AreEqual ([max0; max0 + min0], [max0 .. min0 .. min0])
        Assert.AreEqual ([max0; max0 + min1; max0 + min1 + min1 ], [max0 .. min1 .. min0])
        Assert.AreEqual ([max0; max0 + min2; max0 + min2 + min2 ], [max0 .. min2 .. min0])
        Assert.AreEqual ([max0; max0 + min3; max0 + min3 + min3 ], [max0 .. min3 .. min0])

        Assert.AreEqual ([|max0; max0 + min0|], [|max0 .. min0 .. min0|])
        Assert.AreEqual ([|max0; max0 + min1; max0 + min1 + min1 |], [|max0 .. min1 .. min0|])
        Assert.AreEqual ([|max0; max0 + min2; max0 + min2 + min2 |], [|max0 .. min2 .. min0|])
        Assert.AreEqual ([|max0; max0 + min3; max0 + min3 + min3 |], [|max0 .. min3 .. min0|])

    let inline unsigned zero one min0 max0 =
        let two   = one + one
        let three = two + one

        let min1 = min0 + one
        let min2 = min1 + one
        let min3 = min2 + one

        let max1 = max0 - one
        let max2 = max1 - one
        let max3 = max2 - one

        common (min0, min1, min2, min3) (max0, max1, max2, max3) (zero, one, two, three)

        Assert.AreEqual (seq {yield min0; yield min0 + max0}, seq {min0 .. max0 .. max0})
        Assert.AreEqual (seq {min0; min0 + max1}, seq {min0 .. max1 .. max0})
        Assert.AreEqual (seq {min0; min0 + max2}, seq {min0 .. max2 .. max0})
        Assert.AreEqual (seq {min0; min0 + max3}, seq {min0 .. max3 .. max0})

        Assert.AreEqual ([min0; min0 + max0], [min0 .. max0 .. max0])
        Assert.AreEqual ([min0; min0 + max1], [min0 .. max1 .. max0])
        Assert.AreEqual ([min0; min0 + max2], [min0 .. max2 .. max0])
        Assert.AreEqual ([min0; min0 + max3], [min0 .. max3 .. max0])

        Assert.AreEqual ([|min0; min0 + max0|], [|min0 .. max0 .. max0|])
        Assert.AreEqual ([|min0; min0 + max1|], [|min0 .. max1 .. max0|])
        Assert.AreEqual ([|min0; min0 + max2|], [|min0 .. max2 .. max0|])
        Assert.AreEqual ([|min0; min0 + max3|], [|min0 .. max3 .. max0|])

// Note to future contributors: if the code gen for ranges is not correct,
// some of these tests may loop forever or use up all available memory instead of failing outright.
module RangeTests =
    /// [|Byte.MinValue..Byte.MaxValue|]
    let allBytesArray =
        [|
            0x00uy; 0x01uy; 0x02uy; 0x03uy; 0x04uy; 0x05uy; 0x06uy; 0x07uy; 0x08uy; 0x09uy; 0x0auy; 0x0buy; 0x0cuy; 0x0duy; 0x0euy; 0x0fuy
            0x10uy; 0x11uy; 0x12uy; 0x13uy; 0x14uy; 0x15uy; 0x16uy; 0x17uy; 0x18uy; 0x19uy; 0x1auy; 0x1buy; 0x1cuy; 0x1duy; 0x1euy; 0x1fuy
            0x20uy; 0x21uy; 0x22uy; 0x23uy; 0x24uy; 0x25uy; 0x26uy; 0x27uy; 0x28uy; 0x29uy; 0x2auy; 0x2buy; 0x2cuy; 0x2duy; 0x2euy; 0x2fuy
            0x30uy; 0x31uy; 0x32uy; 0x33uy; 0x34uy; 0x35uy; 0x36uy; 0x37uy; 0x38uy; 0x39uy; 0x3auy; 0x3buy; 0x3cuy; 0x3duy; 0x3euy; 0x3fuy
            0x40uy; 0x41uy; 0x42uy; 0x43uy; 0x44uy; 0x45uy; 0x46uy; 0x47uy; 0x48uy; 0x49uy; 0x4auy; 0x4buy; 0x4cuy; 0x4duy; 0x4euy; 0x4fuy
            0x50uy; 0x51uy; 0x52uy; 0x53uy; 0x54uy; 0x55uy; 0x56uy; 0x57uy; 0x58uy; 0x59uy; 0x5auy; 0x5buy; 0x5cuy; 0x5duy; 0x5euy; 0x5fuy
            0x60uy; 0x61uy; 0x62uy; 0x63uy; 0x64uy; 0x65uy; 0x66uy; 0x67uy; 0x68uy; 0x69uy; 0x6auy; 0x6buy; 0x6cuy; 0x6duy; 0x6euy; 0x6fuy
            0x70uy; 0x71uy; 0x72uy; 0x73uy; 0x74uy; 0x75uy; 0x76uy; 0x77uy; 0x78uy; 0x79uy; 0x7auy; 0x7buy; 0x7cuy; 0x7duy; 0x7euy; 0x7fuy
            0x80uy; 0x81uy; 0x82uy; 0x83uy; 0x84uy; 0x85uy; 0x86uy; 0x87uy; 0x88uy; 0x89uy; 0x8auy; 0x8buy; 0x8cuy; 0x8duy; 0x8euy; 0x8fuy
            0x90uy; 0x91uy; 0x92uy; 0x93uy; 0x94uy; 0x95uy; 0x96uy; 0x97uy; 0x98uy; 0x99uy; 0x9auy; 0x9buy; 0x9cuy; 0x9duy; 0x9euy; 0x9fuy
            0xa0uy; 0xa1uy; 0xa2uy; 0xa3uy; 0xa4uy; 0xa5uy; 0xa6uy; 0xa7uy; 0xa8uy; 0xa9uy; 0xaauy; 0xabuy; 0xacuy; 0xaduy; 0xaeuy; 0xafuy
            0xb0uy; 0xb1uy; 0xb2uy; 0xb3uy; 0xb4uy; 0xb5uy; 0xb6uy; 0xb7uy; 0xb8uy; 0xb9uy; 0xbauy; 0xbbuy; 0xbcuy; 0xbduy; 0xbeuy; 0xbfuy
            0xc0uy; 0xc1uy; 0xc2uy; 0xc3uy; 0xc4uy; 0xc5uy; 0xc6uy; 0xc7uy; 0xc8uy; 0xc9uy; 0xcauy; 0xcbuy; 0xccuy; 0xcduy; 0xceuy; 0xcfuy
            0xd0uy; 0xd1uy; 0xd2uy; 0xd3uy; 0xd4uy; 0xd5uy; 0xd6uy; 0xd7uy; 0xd8uy; 0xd9uy; 0xdauy; 0xdbuy; 0xdcuy; 0xdduy; 0xdeuy; 0xdfuy
            0xe0uy; 0xe1uy; 0xe2uy; 0xe3uy; 0xe4uy; 0xe5uy; 0xe6uy; 0xe7uy; 0xe8uy; 0xe9uy; 0xeauy; 0xebuy; 0xecuy; 0xeduy; 0xeeuy; 0xefuy
            0xf0uy; 0xf1uy; 0xf2uy; 0xf3uy; 0xf4uy; 0xf5uy; 0xf6uy; 0xf7uy; 0xf8uy; 0xf9uy; 0xfauy; 0xfbuy; 0xfcuy; 0xfduy; 0xfeuy; 0xffuy
        |]

    /// [Byte.MinValue..Byte.MaxValue]
    let allBytesList = List.ofArray allBytesArray

    /// {Byte.MinValue..Byte.MaxValue}
    let allBytesSeq = Seq.ofArray allBytesArray

    /// [|SByte.MinValue..SByte.MaxValue|]
    let allSBytesArray =
        [|
            0x80y; 0x81y; 0x82y; 0x83y; 0x84y; 0x85y; 0x86y; 0x87y; 0x88y; 0x89y; 0x8ay; 0x8by; 0x8cy; 0x8dy; 0x8ey; 0x8fy
            0x90y; 0x91y; 0x92y; 0x93y; 0x94y; 0x95y; 0x96y; 0x97y; 0x98y; 0x99y; 0x9ay; 0x9by; 0x9cy; 0x9dy; 0x9ey; 0x9fy
            0xa0y; 0xa1y; 0xa2y; 0xa3y; 0xa4y; 0xa5y; 0xa6y; 0xa7y; 0xa8y; 0xa9y; 0xaay; 0xaby; 0xacy; 0xady; 0xaey; 0xafy
            0xb0y; 0xb1y; 0xb2y; 0xb3y; 0xb4y; 0xb5y; 0xb6y; 0xb7y; 0xb8y; 0xb9y; 0xbay; 0xbby; 0xbcy; 0xbdy; 0xbey; 0xbfy
            0xc0y; 0xc1y; 0xc2y; 0xc3y; 0xc4y; 0xc5y; 0xc6y; 0xc7y; 0xc8y; 0xc9y; 0xcay; 0xcby; 0xccy; 0xcdy; 0xcey; 0xcfy
            0xd0y; 0xd1y; 0xd2y; 0xd3y; 0xd4y; 0xd5y; 0xd6y; 0xd7y; 0xd8y; 0xd9y; 0xday; 0xdby; 0xdcy; 0xddy; 0xdey; 0xdfy
            0xe0y; 0xe1y; 0xe2y; 0xe3y; 0xe4y; 0xe5y; 0xe6y; 0xe7y; 0xe8y; 0xe9y; 0xeay; 0xeby; 0xecy; 0xedy; 0xeey; 0xefy
            0xf0y; 0xf1y; 0xf2y; 0xf3y; 0xf4y; 0xf5y; 0xf6y; 0xf7y; 0xf8y; 0xf9y; 0xfay; 0xfby; 0xfcy; 0xfdy; 0xfey; 0xffy
            0x00y; 0x01y; 0x02y; 0x03y; 0x04y; 0x05y; 0x06y; 0x07y; 0x08y; 0x09y; 0x0ay; 0x0by; 0x0cy; 0x0dy; 0x0ey; 0x0fy
            0x10y; 0x11y; 0x12y; 0x13y; 0x14y; 0x15y; 0x16y; 0x17y; 0x18y; 0x19y; 0x1ay; 0x1by; 0x1cy; 0x1dy; 0x1ey; 0x1fy
            0x20y; 0x21y; 0x22y; 0x23y; 0x24y; 0x25y; 0x26y; 0x27y; 0x28y; 0x29y; 0x2ay; 0x2by; 0x2cy; 0x2dy; 0x2ey; 0x2fy
            0x30y; 0x31y; 0x32y; 0x33y; 0x34y; 0x35y; 0x36y; 0x37y; 0x38y; 0x39y; 0x3ay; 0x3by; 0x3cy; 0x3dy; 0x3ey; 0x3fy
            0x40y; 0x41y; 0x42y; 0x43y; 0x44y; 0x45y; 0x46y; 0x47y; 0x48y; 0x49y; 0x4ay; 0x4by; 0x4cy; 0x4dy; 0x4ey; 0x4fy
            0x50y; 0x51y; 0x52y; 0x53y; 0x54y; 0x55y; 0x56y; 0x57y; 0x58y; 0x59y; 0x5ay; 0x5by; 0x5cy; 0x5dy; 0x5ey; 0x5fy
            0x60y; 0x61y; 0x62y; 0x63y; 0x64y; 0x65y; 0x66y; 0x67y; 0x68y; 0x69y; 0x6ay; 0x6by; 0x6cy; 0x6dy; 0x6ey; 0x6fy
            0x70y; 0x71y; 0x72y; 0x73y; 0x74y; 0x75y; 0x76y; 0x77y; 0x78y; 0x79y; 0x7ay; 0x7by; 0x7cy; 0x7dy; 0x7ey; 0x7fy
        |]

    /// [SByte.MinValue..SByte.MaxValue]
    let allSBytesList = List.ofArray allSBytesArray

    /// {SByte.MinValue..SByte.MaxValue}
    let allSBytesSeq = Seq.ofArray allSBytesArray

    /// These tests' constant arguments are inlined,
    /// and the size of the collection (for lists and arrays) or count (for for-loops) is computed at build-time.
    module BuildTime =
        [<Fact>]
        let ``Range.SByte`` () =
            Assert.AreEqual(256, let mutable c = 0 in for _ in System.SByte.MinValue..System.SByte.MaxValue do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in System.SByte.MinValue..1y..System.SByte.MaxValue do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in System.SByte.MaxValue .. -1y .. System.SByte.MinValue do c <- c + 1 done; c)

            Assert.AreEqual(allSBytesSeq, seq {System.SByte.MinValue..System.SByte.MaxValue})
            Assert.AreEqual(allSBytesList, [System.SByte.MinValue..System.SByte.MaxValue])
            Assert.AreEqual(allSBytesArray, [|System.SByte.MinValue..System.SByte.MaxValue|])

            Assert.AreEqual(allSBytesSeq, seq {System.SByte.MinValue..1y..System.SByte.MaxValue})
            Assert.AreEqual(allSBytesList, [System.SByte.MinValue..1y..System.SByte.MaxValue])
            Assert.AreEqual(allSBytesArray, [|System.SByte.MinValue..1y..System.SByte.MaxValue|])

            Assert.AreEqual(Seq.rev allSBytesSeq, seq {System.SByte.MaxValue .. -1y .. System.SByte.MinValue})
            Assert.AreEqual(List.rev allSBytesList, [System.SByte.MaxValue .. -1y .. System.SByte.MinValue])
            Assert.AreEqual(Array.rev allSBytesArray, [|System.SByte.MaxValue .. -1y .. System.SByte.MinValue|])

            RangeTestsHelpers.signed 0y 1y System.SByte.MinValue System.SByte.MaxValue

        [<Fact>]
        let ``Range.Byte`` () =
            Assert.AreEqual(256, let mutable c = 0 in for _ in System.Byte.MinValue..System.Byte.MaxValue do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in System.Byte.MinValue..1uy..System.Byte.MaxValue do c <- c + 1 done; c)

            Assert.AreEqual(allBytesSeq, seq {System.Byte.MinValue..System.Byte.MaxValue})
            Assert.AreEqual(allBytesList, [System.Byte.MinValue..System.Byte.MaxValue])
            Assert.AreEqual(allBytesArray, [|System.Byte.MinValue..System.Byte.MaxValue|])

            Assert.AreEqual(allBytesSeq, seq {System.Byte.MinValue..1uy..System.Byte.MaxValue})
            Assert.AreEqual(allBytesList, [System.Byte.MinValue..1uy..System.Byte.MaxValue])
            Assert.AreEqual(allBytesArray, [|System.Byte.MinValue..1uy..System.Byte.MaxValue|])

            RangeTestsHelpers.unsigned 0uy 1uy System.Byte.MinValue System.Byte.MaxValue

        //// Note: the IEnumerable<char> range iterator doesn't currently pass these tests. Should it?
        //[<Fact>]
        //let ``Range.Char`` () = RangeTestsHelpers.unsigned '\000' '\001' System.Char.MinValue System.Char.MaxValue

        [<Fact>]
        let ``Range.Int16`` () = RangeTestsHelpers.signed 0s 1s System.Int16.MinValue System.Int16.MaxValue

        [<Fact>]
        let ``Range.UInt16`` () = RangeTestsHelpers.unsigned 0us 1us System.UInt16.MinValue System.UInt16.MaxValue

        [<Fact>]
        let ``Range.Int32`` () = RangeTestsHelpers.signed 0 1 System.Int32.MinValue System.Int32.MaxValue

        [<Fact>]
        let ``Range.UInt32`` () = RangeTestsHelpers.unsigned 0u 1u System.UInt32.MinValue System.UInt32.MaxValue

        [<Fact>]
        let ``Range.Int64`` () = RangeTestsHelpers.signed 0L 1L System.Int64.MinValue System.Int64.MaxValue

        [<Fact>]
        let ``Range.UInt64`` () = RangeTestsHelpers.unsigned 0UL 1UL System.UInt64.MinValue System.UInt64.MaxValue

        [<Fact>]
        let ``Range.IntPtr`` () =
            // 0x80000000n is negative on x86, but would be positive on x64.
            if System.IntPtr.Size = 4 then
                RangeTestsHelpers.signed 0x0n 0x1n 0x80000000n 0x7fffffffn

            if System.IntPtr.Size = 8 then
                RangeTestsHelpers.signed 0x0n 0x1n 0x8000000000000000n 0x7fffffffffffffffn
         
        [<Fact>]
        let ``Range.UIntPtr`` () =
            if System.UIntPtr.Size >= 4 then
                RangeTestsHelpers.unsigned 0x0un 0x1un 0x0un 0xffffffffun

            if System.UIntPtr.Size >= 8 then
                RangeTestsHelpers.unsigned 0x0un 0x1un 0x0un 0xffffffffffffffffun

    /// These tests' arguments are intentionally _not_ inlined
    /// to force the size of the collection (for lists and arrays) or count (for for-loops) to be computed at runtime.
    module Runtime =
        [<Theory; InlineData(0y, 1y, System.SByte.MinValue, System.SByte.MaxValue)>]
        let ``Range.SByte`` (zero: sbyte) (one: sbyte) (min0: sbyte) (max0: sbyte) =
            Assert.AreEqual(256, let mutable c = 0 in for _ in min0..max0 do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in min0..one..max0 do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in max0 .. -one .. min0 do c <- c + 1 done; c)

            Assert.AreEqual(allSBytesSeq, seq {min0..max0})
            Assert.AreEqual(allSBytesList, [min0..max0])
            Assert.AreEqual(allSBytesArray, [|min0..max0|])

            Assert.AreEqual(allSBytesSeq, seq {min0..one..max0})
            Assert.AreEqual(allSBytesList, [min0..one..max0])
            Assert.AreEqual(allSBytesArray, [|min0..one..max0|])

            Assert.AreEqual(Seq.rev allSBytesSeq, seq {max0 .. -one .. min0})
            Assert.AreEqual(List.rev allSBytesList, [max0 .. -one .. min0])
            Assert.AreEqual(Array.rev allSBytesArray, [|max0 .. -one .. min0|])

            RangeTestsHelpers.signed zero one min0 max0

        [<Theory; InlineData(0uy, 1uy, System.Byte.MinValue, System.Byte.MaxValue)>]
        let ``Range.Byte`` (zero: byte) (one: byte) (min0: byte) (max0: byte) =
            Assert.AreEqual(256, let mutable c = 0 in for _ in min0..max0 do c <- c + 1 done; c)
            Assert.AreEqual(256, let mutable c = 0 in for _ in min0..one..max0 do c <- c + 1 done; c)

            Assert.AreEqual(allBytesSeq, seq {min0..max0})
            Assert.AreEqual(allBytesList, [min0..max0])
            Assert.AreEqual(allBytesArray, [|min0..max0|])

            Assert.AreEqual(allBytesSeq, seq {min0..one..max0})
            Assert.AreEqual(allBytesList, [min0..one..max0])
            Assert.AreEqual(allBytesArray, [|min0..one..max0|])

            RangeTestsHelpers.unsigned zero one min0 max0

        //// Note: the IEnumerable<char> range iterator doesn't currently pass these tests. Should it?
        //[<Theory; InlineData('\000', '\001', System.Char.MinValue, System.Char.MaxValue)>]
        //let ``Range.Char`` (zero: char) (one: char) (min0: char) (max0: char) = RangeTestsHelpers.unsigned zero one min0 max0

        [<Theory; InlineData(0s, 1s, System.Int16.MinValue, System.Int16.MaxValue)>]
        let ``Range.Int16`` (zero: int16) (one: int16) (min0: int16) (max0: int16) = RangeTestsHelpers.signed zero one min0 max0

        [<Theory; InlineData(0us, 1us, System.UInt16.MinValue, System.UInt16.MaxValue)>]
        let ``Range.UInt16`` (zero: uint16) (one: uint16) (min0: uint16) (max0: uint16) = RangeTestsHelpers.unsigned zero one min0 max0

        [<Theory; InlineData(0, 1, System.Int32.MinValue, System.Int32.MaxValue)>]
        let ``Range.Int32`` (zero: int) (one: int) (min0: int) (max0: int) = RangeTestsHelpers.signed zero one min0 max0

        [<Theory; InlineData(0u, 1u, System.UInt32.MinValue, System.UInt32.MaxValue)>]
        let ``Range.UInt32`` (zero: uint) (one: uint) (min0: uint) (max0: uint) = RangeTestsHelpers.unsigned zero one min0 max0

        [<Theory; InlineData(0L, 1L, System.Int64.MinValue, System.Int64.MaxValue)>]
        let ``Range.Int64`` (zero: int64) (one: int64) (min0: int64) (max0: int64) =
            RangeTestsHelpers.signed zero one min0 max0

        [<Theory; InlineData(0UL, 1UL, System.UInt64.MinValue, System.UInt64.MaxValue)>]
        let ``Range.UInt64`` (zero: uint64) (one: uint64) (min0: uint64) (max0: uint64) =
            RangeTestsHelpers.unsigned zero one min0 max0

        [<Fact>]
        let ``Range.IntPtr`` () =
            // The arguments here aren't being passed in as constants, so it doesn't matter if they're inlined.
            if System.IntPtr.Size = 4 then
                let zero, one, min0, max0 = System.IntPtr 0, System.IntPtr 1, System.IntPtr System.Int32.MinValue, System.IntPtr System.Int32.MaxValue
                RangeTestsHelpers.signed zero one min0 max0

            if System.IntPtr.Size = 8 then
                let zero, one, min0, max0 = System.IntPtr 0, System.IntPtr 1, System.IntPtr System.Int64.MinValue, System.IntPtr System.Int64.MaxValue
                RangeTestsHelpers.signed zero one min0 max0

        [<Fact>]
        let ``Range.UIntPtr`` () =
            // The arguments here aren't being passed in as constants, so it doesn't matter if they're inlined.
            if System.UIntPtr.Size >= 4 then
                let zero, one, min0, max0 = System.UIntPtr 0u, System.UIntPtr 1u, System.UIntPtr System.UInt32.MinValue, System.UIntPtr System.UInt32.MaxValue
                RangeTestsHelpers.unsigned zero one min0 max0

            if System.UIntPtr.Size >= 8 then
                let zero, one, min0, max0 =  System.UIntPtr 0u, System.UIntPtr 1u, System.UIntPtr System.UInt64.MinValue, System.UIntPtr System.UInt64.MaxValue
                RangeTestsHelpers.unsigned zero one min0 max0

open NonStructuralComparison


type NonStructuralComparisonTests() =

    [<Fact>]
    member _.CompareFloat32() = // https://github.com/dotnet/fsharp/pull/4493

        let x = 32 |> float32
        let y = 32 |> float32
        let comparison = compare x y
        Assert.AreEqual(0, comparison)
