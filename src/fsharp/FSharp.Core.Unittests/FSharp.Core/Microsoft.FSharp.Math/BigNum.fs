// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Math.BigNum type

namespace SystematicUnitTests.FSharp_Core.Microsoft_FSharp_Math

open System
open SystematicUnitTests.LibraryTestFx
open NUnit.Framework
open Microsoft.FSharp.Math

(*
[Test Strategy]
Make sure each method works on:
* positive bignum
* negative bignum
* zero     bignum
* large    bignum
* DivideByZeroException
*)


[<TestFixture>]
type BigNum() =
    let g_positive1 = 1000000000000000000000000000000000018N
    let g_positive2 = 1000000000000000000000000000000000000N
    let g_negative1 = -1000000000000000000000000000000000018N
    let g_negative2 = -1000000000000000000000000000000000000N
    let g_negative3 = -1000000000000000000000000000000000036N
    let g_zero      = 0N
    let g_normal    = 88N
    let g_bigintpositive    = 1000000000000000000000000000000000018I
    let g_bigintnegative    = -1000000000000000000000000000000000018I
    
    // Interfaces
    [<Test>]
    member this.IComparable() =        
        // Legit IC
        let ic = g_positive1 :> IComparable    
        Assert.AreEqual(ic.CompareTo(g_positive1), 0) 
        CheckThrowsArgumentException( fun () -> ic.CompareTo(g_bigintpositive) |> ignore)
    
    // Base class methods
    [<Test>]
    member this.ObjectToString() =
        Assert.AreEqual(g_positive1.ToString(),
                        "1000000000000000000000000000000000018")
        Assert.AreEqual(g_zero.ToString(), "0") 
        Assert.AreEqual(g_normal.ToString(), "88")
        
        
    [<Test>]
    member this.GetHashCode() =
        Assert.AreEqual(g_negative1.GetHashCode(), 1210897093)
        Assert.AreEqual(g_normal.GetHashCode(), 89)
        Assert.AreEqual(g_zero.GetHashCode(), 1)
        ()
    
    // Static methods    
    [<Test>]
    member this.Abs() =
        Assert.AreEqual(bignum.Abs(g_negative1), g_positive1)
        Assert.AreEqual(bignum.Abs(g_negative2), g_positive2)
        Assert.AreEqual(bignum.Abs(g_positive1), g_positive1)
        Assert.AreEqual(bignum.Abs(g_normal), g_normal)
        Assert.AreEqual(bignum.Abs(g_zero), g_zero)
        ()
        
    [<Test>]
    member this.FromBigInt() =
        Assert.AreEqual(bignum.FromBigInt(g_bigintpositive),
                        g_positive1)
        Assert.AreEqual(bignum.FromBigInt(g_bigintnegative),
                        g_negative1)
        Assert.AreEqual(bignum.FromBigInt(0I), g_zero)
        Assert.AreEqual(bignum.FromBigInt(88I), g_normal)
        ()
    
    [<Test>]
    member this.FromInt() =
        Assert.AreEqual(bignum.FromInt(2147483647), 2147483647N)
        Assert.AreEqual(bignum.FromInt(-2147483648), -2147483648N)
        Assert.AreEqual(bignum.FromInt(0), 0N)
        Assert.AreEqual(bignum.FromInt(88), 88N)
        ()
        
    [<Test>]
    member this.One() =
        Assert.AreEqual(bignum.One, 1N)
        ()
    
    [<Test>]
    member this.Parse() =
        Assert.AreEqual(bignum.Parse("100"), 100N)
        Assert.AreEqual(bignum.Parse("-100"), -100N)
        Assert.AreEqual(bignum.Parse("0"), g_zero)
        Assert.AreEqual(bignum.Parse("88"), g_normal)
        ()
        
    [<Test>]
    member this.PowN() =
        Assert.AreEqual(bignum.PowN(100N, 2), 10000N)
        Assert.AreEqual(bignum.PowN(-3N, 3), -27N)
        Assert.AreEqual(bignum.PowN(g_zero, 2147483647), 0N)
        Assert.AreEqual(bignum.PowN(g_normal, 0), 1N)
        ()
        
        
    [<Test>]
    member this.Sign() =
        Assert.AreEqual(bignum.Sign(g_positive1), 1)
        Assert.AreEqual(bignum.Sign(g_negative1), -1)
        Assert.AreEqual(bignum.Sign(g_zero), 0)
        Assert.AreEqual(bignum.Sign(g_normal), 1)
        ()
        
    
        
    [<Test>]
    member this.ToBigInt() =
        Assert.AreEqual(bignum.ToBigInt(g_positive1), g_bigintpositive)
        Assert.AreEqual(bignum.ToBigInt(g_negative1), g_bigintnegative)
        Assert.AreEqual(bignum.ToBigInt(g_zero), 0I)
        Assert.AreEqual(bignum.ToBigInt(g_normal), 88I)
        ()
        
    
        
    [<Test>]
    member this.ToDouble() =
        Assert.AreEqual(double (179769N * 1000000000000000N), 1.79769E+20)
        Assert.AreEqual(double (-179769N * 1000000000000000N), -1.79769E+20)
        Assert.AreEqual(double 0N, 0.0)
        Assert.AreEqual(double 88N, 88.0)
        ()
        
        
    [<Test>]
    member this.ToInt32() =
        Assert.AreEqual(int32 2147483647N, 2147483647)
        Assert.AreEqual(int32 -2147483648N, -2147483648)
        Assert.AreEqual(int32 0N, 0)
        Assert.AreEqual(int32 88N, 88)
        
    
        
    [<Test>]
    member this.Zero() =
        Assert.AreEqual(bignum.Zero,  0N)
        ()
       
    // operator methods  
    [<Test>]
    member this.op_Addition() =
        
        Assert.AreEqual(100N + 200N, 300N)
        Assert.AreEqual((-100N) + (-200N), -300N)
        Assert.AreEqual(g_positive1 + g_negative1, 0N)
        Assert.AreEqual(g_zero + g_zero, 0N)
        Assert.AreEqual(g_normal + g_normal, 176N)
        Assert.AreEqual(g_normal + g_normal, 176N)
        ()
        
        
        
    [<Test>]
    member this.op_Division() =
        Assert.AreEqual(g_positive1 / g_positive1, 1N)
        Assert.AreEqual(-100N / 2N, -50N)
        Assert.AreEqual(g_zero / g_positive1, 0N)      
        ()
        
    [<Test>]
    member this.op_Equality() =
        
        Assert.IsTrue((g_positive1 = g_positive1))
        Assert.IsTrue((g_negative1 = g_negative1))
        Assert.IsTrue((g_zero = g_zero))
        Assert.IsTrue((g_normal = g_normal))
        ()
        
    [<Test>]
    member this.op_GreaterThan() = 
        Assert.AreEqual((g_positive1 > g_positive2), true)
        Assert.AreEqual((g_negative1 > g_negative2), false)
        Assert.AreEqual((g_zero > g_zero), false)
        Assert.AreEqual((g_normal > g_normal), false)
        
        
        ()
    [<Test>]
    member this.op_GreaterThanOrEqual() = 
        Assert.AreEqual((g_positive1 >= g_positive2), true)
        Assert.AreEqual((g_positive2 >= g_positive1), false)                                             
        Assert.AreEqual((g_negative1 >= g_negative1), true)
        Assert.AreEqual((0N >= g_zero), true)
        
        ()
    [<Test>]  
    member this.op_LessThan() = 
        Assert.AreEqual((g_positive1 < g_positive2), false)
        Assert.AreEqual((g_negative1 < g_negative3), false)
        Assert.AreEqual((0N < g_zero), false)
        
        ()
    [<Test>]
    member this.op_LessThanOrEqual() = 
        Assert.AreEqual((g_positive1 <= g_positive2), false)
        Assert.AreEqual((g_positive2 <= g_positive1), true)                                             
        Assert.AreEqual((g_negative1 <= g_negative1), true)
        Assert.AreEqual((0N <= g_zero), true)
       
        ()
    
    [<Test>]
    member this.op_Multiply() = 
        Assert.AreEqual(3N * 5N, 15N)
        Assert.AreEqual((-3N) * (-5N), 15N)
        Assert.AreEqual((-3N) * 5N, -15N)
        Assert.AreEqual(0N * 5N, 0N)
        
        ()
        
    [<Test>]
    member this.op_Range() = 
        let resultPos = [0N..2N]
        let seqPos    = [0N;1N;2N]                                                                
        VerifySeqsEqual resultPos seqPos
        
        let resultNeg = [-2N..0N]                                       
        let seqNeg =  [-2N;-1N;0N]  
        VerifySeqsEqual resultNeg seqNeg
        
        let resultSmall = [0N..5N]
        let seqSmall = [0N;1N;2N;3N;4N;5N]        
        VerifySeqsEqual resultSmall seqSmall
           
        ()
        
        
    [<Test>]
    member this.op_RangeStep() = 
        let resultPos = [0N..3N..6N]
        let seqPos    = [0N;3N;6N]                                                                
        VerifySeqsEqual resultPos seqPos
        
        let resultNeg = [-6N..3N..0N]                                        
        let seqNeg =  [-6N;-3N;0N]  
        VerifySeqsEqual resultNeg seqNeg
        
        let resultSmall = [0N..3N..9N]
        let seqSmall = [0N;3N;6N;9N]        
        VerifySeqsEqual resultSmall seqSmall
                   
        ()
        
    [<Test>]
    member this.op_Subtraction() = 
        Assert.AreEqual(g_positive1-g_positive2, 18N)
        Assert.AreEqual(g_negative1-g_negative3, 18N)
        Assert.AreEqual(0N-g_positive1, g_negative1)
        ()
        
    [<Test>]
    member this.op_UnaryNegation() = 
        Assert.AreEqual(-g_positive1,  g_negative1)
        Assert.AreEqual(-g_negative1, g_positive1)
        Assert.AreEqual(-0N, 0N) 
        
        ()
        
    [<Test>]
    member this.op_UnaryPlus() = 
        Assert.AreEqual(+g_positive1, g_positive1)
        Assert.AreEqual(+g_negative1, g_negative1)
        Assert.AreEqual(+0N, 0N)
        
        ()
    
    // instance methods
    [<Test>]
    member this.Denominator() = 
        Assert.AreEqual(g_positive1.Denominator, 1I)
        Assert.AreEqual(g_negative1.Denominator, 1I)
        Assert.AreEqual(0N.Denominator, 1I)
        
        ()       
    
    [<Test>]
    member this.IsNegative() = 
        Assert.IsFalse(g_positive1.IsNegative)
        Assert.IsTrue(g_negative1.IsNegative)
        Assert.IsFalse(0N.IsNegative)
        Assert.IsFalse(-0N.IsNegative)
        
        () 
        
        
    [<Test>]
    member this.IsPositive() = 
        Assert.IsTrue(g_positive1.IsPositive)
        Assert.IsFalse(g_negative1.IsPositive)
        Assert.IsFalse(0N.IsPositive)
        Assert.IsFalse(-0N.IsPositive)
        
        ()     
        
    [<Test>]
    member this.Numerator() = 
        Assert.AreEqual(g_positive1.Numerator, g_bigintpositive)
        Assert.AreEqual(g_negative1.Numerator, g_bigintnegative)
        Assert.AreEqual(0N.Numerator, 0I)
        
        ()  
        
    
        
        
   