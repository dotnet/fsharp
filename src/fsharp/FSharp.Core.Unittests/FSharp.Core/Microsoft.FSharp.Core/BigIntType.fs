// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// System.Numerics.BigInteger struct

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Math

#nowarn "52" // error FS0052: The value has been copied to ensure the original is not mutated by this operation 

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework
open Microsoft.FSharp.Math
open System.Numerics

(*
[Test Strategy]
Make sure each method works on:
* positive bigints
* negative bigints
* zero     bigints
* large    bigints
*)

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type BigIntType() =

    // global variables
    let bigPositiveA = 12345678901234567890I
    let bigPositiveB = 98765432109876543210I
    let bigNegativeA = -bigPositiveA
    let bigNegativeB = -bigPositiveB
        
    // Interfaces
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.IComparable() =        
        
        // Legit IC
        let ic = bigPositiveA :> IComparable    
        Assert.AreEqual(ic.CompareTo(bigPositiveA), 0) 
               
    // Base class methods
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.ObjectToString() =
        Assert.AreEqual(bigPositiveA.ToString(), 
                        "12345678901234567890")
        Assert.AreEqual((new BigInteger(0)).ToString(),   "0")
        Assert.AreEqual((new BigInteger(168)).ToString(), "168")
        Assert.AreEqual(-168I.ToString(), "-168")
        Assert.AreEqual(-0I.ToString(),   "0")
        Assert.AreEqual((BigInteger()).ToString(),   "0")
        
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.ObjectEquals() =
        // All three are different constructor, but have equivalent value
        
        let a = new BigInteger(168)
        let b = 168I
        let c = new BigInteger(168L)
        let z1 = 0I
        let z2 = BigInteger(0)
        let z3 = BigInteger.Zero
        let z4 = BigInteger()

        Assert.IsTrue( (a = b) )
        Assert.IsTrue( (b = c) )
        Assert.IsTrue( (c = a) )
        Assert.IsTrue( (z1 = z2) )
        Assert.IsTrue( (z2 = z3) )
        Assert.IsTrue( (z3 = z4) )
        Assert.IsTrue( (z4 = z1) )
        Assert.IsTrue( (z1 = -z2) )
        Assert.IsTrue( (z2 = -z3) )
        Assert.IsTrue( (z3 = -z4) )
        Assert.IsTrue( (z4 = -z1) )
        Assert.IsTrue( a.Equals(b) ); Assert.IsTrue( b.Equals(a) )
        Assert.IsTrue( b.Equals(c) ); Assert.IsTrue( c.Equals(b) )
        Assert.IsTrue( c.Equals(a) ); Assert.IsTrue( a.Equals(c) )
        Assert.IsTrue( z1.Equals(z2) ); Assert.IsTrue( z2.Equals(z3) )
        Assert.IsTrue( z3.Equals(z4) ); Assert.IsTrue( z4.Equals(z1) )
        
        // Self equality
        let a = new BigInteger(168)
        Assert.IsTrue( (a = a) )        
        Assert.IsTrue( (z1 = z1) )
        Assert.IsTrue( (z2 = z2) )
        Assert.IsTrue( (z3 = z3) )
        Assert.IsTrue( (z4 = z4) )
        Assert.IsTrue(a.Equals(a))
        Assert.IsTrue(z1.Equals(z1))
        Assert.IsTrue(z2.Equals(z2))
        Assert.IsTrue(z3.Equals(z3))
        Assert.IsTrue(z4.Equals(z4))
        
        // Null
        Assert.IsFalse(a.Equals(null))

        Assert.IsTrue(0I.GetHashCode() = (BigInteger()).GetHashCode())
    
    // static methods
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Abs() = 
        Assert.AreEqual(BigInteger.Abs(bigPositiveA),
                                   bigPositiveA)
        Assert.AreEqual(BigInteger.Abs(bigPositiveB),
                                   bigPositiveB)
        Assert.AreEqual(BigInteger.Abs(bigNegativeA),
                                   bigPositiveA)
        Assert.AreEqual(BigInteger.Abs(bigNegativeB),
                                   bigPositiveB)
        Assert.AreEqual(BigInteger.Abs(0I), 0I)
        Assert.AreEqual(BigInteger.Abs(BigInteger()), 0I)
    
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.DivRem() = 
        let mutable r = BigInteger(0)        
        let mutable q = BigInteger(0)
        let mutable qr = (BigInteger.Zero, BigInteger.Zero)
        
        q <- BigInteger.DivRem(100I, 123I, &r)
        Assert.AreEqual((q,r), (0I, 100I))
        
        q <- BigInteger.DivRem(123I, 100I, &r)
        Assert.AreEqual((q,r), (1I, 23I))
        
        q <- BigInteger.DivRem(123I, -100I, &r)
        Assert.AreEqual((q,r), (-1I, 23I))
        
        q <- BigInteger.DivRem(0I, 1I, &r)
        Assert.AreEqual((q,r), (0I, 0I)) 
        
        q <- BigInteger.DivRem(-100I, -123I, &r)
        Assert.AreEqual((q,r), (0I, -100I))
        
        q <- BigInteger.DivRem(-123I, -100I, &r)
        Assert.AreEqual((q,r), (1I, -23I))        
        
        q <- BigInteger.DivRem(0I, 100I, &r)
        Assert.AreEqual((q,r), (0I, 0I))

        qr <- BigInteger.DivRem(BigInteger(), 1I)
        Assert.AreEqual(qr, (0I, 0I))
       
        CheckThrowsDivideByZeroException(fun () -> BigInteger.DivRem(100I,0I) |> ignore)
        CheckThrowsDivideByZeroException(fun () -> BigInteger.DivRem(100I, BigInteger()) |> ignore)
        CheckThrowsDivideByZeroException(fun () -> BigInteger.DivRem(BigInteger(), BigInteger()) |> ignore)
        
        ()
        
       
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.GreatestCommonDivisor() = 
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(bigPositiveA, bigPositiveB), 900000000090I)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(bigNegativeA, bigNegativeB), 900000000090I)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(0I, bigPositiveA), bigPositiveA)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(BigInteger(), bigPositiveA), bigPositiveA)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(bigPositiveA, BigInteger()), bigPositiveA)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(BigInteger(), bigNegativeA), bigPositiveA)
        Assert.AreEqual(BigInteger.GreatestCommonDivisor(BigInteger(), BigInteger()), 0I)

        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.One() = 
        Assert.AreEqual(BigInteger.One,1I)
        
        ()
#if FSHARP_CORE_PORTABLE
#else
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Parse() = 
        Assert.AreEqual(BigInteger.Parse("12345678901234567890"),
                                     bigPositiveA)
        Assert.AreEqual(BigInteger.Parse("168"), 168I)
        Assert.AreEqual(BigInteger.Parse("000"), 0I)
        Assert.AreEqual(BigInteger.Parse("-100"), BigInteger(-100))
        Assert.AreEqual(BigInteger.Parse("+4242"), BigInteger(4242))
        Assert.AreEqual(BigInteger.Parse(" +4242   "), BigInteger(4242))
        Assert.AreEqual(BigInteger.Parse("+0003"), BigInteger(3))

        
        CheckThrowsFormatException(fun() -> BigInteger.Parse("abc168L") |> ignore)
        CheckThrowsFormatException(fun() -> BigInteger.Parse("") |> ignore)
        CheckThrowsFormatException(fun() -> BigInteger.Parse("   ") |> ignore)
        CheckThrowsFormatException(fun() -> BigInteger.Parse("-") |> ignore)
        CheckThrowsFormatException(fun() -> BigInteger.Parse("+") |> ignore)
        CheckThrowsFormatException(fun() -> BigInteger.Parse("+-") |> ignore)
        CheckThrowsArgumentNullException(fun() -> BigInteger.Parse(null) |> ignore)
        
        ()
#endif
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Pow() = 
        Assert.AreEqual(BigInteger.Pow(2I, 3),   8I)
        Assert.AreEqual(BigInteger.Pow(0I, 100), 0I)
        Assert.AreEqual(BigInteger.Pow(2I, 0),   1I)
        Assert.AreEqual(BigInteger.Pow(-10I, 2), 100I)
        Assert.AreEqual(BigInteger.Pow(0I, 0),   1I)
        Assert.AreEqual(BigInteger.Pow(BigInteger(), 0),   1I)
        Assert.AreEqual(BigInteger.Pow(0I, 1),   0I)
        Assert.AreEqual(BigInteger.Pow(BigInteger(), 1),   0I)
        CheckThrowsArgumentOutOfRangeException(fun() -> BigInteger.Pow(100I, -2) |> ignore)              
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Sign() = 
        Assert.AreEqual(0I.Sign,            0)
        Assert.AreEqual(BigInteger().Sign,  0)
        Assert.AreEqual(bigPositiveA.Sign,  1)
        Assert.AreEqual(bigNegativeA.Sign, -1)
        ()

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.IsZero() = 
        Assert.IsTrue(0I.IsZero)
        Assert.IsTrue(-0I.IsZero)
        Assert.IsTrue(BigInteger.Zero.IsZero)
        Assert.IsTrue((-BigInteger.Zero).IsZero)
        Assert.IsTrue(BigInteger().IsZero)
        Assert.IsTrue((-BigInteger()).IsZero)
        Assert.IsTrue(BigInteger(0).IsZero)
        Assert.IsTrue((-BigInteger(0)).IsZero)
        Assert.IsFalse(1I.IsZero)
        Assert.IsFalse(BigInteger.One.IsZero)
        Assert.IsFalse(-1I.IsZero)
        ()

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.IsOne() = 
        Assert.IsFalse(0I.IsOne)
        Assert.IsFalse(-0I.IsOne)
        Assert.IsFalse(BigInteger.Zero.IsOne)
        Assert.IsFalse((-BigInteger.Zero).IsOne)
        Assert.IsFalse(BigInteger().IsOne)
        Assert.IsFalse((-BigInteger()).IsOne)
        Assert.IsFalse(BigInteger(0).IsOne)
        Assert.IsFalse((-BigInteger(0)).IsOne)
        Assert.IsTrue(1I.IsOne)
        Assert.IsTrue(BigInteger.One.IsOne)
        Assert.IsTrue(BigInteger(1).IsOne)
        Assert.IsFalse(-1I.IsOne)
        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.ToDouble() = 
        Assert.AreEqual(double 0I,       0.0)
        Assert.AreEqual(double (BigInteger()), 0.0)
        Assert.AreEqual(double 123I,   123.0)
        Assert.AreEqual(double -123I, -123.0)
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.ToInt32() = 
        Assert.AreEqual(int32 0I,       0)
        Assert.AreEqual(int32 (BigInteger()), 0)
        Assert.AreEqual(int32 123I,   123)
        Assert.AreEqual(int32 -123I, -123)
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.ToInt64() = 
        Assert.AreEqual(int64 0I,       0L)
        Assert.AreEqual(int64 (BigInteger()), 0L)
        Assert.AreEqual(int64 123I,   123L)
        Assert.AreEqual(int64 -123I, -123L)
         
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Zero() = 
        Assert.AreEqual(BigInteger.Zero,0I)
        Assert.AreEqual(BigInteger.Zero, BigInteger())
        ()
     
    // operators    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Addition() = 
        Assert.AreEqual((123I + 456I),579I)
        Assert.AreEqual((-123I + (-456I)),-579I)
        Assert.AreEqual((0I + 123I),123I)
        Assert.AreEqual((BigInteger() + 123I),123I)
        Assert.AreEqual((123I + BigInteger()),123I)
        Assert.AreEqual((bigPositiveA + 0I),bigPositiveA)
        Assert.AreEqual((bigPositiveA + bigNegativeA),0I)                           
           
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Division() = 
        Assert.AreEqual((123I / 124I),0I)
        Assert.AreEqual((123I / (-124I)),0I)
        Assert.AreEqual((0I / 123I),0I) 
        Assert.AreEqual((BigInteger() / 123I),0I)

        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]    
    member this.Equality() = 
        Assert.AreEqual((bigPositiveA = bigPositiveA),true)
        Assert.AreEqual((bigPositiveA = bigNegativeA),false)                                   
        Assert.AreEqual((bigNegativeA = bigPositiveA),false)
        Assert.AreEqual((bigNegativeA = (-123I)),false)
        Assert.AreEqual((0I = new BigInteger(0)),true)
        Assert.AreEqual((0I = new BigInteger()),true)
        
        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]    
    member this.GreaterThan() = 
        Assert.AreEqual((bigPositiveA > bigPositiveB),false)
        Assert.AreEqual((bigNegativeA > bigPositiveB),false)
        Assert.AreEqual((bigNegativeA > (-123I)),false)
        Assert.AreEqual((0I > new BigInteger(0)),false)
        Assert.AreEqual((0I > new BigInteger()),false)
        Assert.AreEqual((BigInteger() > BigInteger()),false)
        Assert.AreEqual((BigInteger() > 1I),false)
        Assert.AreEqual((BigInteger() > -1I),true)
        
        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]    
    member this.GreaterThanOrEqual() = 
        Assert.AreEqual((bigPositiveA >= bigPositiveB),false)
        Assert.AreEqual((bigPositiveA >= bigNegativeB),true)
        Assert.AreEqual((bigPositiveB >= bigPositiveA),true)                                             
        Assert.AreEqual((bigNegativeA >= bigNegativeA),true)
        Assert.AreEqual((0I >= new BigInteger(0)),true)
        Assert.AreEqual((0I >= new BigInteger()),true)
        Assert.AreEqual((BigInteger() >= BigInteger()),true)
        Assert.AreEqual((BigInteger() >= 1I),false)
        Assert.AreEqual((BigInteger() >= -1I),true)
        
        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]    
    member this.LessThan() = 
        Assert.AreEqual((bigPositiveA < bigPositiveB),true)
        Assert.AreEqual((bigNegativeA < bigPositiveB),true)
        Assert.AreEqual((bigPositiveA < bigNegativeB),false)
        Assert.AreEqual((bigNegativeA < bigPositiveB),true)
        Assert.AreEqual((0I < new BigInteger(0)),false)
        Assert.AreEqual((0I < new BigInteger()),false)
        Assert.AreEqual((BigInteger() < BigInteger()),false)
        Assert.AreEqual((BigInteger() < 1I),true)
        Assert.AreEqual((BigInteger() < -1I),false)

        ()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]    
    member this.LessThanOrEqual() = 
        Assert.AreEqual((bigPositiveA <= bigPositiveB),true)
        Assert.AreEqual((bigPositiveA <= bigNegativeB),false)
        Assert.AreEqual((bigNegativeB <= bigPositiveA),true)                                             
        Assert.AreEqual((bigNegativeA <= bigNegativeA),true)        
        Assert.AreEqual((0I <= new BigInteger(-0)),true)
        Assert.AreEqual((0I <= new BigInteger()),true)
        Assert.AreEqual((BigInteger() <= BigInteger()),true)
        Assert.AreEqual((BigInteger() <= 1I),true)
        Assert.AreEqual((BigInteger() <= -1I),false)

        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Modulus() = 
        Assert.AreEqual((bigPositiveA % bigPositiveB),bigPositiveA)
        Assert.AreEqual((bigNegativeA % bigNegativeB),bigNegativeA)
        Assert.AreEqual((0I % bigPositiveA),0I)
        Assert.AreEqual((BigInteger() % bigPositiveA),0I)
        CheckThrowsDivideByZeroException(fun () -> 2I % 0I |> ignore)
        CheckThrowsDivideByZeroException(fun () -> 2I % (BigInteger()) |> ignore)

        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Multiply() = 
        Assert.AreEqual((123I * 100I),12300I)
        Assert.AreEqual((123I * (-100I)),-12300I)
        Assert.AreEqual((-123I * (-100I)),12300I)
        Assert.AreEqual((0I * bigPositiveA),0I)
        Assert.AreEqual((BigInteger() * bigPositiveA),0I)
        Assert.AreEqual((1I * 0I),0I)
           
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Range() = 
        let resultPos = [123I..128I]
        let seqPos = 
            [
                123I
                124I
                125I
                126I
                127I
                128I
            ]                                                           
        VerifySeqsEqual resultPos seqPos
        
        let resultNeg = [(-128I) .. (-123I)]                                      
        let seqNeg =  
            [
                -128I
                -127I
                -126I
                -125I
                -124I
                -123I
            ]   
        VerifySeqsEqual resultNeg seqNeg
        
        let resultSmall1 = [0I..5I]
        let resultSmall2 = [BigInteger()..5I]
        let seqSmall = [0I;1I;2I;3I;4I;5I]        
        VerifySeqsEqual resultSmall1 seqSmall
        VerifySeqsEqual resultSmall2 seqSmall
           
        ()
        
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.RangeStep() = 
        let resultPos = [100I .. 3I .. 109I]
        let seqPos    = 
            [
                100I
                103I
                106I
                109I
            ]                                                                 
        VerifySeqsEqual resultPos seqPos
        
        let resultNeg = [(-109I) .. 3I .. (-100I)]                                   
        let seqNeg =  
            [
                -109I
                -106I
                -103I
                -100I
            ]         
        VerifySeqsEqual resultNeg seqNeg
        
        let resultSmall1 = [0I..3I..9I]
        let resultSmall1 = [BigInteger()..3I..9I]
        let seqSmall = [0I;3I;6I;9I]        
        VerifySeqsEqual resultSmall1 seqSmall

        CheckThrowsArgumentException(fun () -> [0I .. BigInteger() .. 3I] |> ignore)

        VerifySeqsEqual [0I .. -2I .. 10I] []
                   
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.Subtraction() = 
        Assert.AreEqual((100I - 123I),-23I)
        Assert.AreEqual((0I - bigPositiveB),bigNegativeB)
        Assert.AreEqual((BigInteger() - bigPositiveB),bigNegativeB)
        Assert.AreEqual((bigPositiveB - BigInteger()),bigPositiveB)
        Assert.AreEqual((bigPositiveB - 0I),bigPositiveB)                                      
        Assert.AreEqual((-100I - (-123I)),23I)
        Assert.AreEqual((100I - (-123I)),223I)
        Assert.AreEqual((-100I - 123I),-223I)          
        
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.UnaryNegation() = 
        Assert.AreEqual(-bigPositiveA,bigNegativeA)
        Assert.AreEqual(-bigNegativeA,bigPositiveA)
        Assert.AreEqual(-0I,0I) 
        Assert.AreEqual(-BigInteger(),0I) 
        
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.UnaryPlus() = 
        Assert.AreEqual(+bigPositiveA,bigPositiveA)
        Assert.AreEqual(+bigNegativeA,bigNegativeA)
        Assert.AreEqual(+0I,0I)
        Assert.AreEqual(+BigInteger(),0I)
        
        ()
        
    // instance methods
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.New_int32() = 
        Assert.AreEqual(new BigInteger(0), 0I)
        Assert.AreEqual(new BigInteger(-10), -10I)
        Assert.AreEqual(new BigInteger(System.Int32.MinValue),-2147483648I)        
        
        ()
        
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.New_int64() = 
        Assert.AreEqual(new BigInteger(0L), 0I)
        Assert.AreEqual(new BigInteger(-100L), -100I)
        Assert.AreEqual(new BigInteger(System.Int64.MinValue), -9223372036854775808I)
        
        ()
    
           
