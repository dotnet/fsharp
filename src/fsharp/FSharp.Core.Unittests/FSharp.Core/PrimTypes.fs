// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Core.LanguagePrimitives module

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open System.Numerics 
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

[<Measure>]
type m

[<TestFixture>]
type LanguagePrimitivesModule() =
    [<Test>]
    member this.CastingUnits() =
        let f = 2.5
        let ff= 2.5f
        let d = 2.0m
        let i = 2
        let l = 2L
        let s = 2s
        let y = 2y
        Assert.AreEqual(f, f |> LanguagePrimitives.FloatWithMeasure<m> |> float)
        Assert.AreEqual(ff, ff |> LanguagePrimitives.Float32WithMeasure<m> |> float32)
        Assert.AreEqual(d, d |> LanguagePrimitives.DecimalWithMeasure<m> |> decimal)
        Assert.AreEqual(i, i |> LanguagePrimitives.Int32WithMeasure<m> |> int)
        Assert.AreEqual(l, l |> LanguagePrimitives.Int64WithMeasure<m> |> int64)
        Assert.AreEqual(s, s |> LanguagePrimitives.Int16WithMeasure<m> |> int16)
        Assert.AreEqual(y, y |> LanguagePrimitives.SByteWithMeasure<m> |> sbyte)

    [<Test>]
    member this.MaxMinNan() =
        Assert.IsTrue(Double.IsNaN(max nan 1.0))
        Assert.IsTrue(Double.IsNaN(max 1.0 nan))
        Assert.IsTrue(Double.IsNaN(max nan nan))

        Assert.IsTrue(Single.IsNaN(max Single.NaN 1.0f))
        Assert.IsTrue(Single.IsNaN(max 1.0f Single.NaN))
        Assert.IsTrue(Single.IsNaN(max Single.NaN Single.NaN))
        
        Assert.IsTrue(Double.IsNaN(min nan 1.0))
        Assert.IsTrue(Double.IsNaN(min 1.0 nan))
        Assert.IsTrue(Double.IsNaN(min nan nan))

        Assert.IsTrue(Single.IsNaN(min Single.NaN 1.0f))
        Assert.IsTrue(Single.IsNaN(min 1.0f Single.NaN))
        Assert.IsTrue(Single.IsNaN(min Single.NaN Single.NaN))

    [<Test>]
    member this.DivideByInt() =
            
        // float32 
        let resultFloat32 = LanguagePrimitives.DivideByInt 3.0f 3
        Assert.AreEqual(resultFloat32, 1.0f)
        
        // double 
        let resultDouble = LanguagePrimitives.DivideByInt 3.9 3
        Assert.AreEqual(resultDouble,1.3)
        
        // decimal 
        let resultDecimal = LanguagePrimitives.DivideByInt 3.9M 3
        Assert.AreEqual(resultDecimal,1.3M)   
        
        
        ()

    [<Test>]
    member this.EnumOfValue() =  
        let monday = System.DayOfWeek.Monday
        let result = LanguagePrimitives.EnumOfValue<int,System.DayOfWeek>(1)
        
        Assert.AreEqual(result,monday)
        
        ()
    
    [<Test>]
    member this.EnumToValue() =

        let monday = System.DayOfWeek.Monday
        let result = LanguagePrimitives.EnumToValue monday
        Assert.AreEqual(result,1)
        
        ()   
        
    [<Test>]
    member this.GuidToString() =
        let s = "F99D95E0-2A5E-47c4-9B92-6661D65AE6B3"
        let guid = new Guid(s)
#if FX_NO_TO_LOWER_INVARIANT
        Assert.AreEqual(s.ToLower(), (string guid).ToLower())
#else        
        Assert.AreEqual(s.ToLowerInvariant(), (string guid).ToLowerInvariant())
#endif
    [<Test>]
    member this.GenericComparison() =

        // value type
        let resultValue = LanguagePrimitives.GenericComparison 1 1
        Assert.AreEqual(resultValue,0)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericComparison "ABC" "ABCDE"
        Assert.AreEqual(resultRef,-68)
        
        // null reference
        let resultRef = LanguagePrimitives.GenericComparison "ABC" null
        Assert.AreEqual(resultRef,1)
        
        ()   


#if FX_ATLEAST_PORTABLE
// TODO named #define ?
#else  
    [<Test>]
    member this.GenericComparisonBiModal() =

        // value type
        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 100 1
        Assert.AreEqual(resultValue,1)

        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 1 1
        Assert.AreEqual(resultValue,0)

        let resultValue = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default 1 200
        Assert.AreEqual(resultValue,-1)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABCDE" "ABC"
        Assert.AreEqual(resultRef,68)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABC" "ABC"
        Assert.AreEqual(resultRef,0)
        
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "abc" "abcde"
        Assert.AreEqual(resultRef,-100)
        
        // null reference
        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default "ABC" null
        Assert.AreEqual(resultRef,1)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default null null
        Assert.AreEqual(resultRef,0)

        let resultRef = LanguagePrimitives.GenericComparisonWithComparer System.Collections.Comparer.Default null "abc"
        Assert.AreEqual(resultRef,-1)
        
        ()   
#endif
        
    [<Test>]
    member this.GenericEquality() =

        // value type
        let resultValue = LanguagePrimitives.GenericEquality 1 1
        Assert.IsTrue(resultValue)

        let resultValue = LanguagePrimitives.GenericEquality 1 2
        Assert.IsFalse(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericEquality "ABC" "ABC"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericEquality "ABC" "ABCDE"
        Assert.IsFalse(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericEquality null null
        Assert.IsTrue(resultNul)

        let resultNul = LanguagePrimitives.GenericEquality "ABC" null
        Assert.IsFalse(resultNul)
        
        ()
        
    [<Test>]
    member this.GenericGreaterOrEqual() =

        // value type
        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 1 1
        Assert.IsTrue(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 2 1
        Assert.IsTrue(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterOrEqual 1 2
        Assert.IsFalse(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "abcde" "abc"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "ABCDE" "ABCDE"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterOrEqual "ABC" "ABCDE"
        Assert.IsFalse(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericGreaterOrEqual null null
        Assert.IsTrue(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterOrEqual null "ABCDE"
        Assert.IsFalse(resultNul)
        
        ()
        
        
    [<Test>]
    member this.GenericGreaterThan() =

        // value type
        let resultValue = LanguagePrimitives.GenericGreaterThan 1 1
        Assert.IsFalse(resultValue)

        let resultValue = LanguagePrimitives.GenericGreaterThan 2 1
        Assert.IsTrue(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericGreaterThan "ABC" "ABCDE"
        Assert.IsFalse(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterThan "ABCDE" "ABCDE"
        Assert.IsFalse(resultRef)

        let resultRef = LanguagePrimitives.GenericGreaterThan "abc" "a"
        Assert.IsTrue(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericGreaterThan null null
        Assert.IsFalse(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterThan null "ABC"
        Assert.IsFalse(resultNul)

        let resultNul = LanguagePrimitives.GenericGreaterThan "ABC" null
        Assert.IsTrue(resultNul)
        
        ()
        
        
    [<Test>]
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
        
        ()   
        
        
    [<Test>]
    member this.GenericLessOrEqual() =

        // value type
        let resultValue = LanguagePrimitives.GenericLessOrEqual 1 1
        Assert.IsTrue(resultValue)

        let resultValue = LanguagePrimitives.GenericLessOrEqual 1 2
        Assert.IsTrue(resultValue)

        let resultValue = LanguagePrimitives.GenericLessOrEqual -1 0
        Assert.IsTrue(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericLessOrEqual "ABC" "ABCDE"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericLessOrEqual "ABCDE" "ABCDE"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericLessOrEqual "abcde" "abc"
        Assert.IsFalse(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericLessOrEqual null null
        Assert.IsTrue(resultNul)

        let resultNul = LanguagePrimitives.GenericLessOrEqual null "abc"
        Assert.IsTrue(resultNul)

        let resultNul = LanguagePrimitives.GenericLessOrEqual "abc" null
        Assert.IsFalse(resultNul)
        
        ()
        
        
    [<Test>]
    member this.GenericLessThan() =

        // value type
        let resultValue = LanguagePrimitives.GenericLessThan 1 1
        Assert.IsFalse(resultValue)

        let resultValue = LanguagePrimitives.GenericLessThan -2 -4
        Assert.IsFalse(resultValue)

        let resultValue = LanguagePrimitives.GenericLessThan 1 2
        Assert.IsTrue(resultValue)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericLessThan "ABC" "ABCDE"
        Assert.IsTrue(resultRef)

        let resultRef = LanguagePrimitives.GenericLessThan "ABC" "ABC"
        Assert.IsFalse(resultRef)

        let resultRef = LanguagePrimitives.GenericLessThan "abc" "a"
        Assert.IsFalse(resultRef)
        
        // null reference
        let resultNul = LanguagePrimitives.GenericLessThan null "abc"
        Assert.IsTrue(resultNul)

        let resultNul = LanguagePrimitives.GenericLessThan "aa" null
        Assert.IsFalse(resultNul)

        let resultNul = LanguagePrimitives.GenericLessThan null null
        Assert.IsFalse(resultNul)
        
        ()

    [<Test>]
    member this.GenericMaximum() =

        // value type
        let resultValue = LanguagePrimitives.GenericMaximum 8 9
        Assert.AreEqual(resultValue,9)

        let resultValue = LanguagePrimitives.GenericMaximum -800 -900
        Assert.AreEqual(resultValue,-800)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericMaximum "ABC" "ABCDE"
        Assert.AreEqual(resultRef,"ABCDE") 
        
        let resultRef = LanguagePrimitives.GenericMaximum "ABCDE" "ABC" 
        Assert.AreEqual(resultRef,"ABCDE")
        
        
        // null reference
        let resultNul = LanguagePrimitives.GenericMaximum null null
        Assert.AreEqual(resultNul,null)

        let resultNul = LanguagePrimitives.GenericMaximum null "ABCDE"
        Assert.AreEqual(resultNul,"ABCDE")

        let resultNul = LanguagePrimitives.GenericMaximum "ABCDE" null
        Assert.AreEqual(resultNul,"ABCDE")
        
        ()
        
    [<Test>]
    member this.GenericMinimum() =

        // value type
        let resultValue = LanguagePrimitives.GenericMinimum 8 9
        Assert.AreEqual(resultValue,8)

        let resultValue = LanguagePrimitives.GenericMinimum -800 -900
        Assert.AreEqual(resultValue,-900)
        
        // reference type
        let resultRef = LanguagePrimitives.GenericMinimum "ABC" "ABCDE"
        Assert.AreEqual(resultRef,"ABC")

        let resultRef = LanguagePrimitives.GenericMinimum "abcde" "abc"
        Assert.AreEqual(resultRef,"abc")
        
        // null reference
        let resultNul = LanguagePrimitives.GenericMinimum null null
        Assert.AreEqual(resultNul,null)

        let resultNul = LanguagePrimitives.GenericMinimum null "ABC"
        Assert.AreEqual(resultNul,null)

        let resultNul = LanguagePrimitives.GenericMinimum "ABC" null
        Assert.AreEqual(resultNul,null)
        
        ()
        
    [<Test>]
    member this.GenericOne() =
        // int type
        let resultValue = LanguagePrimitives.GenericOne<int> 
        Assert.AreEqual(resultValue,1)

        // float type
        let resultValue = LanguagePrimitives.GenericOne<float> 
        Assert.AreEqual(resultValue,1.)

        // bigint type
        let resultValue = LanguagePrimitives.GenericOne<bigint> 
        Assert.AreEqual(resultValue,1I)
        
        ()
        
    [<Test>]
    member this.GenericZero() =
        // int type
        let resultValue = LanguagePrimitives.GenericZero<int> 
        Assert.AreEqual(resultValue,0)

        // float type
        let resultValue = LanguagePrimitives.GenericZero<float> 
        Assert.AreEqual(resultValue,0.)

        // bigint type
        let resultValue = LanguagePrimitives.GenericZero<bigint> 
        Assert.AreEqual(resultValue,0I)
        
        ()
        
    [<Test>]
    member this.ParseInt32() =
        
        let resultValue = LanguagePrimitives.ParseInt32 "100" 
        Assert.AreEqual(resultValue.GetType(),typeof<int>)
        Assert.AreEqual(resultValue,100)    

        let resultValue = LanguagePrimitives.ParseInt32 "-123" 
        Assert.AreEqual(resultValue,-123)    

        let resultValue = LanguagePrimitives.ParseInt32 "0" 
        Assert.AreEqual(resultValue,0)    

        
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseInt32 "-100000000000000000" |> ignore)

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseInt32 "" |> ignore)    
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseInt32 null  |> ignore)
        
        ()
        
    [<Test>]
    member this.ParseInt64() =
        
        let resultValue = LanguagePrimitives.ParseInt64 "100" 
        Assert.AreEqual(resultValue.GetType(),typeof<int64>)    
        Assert.AreEqual(resultValue,100L)   

        let resultValue = LanguagePrimitives.ParseInt64 "-100000000000000000" 
        Assert.AreEqual(resultValue,-100000000000000000L)   

        let resultValue = LanguagePrimitives.ParseInt64 "0" 
        Assert.AreEqual(resultValue,0)    

        CheckThrowsFormatException(fun () -> LanguagePrimitives.ParseInt64 "" |> ignore)    
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseInt64 null  |> ignore)
        
        ()
        
    [<Test>]
    member this.ParseUInt32() =
        
        let resultValue = LanguagePrimitives.ParseUInt32 "100" 
        Assert.AreEqual(resultValue.GetType(),typeof<uint32>)   
        Assert.AreEqual(resultValue,100ul)        
        
        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt32 "-1" |> ignore)
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseUInt32 null  |> ignore)
        
        ()
        
    [<Test>]
    member this.ParseUInt64() =
        
        let resultValue = LanguagePrimitives.ParseUInt64 "100" 
        Assert.AreEqual(resultValue.GetType(),typeof<uint64>) 
        Assert.AreEqual(resultValue,100UL)        

        CheckThrowsOverflowException(fun () -> LanguagePrimitives.ParseUInt64 "-1" |> ignore)
        
        CheckThrowsArgumentNullException(fun () -> LanguagePrimitives.ParseUInt64 null  |> ignore)
        
        ()

    [<Test>]
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
        // SL and Portable Runtimes are compiled with FX_NO_CHAR_PARSE
#if FX_NO_CHAR_PARSE
#else        
        CheckThrowsArgumentNullException2 "char" (fun () -> char s |> ignore)
#endif        
    [<Test>]
    member this.PhysicalEquality() =

        // revordtype
        let ref1 = ref 8
        let ref2 = ref 8
        let resultValue = LanguagePrimitives.PhysicalEquality ref1 ref2
        Assert.IsFalse(resultValue)
        Assert.IsTrue(LanguagePrimitives.PhysicalEquality ref1 ref1)
        Assert.IsTrue(LanguagePrimitives.PhysicalEquality ref2 ref2)
        
        // reference type
        let resultRef0 = LanguagePrimitives.PhysicalEquality "ABC" "ABC"
        Assert.IsTrue(resultRef0)
        
        let resultRef1 = LanguagePrimitives.PhysicalEquality "ABC" "DEF"
        Assert.IsFalse(resultRef1)
        
        // object type
        let resultRef2 = LanguagePrimitives.PhysicalEquality (obj()) (obj())
        Assert.IsFalse(resultRef2)
        
        // object type
        let o = obj()
        let resultRef3 = LanguagePrimitives.PhysicalEquality o o 
        Assert.IsTrue(resultRef3)
        
        // System.ValueType type
        let resultRef4 = LanguagePrimitives.PhysicalEquality (1 :> System.ValueType) (1 :> System.ValueType)
        Assert.IsFalse(resultRef4)
        
        // System.ValueType type
        let resultRef5 = LanguagePrimitives.PhysicalEquality (1 :> System.ValueType) (2 :> System.ValueType)
        Assert.IsFalse(resultRef5)
        
        // null reference
        let resultNul = LanguagePrimitives.PhysicalEquality null null
        Assert.IsTrue(resultNul)

[<TestFixture>]
type HashCompareModule() = // this module is internal/obsolete, but contains code reachable from many public APIs
    member inline this.ComparisonsFor< ^T when ^T : comparison>(x : ^T, y : ^T) =
        Assert.IsTrue( x < y )
        Assert.IsTrue( y > x ) 
        Assert.IsTrue( (x = x) )
        Assert.IsFalse( y < x )
        Assert.IsFalse( x > y )
        Assert.IsFalse( (x = y) )
        ()     
    [<Test>]
    member this.ComparisonsForArraysOfNativeInts() =
        this.ComparisonsFor( [|0n|], [|1n|] )
        this.ComparisonsFor( [|0un|], [|1un|] )
        ()     
    [<Test>]
    member this.ComparisonsForArraysOfFloatingPoints() =
        this.ComparisonsFor( [|0.0|], [|1.0|] )
        this.ComparisonsFor( [|0.0f|], [|1.0f|] )
        Assert.IsFalse( [| System.Double.NaN |] = [| System.Double.NaN |] )
        Assert.IsFalse( [| System.Single.NaN |] = [| System.Single.NaN |] )
        Assert.IsFalse( [| System.Double.NaN |] < [| System.Double.NaN |] )
        Assert.IsFalse( [| System.Single.NaN |] < [| System.Single.NaN |] )
        ()     
    [<Test>]
    member this.ComparisonsForOtherArrays() =
        this.ComparisonsFor( [|0uy|], [|1uy|] )
        this.ComparisonsFor( [|'a'|], [|'b'|] )
        this.ComparisonsFor( [|0UL|], [|1UL|] )
    [<Test>]
    member this.ComparisonsForStrings() =
        this.ComparisonsFor( "bar", "foo" )
        this.ComparisonsFor( [| "bar" |], [| "foo" |] )
    [<Test>]
    member this.ComparisonsForMultidimensionalIntArrays() =
        let N = 10
        let M = 100
        let Z = 9999
        let x = Array2D.init 3 3 (fun x y -> N*x + y)
        let y = Array2D.init 3 3 (fun x y -> N*x + y)
        Assert.IsTrue( hash x = hash y )
        y.[2,2] <- Z
        this.ComparisonsFor( x, y )
        let x = Array3D.init 3 3 3 (fun x y z -> M*x + N*y + z)
        let y = Array3D.init 3 3 3 (fun x y z -> M*x + N*y + z)
        Assert.IsTrue( hash x = hash y )
        y.[2,2,2] <- Z
        this.ComparisonsFor( x, y )
    [<Test>]
    member this.ComparisonsForMultidimensionalInt64Arrays() =
        let N = 10L
        let M = 100L
        let Z = 9999L
        let x = Array2D.init 3 3 (fun x y -> N*(int64 x) + (int64 y))
        let y = Array2D.init 3 3 (fun x y -> N*(int64 x) + (int64 y))
        Assert.IsTrue( hash x = hash y )
        y.[2,2] <- Z
        this.ComparisonsFor( x, y )
        let x = Array3D.init 3 3 3 (fun x y z -> M*(int64 x) + N*(int64 y) + (int64 z))
        let y = Array3D.init 3 3 3 (fun x y z -> M*(int64 x) + N*(int64 y) + (int64 z))
        Assert.IsTrue( hash x = hash y )
        y.[2,2,2] <- Z
        this.ComparisonsFor( x, y )
    
    [<Test>]
    member this.MonsterTuple() =
        let mt = 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        let mt2 = 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        Assert.AreEqual(mt,mt2)
    
[<TestFixture>]
type UnitType() =
    // interface
    [<Test>]
    member this.IComparable() =        
        let u:Unit = ()
        // value type
        let ic = u :> IComparable   
        CheckThrowsNullRefException(fun() ->ic.CompareTo(3) |>ignore) 
                
        ()
        
    // Base class methods
    [<Test>]
    member this.ObjectGetHashCode() =
        let u:Unit = ()
        CheckThrowsNullRefException(fun() ->u.GetHashCode() |>ignore) 
        
        ()
        
    [<Test>]
    member this.ObjectEquals() =
        let u:Unit = ()
        CheckThrowsNullRefException(fun() ->u.Equals(null) |>ignore) 
        

#if FX_ATLEAST_PORTABLE
// TODO named #define ?
#else     
[<TestFixture>]
type SourceConstructFlagsEnum() =
    [<Test>]
    member this.Getvalue() =
        Assert.AreEqual(SourceConstructFlags.GetNames(typeof<SourceConstructFlags>),
                        [|"None";"SumType";"RecordType";"ObjectType";"Field";
                          "Exception";"Closure";"Module";"UnionCase";"Value";
                          "KindMask";"NonPublicRepresentation"|])
              
        ()
        
[<TestFixture>]
type CompilationRepresentationFlagsEnum() =
    [<Test>]
    member this.Getvalue() =
        Assert.AreEqual(SourceConstructFlags.GetNames(typeof<CompilationRepresentationFlags>),
                        [|"None";"Static";"Instance";"ModuleSuffix";"UseNullAsTrueValue";"Event"|])
            
        ()
#endif

[<TestFixture>]
type MiscStuff() =
    [<Test>]
    member this.ListToString() =
        Assert.IsTrue( [].ToString() = "[]" )
        Assert.IsTrue( [1].ToString() = "[1]" )
        Assert.IsTrue( [1;2].ToString() = "[1; 2]" )
        Assert.IsTrue( [1;2;3].ToString() = "[1; 2; 3]" )
        Assert.IsTrue( [1;2;3;4].ToString() = "[1; 2; 3; ... ]" )
    [<Test>]
    member this.Refs() =
        let x = ref 0
        incr x
        incr x
        decr x
        Assert.IsTrue( 1 = !x )
        
        

[<TestFixture>]
type UnboxAndOptionStuff() =
    [<Test>]
    member this.TryUnbox() =
        Assert.IsTrue( tryUnbox (box ([] : int list)) = Some ([]: int list))
        Assert.IsTrue( tryUnbox (box ([1] : int list)) = Some ([1]: int list))
        Assert.IsTrue( tryUnbox (box ([] : string list)) = (None : int list option)) // Option uses 'null' as representation
        Assert.IsTrue( tryUnbox<int list> (box ([] : string list)) = None)
        Assert.IsTrue( tryUnbox (box (None : int option)) = Some (None: int option))
        Assert.IsTrue( tryUnbox (box (None : string option)) = Some (None: string option))
        Assert.IsTrue( tryUnbox (box (None : string option)) = Some (None: int option)) // Option uses 'null' as representation
        Assert.IsTrue( tryUnbox (box "") = Some "")
        Assert.IsTrue( tryUnbox<int option> (box null) = Some None) // Option uses 'null' as representation
        Assert.IsTrue( tryUnbox<int list> (box null) = None)
        Assert.IsTrue( tryUnbox<int> (box null) = None)
        Assert.IsTrue( tryUnbox<int> (box "1") = None)
        Assert.IsTrue( tryUnbox<int> (box 1) = Some 1)
        Assert.IsTrue( tryUnbox<string> (box "") = Some "")
        Assert.IsTrue( tryUnbox<string> (box 1) = None)
        
    member this.IsNull() =
        Assert.IsTrue( isNull (null : string))
        Assert.IsTrue( isNull (null : string[]))
        Assert.IsTrue( isNull (null : int[]))
        Assert.IsTrue( not (isNull [| |]))
        Assert.IsTrue( not (isNull ""))
        Assert.IsTrue( not (isNull "1"))