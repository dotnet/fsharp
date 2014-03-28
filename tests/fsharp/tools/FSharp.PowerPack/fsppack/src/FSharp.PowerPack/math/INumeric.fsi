namespace Microsoft.FSharp.Math

open Microsoft.FSharp.Math
open System.Numerics
open System

// A type-class for numeric types
type INumeric<'T> =
    abstract Zero: 'T
    abstract One: 'T
    abstract Add: 'T * 'T -> 'T
    abstract Equals : 'T * 'T -> bool
    abstract Compare : 'T * 'T -> int
    abstract Subtract: 'T * 'T -> 'T
    abstract Multiply : 'T * 'T -> 'T
    abstract Negate : 'T -> 'T
    abstract Sign : 'T -> int
    abstract Abs : 'T -> 'T    
    abstract ToString : 'T * string * System.IFormatProvider -> string
    abstract Parse : string * System.Globalization.NumberStyles * System.IFormatProvider -> 'T

type IIntegral<'T> =
    inherit INumeric<'T>
    abstract Modulus: 'T * 'T -> 'T
    abstract Divide : 'T * 'T -> 'T
    abstract DivRem : 'T * 'T -> 'T * 'T
    abstract ToBigInt : 'T -> BigInteger
    abstract OfBigInt : BigInteger -> 'T
  
type IFractional<'T> =
    inherit INumeric<'T>
    abstract Reciprocal : 'T -> 'T
    abstract Divide : 'T * 'T -> 'T

// Suggestion: IReal (since transcendentals are added here).
type IFloating<'T> =
    inherit IFractional<'T>
    abstract Pi : 'T
    abstract Exp : 'T -> 'T
    abstract Log : 'T -> 'T
    abstract Sqrt : 'T -> 'T
    abstract LogN : 'T * 'T -> 'T
    abstract Sin : 'T -> 'T
    abstract Cos : 'T -> 'T
    abstract Tan : 'T -> 'T
    abstract Asin : 'T -> 'T
    abstract Acos : 'T -> 'T
    abstract Atan : 'T -> 'T
    abstract Atan2 : 'T * 'T -> 'T
    abstract Sinh : 'T -> 'T
    abstract Cosh : 'T -> 'T
    abstract Tanh : 'T -> 'T

type INormFloat<'T> =
    abstract Norm : 'T -> float
  
// Direct access to IEEE encoding not easy on .NET
type IIEEE<'T> =
    inherit IFloating<'T>
    abstract PositiveInfinity : 'T
    abstract NegativeInfinity : 'T
    abstract NaN              : 'T
    abstract EpsilonOne       : 'T

    abstract IsNaN: 'T -> bool 
    abstract IsInfinite : 'T -> bool 
    //abstract IsDenormalized   : 'T -> bool 
    //abstract IsNegativeZero   : 'T -> bool 
    //abstract IsIEEE           : 'T -> bool 


module Instances =
    val Float32Numerics  : IFractional<float32> 
    val FloatNumerics    : IIEEE<float>
    val Int32Numerics    : IIntegral<int32>
    val Int64Numerics    : IIntegral<int64>
    val BigIntNumerics   : IIntegral<BigInteger>
    val BigNumNumerics   : IFractional<bignum>  



