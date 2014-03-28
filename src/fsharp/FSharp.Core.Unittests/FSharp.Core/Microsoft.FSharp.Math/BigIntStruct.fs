// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for bigint 

namespace SystematicUnitTests.FSharp_Core.Microsoft_FSharp_Math

open System
open SystematicUnitTests.LibraryTestFx
open NUnit.Framework
open Microsoft.FSharp.Math

(*
[Test Strategy]
Make sure each method works on:
* positive bigint
* negative bigint
* zero     bugint
* large    bigint
* DivideByZeroException
*)


[<TestFixture>]
type BigIntStruct() =
    // global variable
    let bigPositiveA = 12345678901234567890I
    let bigPositiveB = 98765432109876543210I
    let bigNegativeA = -bigPositiveA
    let bigNegativeB = -bigPositiveB
        
    // Interfaces
    [<Test>]
    member this.IComparable() =        
        // Legit IC
        let ic = bigPositiveA :> IComparable    
        Assert.AreEqual(ic.CompareTo(bigPositiveA), 0) 
               
    // Base class methods
    [<Test>]
    member this.ObjectToString() =
        Assert.AreEqual(bigPositiveA.ToString(), 
                        "12345678901234567890")
        Assert.AreEqual((new bigint(0)).ToString(),  "0")
        Assert.AreEqual((new bigint(168)).ToString(),  "168")
        Assert.AreEqual(-168I.ToString(), "-168")
        Assert.AreEqual(-0I.ToString(), "0")
        
    
    [<Test>]
    member this.ObjectEquals() =
        // All three are different constructor, but have equivalent value
        
        let a = new bigint(168)
        let b = 168I
        let c = new bigint(168L)
        Assert.IsTrue( (a = b) )
        Assert.IsTrue( (b = c) )
        Assert.IsTrue( (c = a) )
        Assert.IsTrue( a.Equals(b) ); Assert.IsTrue( b.Equals(a) )
        Assert.IsTrue( b.Equals(c) ); Assert.IsTrue( c.Equals(b) )
        Assert.IsTrue( c.Equals(a) ); Assert.IsTrue( a.Equals(c) )
        
        // Self equality
        let a = new bigint(168)
        Assert.IsTrue( (a = a) )
        Assert.IsTrue(a.Equals(a))
        
        // Null
        Assert.IsFalse(a.Equals(null))  
    
    // static methods
    [<Test>]
    member this.Abs() = 
        Assert.AreEqual(bigint.Abs(bigPositiveA),
                                   bigPositiveA)
        Assert.AreEqual(bigint.Abs(bigPositiveB),
                                   bigPositiveB)
        Assert.AreEqual(bigint.Abs(bigNegativeA),
                                   bigPositiveA)
        Assert.AreEqual(bigint.Abs(bigNegativeB),
                                   bigPositiveB)
        Assert.AreEqual(bigint.Abs(0I), 0I)
    
        ()
        
    [<Test>]
    member this.DivRem() = 
        Assert.AreEqual(bigint.DivRem(100I, 123I), (0I, 100I))
        Assert.AreEqual(bigint.DivRem(123I, 100I), (1I, 23I))
        Assert.AreEqual(bigint.DivRem(123I, -100I), (-1I, 23I))                              
        Assert.AreEqual(bigint.DivRem(0I, 1I), (0I, 0I)) 
        Assert.AreEqual(bigint.DivRem(-100I, -123I), (0I, -100I))
        Assert.AreEqual(bigint.DivRem(-123I, -100I), (1I, -23I))        
        Assert.AreEqual(bigint.DivRem(0I, 100I), (0I, 0I))
        CheckThrowsDivideByZeroException(fun() -> bigint.DivRem(100I, 0I) |> ignore) 
        
        ()
        
(*
    [<Test>]
    member this.Factorial() = 
        Assert.AreEqual(bigint.Factorial(0I), 1I)
        Assert.AreEqual(bigint.Factorial(1I), 1I)
        Assert.AreEqual(bigint.Factorial(5I), 120I)
        Assert.AreEqual(bigint.Factorial(10I), 3628800I)
        CheckThrowsArgumentException(fun() -> bigint.Factorial(-10I) |> ignore)
        
        ()
*)
        
    [<Test>]
    member this.GCD() = 
                            
        Assert.AreEqual(bigint.Gcd(bigPositiveA, bigPositiveB), 900000000090I)
        Assert.AreEqual(bigint.Gcd(bigNegativeA, bigNegativeB), 900000000090I)
        Assert.AreEqual(bigint.Gcd(0I, bigPositiveA), bigPositiveA)
                
        ()
        
    [<Test>]
    member this.One() = 
        Assert.AreEqual(bigint.One, 1I)
        
        ()
    [<Test>]
    member this.Parse() = 
        Assert.AreEqual(bigint.Parse("12345678901234567890"), 
                                     bigPositiveA)
        Assert.AreEqual(bigint.Parse("168"), 168I)
        Assert.AreEqual(bigint.Parse("000"), 0I)
        CheckThrowsArgumentException(fun() -> bigint.Parse("abc168L") |> ignore)
        CheckThrowsArgumentException(fun() -> bigint.Parse("") |> ignore)
        
        ()
        
    [<Test>]
    member this.Pow() = 
        Assert.AreEqual(bigint.Pow(2I, 3I), 8I)
        Assert.AreEqual(bigint.Pow(0I, 100I), 0I)
        Assert.AreEqual(bigint.Pow(-10I, 2I), 100I)
        CheckThrowsArgumentException(fun() -> bigint.Pow(100I, -2I) |> ignore)        
        
        ()
        
    [<Test>]
    member this.Sign() = 
        Assert.AreEqual(bigint.Sign(0I), 0)
        Assert.AreEqual(bigint.Sign(bigPositiveA), 1)
        Assert.AreEqual(bigint.Sign(bigNegativeA), -1)
               
        ()
    
    [<Test>]
    member this.ToDouble() = 
        Assert.AreEqual(double 0I, 0)
        Assert.AreEqual(double 123I, 123.0)
        Assert.AreEqual(double -123I, -123.0)
               
        ()
        
    [<Test>]
    member this.ToInt32() = 
        Assert.AreEqual(int32 0I, 0)
        Assert.AreEqual(int32 123I, 123)
        Assert.AreEqual(int32 -123I, -123)
               
        ()
        
    [<Test>]
    member this.ToInt64() = 
        Assert.AreEqual(int64 0I, 0)
        Assert.AreEqual(int64 123I, 123L)
        Assert.AreEqual(int64 -123I, -123L)
         
        ()
        
    [<Test>]
    member this.Zero() = 
        Assert.AreEqual(bigint.Zero, 0I)
         
        ()
     
    // operators    
    [<Test>]
    member this.op_Addition() = 
        Assert.AreEqual((123I + 456I), 579I)
        Assert.AreEqual((-123I + (-456I)), -579I)
        Assert.AreEqual((0I + 123I), 123I)
        Assert.AreEqual((bigPositiveA + 0I), bigPositiveA)
        Assert.AreEqual((bigPositiveA + bigNegativeA), 0I)                           
           
        ()
        
    [<Test>]
    member this.op_Division() = 
        Assert.AreEqual((123I / 124I), 0I)
        Assert.AreEqual((123I / (-124I)), 0I)
        Assert.AreEqual((0I / 123I), 0I) 
           
        ()
    
    [<Test>]    
    member this.op_Equality() = 
        Assert.AreEqual((bigPositiveA = bigPositiveA), true)
        Assert.AreEqual((bigPositiveA = bigNegativeA), false)                                   
        Assert.AreEqual((bigNegativeA = bigPositiveA), false)
        Assert.AreEqual((bigNegativeA = (-123I)), false)
        Assert.AreEqual((0I = new bigint(0)), true)
        
        ()
    
    [<Test>]    
    member this.op_GreaterThan() = 
        Assert.AreEqual((bigPositiveA > bigPositiveB), false)
        Assert.AreEqual((bigNegativeA > bigPositiveB), false)
        Assert.AreEqual((bigNegativeA > (-123I)), false)
        Assert.AreEqual((0I > new bigint(0)), false)
        
        ()
    
    [<Test>]    
    member this.op_GreaterThanOrEqual() = 
        Assert.AreEqual((bigPositiveA >= bigPositiveB), false)
        Assert.AreEqual((bigPositiveA >= bigNegativeB), true)
        Assert.AreEqual((bigPositiveB >= bigPositiveA), true)                                             
        Assert.AreEqual((bigNegativeA >= bigNegativeA), true)
        Assert.AreEqual((0I >= new bigint(0)), true)
        
        ()
    
    [<Test>]    
    member this.op_LessThan() = 
        Assert.AreEqual((bigPositiveA < bigPositiveB), true)
        Assert.AreEqual((bigNegativeA < bigPositiveB), true)
        Assert.AreEqual((bigPositiveA < bigNegativeB), false)
        Assert.AreEqual((bigNegativeA < bigPositiveB), true)
        Assert.AreEqual((0I < new bigint(0)), false)
        
        ()
    
    [<Test>]    
    member this.op_LessThanOrEqual() = 
        Assert.AreEqual((bigPositiveA <= bigPositiveB), true)
        Assert.AreEqual((bigPositiveA <= bigNegativeB), false)
        Assert.AreEqual((bigNegativeB <= bigPositiveA), true)                                             
        Assert.AreEqual((bigNegativeA <= bigNegativeA), true)        
        Assert.AreEqual((0I <= new bigint(-0)), true)
        
        ()
        
    [<Test>]
    member this.op_Modulus() = 
        Assert.AreEqual((bigPositiveA % bigPositiveB), bigPositiveA)
        Assert.AreEqual((bigNegativeA % bigNegativeB), bigNegativeA)
        Assert.AreEqual((0I % bigPositiveA), 0I)
           
        ()
        
    [<Test>]
    member this.op_Multiply() = 
        Assert.AreEqual((123I * 100I), 12300I)
        Assert.AreEqual((123I * (-100I)), -12300I)
        Assert.AreEqual((-123I * (-100I)), 12300I)
        Assert.AreEqual((0I * bigPositiveA), 0I)
        Assert.AreEqual((1I * 0I), 0I)
           
        ()
        
    [<Test>]
    member this.op_Range() = 
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
        
        let resultNeg = [(-128I)..(-123I)]                                      
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
        
        let resultSmall = [0I..5I]
        let seqSmall = [0I;1I;2I;3I;4I;5I]        
        VerifySeqsEqual resultSmall seqSmall
           
        ()
        
        
    [<Test>]
    member this.op_RangeStep() = 
        let resultPos = [100I..3I..109I]
        let seqPos    = 
            [
                100I
                103I
                106I
                109I
            ]                                                                 
        VerifySeqsEqual resultPos seqPos
        
        let resultNeg = [(-109I)..3I..(-100I)]                                   
        let seqNeg =  
            [
                -109I
                -106I
                -103I
                -100I
            ]         
        VerifySeqsEqual resultNeg seqNeg
        
        let resultSmall = [0I..3I..9I]
        let seqSmall = [0I;3I;6I;9I]        
        VerifySeqsEqual resultSmall seqSmall
                   
        ()
        
    [<Test>]
    member this.op_Subtraction() = 
        Assert.AreEqual((100I - 123I), -23I)
        Assert.AreEqual((0I - bigPositiveB), bigNegativeB)
        Assert.AreEqual((bigPositiveB - 0I), bigPositiveB)                                      
        Assert.AreEqual((-100I - (-123I)), 23I)
        Assert.AreEqual((100I - (-123I)), 223I)
        Assert.AreEqual((-100I - 123I), -223I)          
        
        ()
        
    [<Test>]
    member this.op_UnaryNegation() = 
        Assert.AreEqual(-bigPositiveA, bigNegativeA)
        Assert.AreEqual(-bigNegativeA, bigPositiveA)
        Assert.AreEqual(-0I, 0I) 
        
        ()
        
    [<Test>]
    member this.op_UnaryPlus() = 
        Assert.AreEqual(+bigPositiveA, bigPositiveA)
        Assert.AreEqual(+bigNegativeA, bigNegativeA)
        Assert.AreEqual(+0I, 0I) 
        
        ()
        
    // instance methods
    [<Test>]
    member this.New_int32() = 
        Assert.AreEqual(new bigint(0),  0I)
        Assert.AreEqual(new bigint(-10),  -10I)
        Assert.AreEqual(new bigint(System.Int32.MinValue), -2147483648I)        
        
        ()
        
    [<Test>]
    member this.New_int64() = 
        Assert.AreEqual(new bigint(0L),  0I)
        Assert.AreEqual(new bigint(-100L),  -100I)
        Assert.AreEqual(new bigint(System.Int64.MinValue),  -9223372036854775808I)
        
        ()
    
           